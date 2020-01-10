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
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string TableName { get; set; } = DistributedLockOptions.DefaultTableName;
        public int LeasePeriodInMinutes { get; set; } = DistributedLockOptions.DefaultLeasePeriodInMinutes;
        public bool ThrowOnFailedRenew { get; set; } = DistributedLockOptions.DefaultThrowOnFailedRenew;
        public bool TerminateApplicationOnFailedRenew { get; set; } = DistributedLockOptions.DefaultTerminateApplicationOnFailedRenew;
        public bool Enabled { get; set; } = true;
    }

    public class DistributedLockOptions
    {
        public static string DefaultRegion = RegionEndpoint.EUWest1.SystemName;
        public static string DefaultTableName = "__DistributedLocks__";
        public static int DefaultLeasePeriodInMinutes = 5;
        public static bool DefaultThrowOnFailedRenew = true;
        public static bool DefaultTerminateApplicationOnFailedRenew = true;

        public static DistributedLockOptions Defaults => new DistributedLockOptions();

        public RegionEndpoint Region { get; set; } = RegionEndpoint.GetBySystemName(DefaultRegion);

        public string AwsAccessKeyId { get; set; }
        public string AwsSecretAccessKey { get; set; }

        public string TableName { get; set; } = DefaultTableName;

        public TimeSpan LeasePeriod { get; set; } = TimeSpan.FromMinutes(DefaultLeasePeriodInMinutes);

        public bool ThrowOnFailedRenew { get; set; } = DefaultThrowOnFailedRenew;
        public bool TerminateApplicationOnFailedRenew { get; set; } = DefaultTerminateApplicationOnFailedRenew ;
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
                Enabled = config.Enabled
            };
        }
    }

    public class DistributedLock<T>
    {
        private readonly DistributedLockOptions _options;
        private readonly ILogger _logger;
        private readonly string _lockName;

        private readonly DynamoDBMutex _mutex;
        private readonly Timer _renewLeaseTimer = new Timer();

        private LockToken? _lockToken;

        private bool Disabled => !_options.Enabled;

        public DistributedLock(DistributedLockOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;

            _lockName = typeof(T).FullName ?? Guid.NewGuid().ToString("N");

            _mutex = new DynamoDBMutex(
                new AmazonDynamoDBClient(
                    options.AwsAccessKeyId,
                    options.AwsSecretAccessKey,
                    options.Region),
                new DynamoDBMutexSettings
                {
                    CreateTableIfNotExists = true,
                    TableName = options.TableName
                });

            _renewLeaseTimer.Interval = options.LeasePeriod.TotalMilliseconds / 2;
            _renewLeaseTimer.Elapsed += (sender, args) => RenewLease();
        }

        public static void Run(
            Action runFunc,
            DistributedLockOptions options,
            ILogger logger)
        {
            var distributedLock = new DistributedLock<T>(options, logger);

            var acquiredLock = false;
            try
            {
                logger.LogInformation("Trying to acquire lock.");
                acquiredLock = distributedLock.AcquireLock();

                if (!acquiredLock)
                {
                    logger.LogInformation("Could not get lock, another instance is busy.");
                    return;
                }

                runFunc();
            }
            catch (Exception e)
            {
                logger.LogCritical(0, e, "Encountered a fatal exception, exiting program.");
                throw;
            }
            finally
            {
                if (acquiredLock)
                    distributedLock.ReleaseLock();
            }
        }

        public bool AcquireLock()
        {
            if (Disabled)
            {
                _logger.LogWarning($"Bypassing the expected lock. DistributedLock for {typeof(T).Name} is disabled in configuration");
                return true;
            }

            _lockToken = _mutex.AcquireLockAsync(_lockName, _options.LeasePeriod).GetAwaiter().GetResult();

            _renewLeaseTimer.Start();

            return _lockToken != null;
        }

        public void ReleaseLock()
        {
            if (Disabled)
                return;

            _mutex.ReleaseLockAsync(_lockToken).GetAwaiter().GetResult();

            _lockToken = null;

            _renewLeaseTimer.Stop();
        }

        private void RenewLease()
        {
            if (Disabled || _lockToken == null)
                return;

            _lockToken = _mutex.RenewAsync(_lockToken, _options.LeasePeriod).GetAwaiter().GetResult();

            if (_lockToken == null && _options.ThrowOnFailedRenew)
                throw new Exception("Failed to renew lease.");

            if (_lockToken == null && _options.TerminateApplicationOnFailedRenew)
                Environment.Exit(1);
        }
    }
}
