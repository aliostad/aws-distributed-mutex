using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using System;
    using System.Timers;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Microsoft.Extensions.Logging;

    public class DistributedLockConfiguration
    {
        public const string SectionName = "DistributedLock";

        public string Region { get; set; } = DistributedLockOptions.DefaultRegion;
        public string? AccessKeyId { get; set; }
        public string? AccessKeySecret { get; set; }
        public string TableName { get; set; } = DistributedLockOptions.DefaultTableName;
        public int LeasePeriodInMinutes { get; set; } = DistributedLockOptions.DefaultLeasePeriodInMinutes;
        public bool ThrowOnFailedRenew { get; set; } = DistributedLockOptions.DefaultThrowOnFailedRenew;
        public bool TerminateApplicationOnFailedRenew { get; set; } =
            DistributedLockOptions.DefaultTerminateApplicationOnFailedRenew;
        public bool ThrowOnFailedAcquire { get; set; } = DistributedLockOptions.DefaultThrowOnFailedAcquire;
        public bool TerminateApplicationOnFailedAcquire { get; set; } =
            DistributedLockOptions.DefaultTerminateApplicationOnFailedAcquire;

        public bool Enabled { get; set; } = true;
    }

    public class DistributedLockOptions
    {
        public static readonly string DefaultRegion = RegionEndpoint.EUWest1.SystemName;
        public const string DefaultTableName = "__DistributedLocks__";
        public const int DefaultLeasePeriodInMinutes = 5;
        public const bool DefaultThrowOnFailedRenew = true;
        public const bool DefaultTerminateApplicationOnFailedRenew = true;
        public const bool DefaultThrowOnFailedAcquire = false;
        public const bool DefaultTerminateApplicationOnFailedAcquire = false;

        public static DistributedLockOptions Defaults => new DistributedLockOptions();

        public RegionEndpoint Region { get; set; } = RegionEndpoint.GetBySystemName(DefaultRegion);

        public string? AwsAccessKeyId { get; set; }
        public string? AwsSecretAccessKey { get; set; }

        public string TableName { get; set; } = DefaultTableName;

        public TimeSpan LeasePeriod { get; set; } = TimeSpan.FromMinutes(DefaultLeasePeriodInMinutes);

        public bool ThrowOnFailedRenew { get; set; } = DefaultThrowOnFailedRenew;
        public bool TerminateApplicationOnFailedRenew { get; set; } = DefaultTerminateApplicationOnFailedRenew;
        public bool ThrowOnFailedAcquire { get; set; } = DefaultThrowOnFailedAcquire;
        public bool TerminateApplicationOnFailedAcquire { get; set; } = DefaultTerminateApplicationOnFailedAcquire;
        public bool Enabled { get; set; } = true;

        public static DistributedLockOptions LoadFromConfiguration(IConfiguration configuration)
        {
            var config = new DistributedLockConfiguration();
            configuration.GetSection(DistributedLockConfiguration.SectionName).Bind(config);

            return new DistributedLockOptions
            {
                Region = RegionEndpoint.GetBySystemName(config.Region),
                AwsAccessKeyId = config.AccessKeyId,
                AwsSecretAccessKey = config.AccessKeySecret,
                TableName = config.TableName,
                LeasePeriod = TimeSpan.FromMinutes(config.LeasePeriodInMinutes),
                ThrowOnFailedRenew = config.ThrowOnFailedRenew,
                TerminateApplicationOnFailedRenew = config.TerminateApplicationOnFailedRenew,
                ThrowOnFailedAcquire = config.ThrowOnFailedAcquire,
                TerminateApplicationOnFailedAcquire = config.TerminateApplicationOnFailedAcquire,
                Enabled = config.Enabled
            };
        }
    }

    public class DistributedLock<T>
    {
        private readonly DistributedLockOptions _options;
        private readonly ILogger _logger;
        private readonly string _lockName;

        private readonly IMutex _mutex;
        private readonly Timer _renewLeaseTimer = new Timer();

        private LockToken? _lockToken;

        private bool Disabled => !_options.Enabled;

        public DistributedLock(DistributedLockOptions options, ILogger logger)
            : this(options, typeof(T).FullName ?? Guid.NewGuid().ToString("N"), logger)
        {
        }

        public DistributedLock(DistributedLockOptions options, string lockName, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(lockName);
            ArgumentNullException.ThrowIfNull(logger);

            _options = options;
            _logger = logger;
            _lockName = lockName;

            _mutex = CreateMutex(options);
            if (_mutex == null)
            {
                throw new NullReferenceException($"{nameof(CreateMutex)} result can't be null");
            }

            _renewLeaseTimer.Interval = options.LeasePeriod.TotalMilliseconds / 2;
            _renewLeaseTimer.Elapsed += (sender, args) => RenewLease();
        }

        protected virtual IMutex CreateMutex(DistributedLockOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.AwsAccessKeyId) &&
                !string.IsNullOrWhiteSpace(options.AwsSecretAccessKey))
            {
                return new DynamoDBMutex(
                    new AmazonDynamoDBClient(
                        options.AwsAccessKeyId,
                        options.AwsSecretAccessKey,
                        options.Region),
                    new DynamoDBMutexSettings
                    {
                        CreateTableIfNotExists = true,
                        TableName = options.TableName
                    });
            }

            return new DynamoDBMutex(
                new AmazonDynamoDBClient(options.Region),
                new DynamoDBMutexSettings
                {
                    CreateTableIfNotExists = true,
                    TableName = options.TableName
                });
        }

        public static void Run(
            Action runFunc,
            DistributedLockOptions options,
            ILogger logger)
        {
            RunAsync(() =>
            {
                runFunc();
                return Task.CompletedTask;
            }, options, logger).GetAwaiter().GetResult();
        }

        public static async Task RunAsync(
            Func<Task> runFunc,
            DistributedLockOptions options,
            ILogger logger)
        {
            var distributedLock = new DistributedLock<T>(options, logger);
            await distributedLock.RunAsync(runFunc);
        }

        public void Run(
            Action runFunc)
        {
            RunAsync(() =>
            {
                runFunc();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();
        }

        public async Task RunAsync(Func<Task> runFunc)
        {
            var acquiredLock = false;
            try
            {
                _logger.LogInformation("Trying to acquire lock.");
                acquiredLock = AcquireLock();

                if (!acquiredLock)
                {
                    _logger.LogWarning("Could not get lock, another instance is busy.");
                    return;
                }

                await runFunc();
            }
            catch (Exception e)
            {
                _logger.LogCritical(0, e, "Encountered a fatal exception, exiting program.");
                throw;
            }
            finally
            {
                if (acquiredLock)
                {
                    ReleaseLock();
                }
            }
        }

        public bool AcquireLock()
        {
            if (Disabled)
            {
                _logger.LogWarning(
                    $"Bypassing the expected lock. DistributedLock for {typeof(T).Name} is disabled in configuration");
                return true;
            }

            _lockToken = _mutex.AcquireLockAsync(_lockName, _options.LeasePeriod).GetAwaiter().GetResult();

            if (_lockToken == null && _options.ThrowOnFailedAcquire)
            {
                throw new AcquireLockFailedException();
            }

            if (_lockToken == null && _options.TerminateApplicationOnFailedAcquire)
            {
                Environment.Exit(1);
            }

            _renewLeaseTimer.Start();

            return _lockToken != null;
        }

        public void ReleaseLock()
        {
            if (Disabled)
            {
                return;
            }

            _mutex.ReleaseLockAsync(_lockToken)
                .GetAwaiter()
                .GetResult();

            _lockToken = null;

            _renewLeaseTimer.Stop();
        }

        private void RenewLease()
        {
            if (Disabled || _lockToken == null)
            {
                return;
            }

            _lockToken = _mutex.RenewAsync(_lockToken, _options.LeasePeriod).GetAwaiter().GetResult();

            if (_lockToken == null && _options.ThrowOnFailedRenew)
            {
                throw new InvalidOperationException("Failed to renew lease.");
            }

            if (_lockToken == null && _options.TerminateApplicationOnFailedRenew)
            {
                Environment.Exit(1);
            }
        }
    }
}
