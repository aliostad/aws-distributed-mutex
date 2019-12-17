namespace Aws.DistributedMutex
{
    using System;
    using System.Timers;
    using Amazon;

    public class DistributedLockOptions
    {
        public RegionEndpoint Region { get; set; }
        public TimeSpan LeasePeriod { get; set; } = TimeSpan.FromMinutes(5);

        public bool ThrowOnFailedRenew { get; set; } = true;
        public bool TerminateApplicationOnFailedRenew { get; set; } = true;
    }

    public class DistributedLock<T>
    {
        private readonly DistributedLockOptions _options;

        private readonly DynamoDBMutex _mutex;
        private readonly Timer _renewLeaseTimer = new Timer();

        private LockToken _lockToken;

        public DistributedLock(DistributedLockOptions options)
        {
            _options = options;

            _mutex = new DynamoDBMutex(
                options.Region,
                new DynamoDBMutexSettings
                {
                    CreateTableIfNotExists = true,
                    TableName = "__DistributedLocks__"
                });

            _renewLeaseTimer.Elapsed += (sender, args) => RenewLease();
        }

        public bool AcquireLock()
        {
            _lockToken = _mutex.AcquireLockAsync(typeof(T).FullName, _options.LeasePeriod).GetAwaiter().GetResult();

            _renewLeaseTimer.Start();

            return _lockToken != null;
        }

        public void ReleaseLock()
        {
            _lockToken = null;
            _renewLeaseTimer.Stop();
        }

        private void RenewLease()
        {
            if (_lockToken == null)
                return;

            _lockToken = _mutex.RenewAsync(_lockToken, _options.LeasePeriod).GetAwaiter().GetResult();

            if (_lockToken == null && _options.ThrowOnFailedRenew)
                throw new Exception("Failed to renew lease.");

            if (_lockToken == null && _options.TerminateApplicationOnFailedRenew)
                Environment.Exit(1);
        }
    }

    //public class Example
    //{
    //    public void Main()
    //    {
    //        var distributedLock = new DistributedLock<Example>(
    //            new DistributedLockOptions
    //            {
    //                Region = RegionEndpoint.EUWest1,
    //                LeasePeriod = TimeSpan.FromMinutes(5),
    //                ThrowOnFailedRenew = true,
    //                TerminateApplicationOnFailedRenew = true
    //            });

    //        if (!distributedLock.AcquireLock())
    //        {
    //            Console.WriteLine("Could not get lock, another instance is busy");
    //            return;
    //        }

    //        // Do stuff

    //        distributedLock.ReleaseLock();
    //    }
    //}
}
