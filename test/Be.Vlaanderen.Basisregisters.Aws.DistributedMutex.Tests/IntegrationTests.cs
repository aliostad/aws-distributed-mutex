namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex.Tests
{
    using Amazon;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class EnvVarIgnoreFactAttribute : FactAttribute
    {
        public EnvVarIgnoreFactAttribute(string envVar)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
                Skip = $"Please set {envVar} env var to run.";
        }
    }

    public class EnvVarIgnoreTheoryAttribute : TheoryAttribute
    {
        public EnvVarIgnoreTheoryAttribute(string envVar)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
                Skip = $"Please set {envVar} env var to run.";
        }
    }

    public class DynamoDBMutexTests
    {
        private const string EnvVarName = "aws_access_key_id";

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task CanGetLock()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var t = await m.AcquireLockAsync(Guid.NewGuid().ToString("N"), TimeSpan.FromSeconds(10));
            Assert.NotNull(t);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task CanNotGetLockOnTheSameItemTwice()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t2 = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            Assert.Null(t2);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task CanRenew()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t2 = await m.RenewAsync(t, TimeSpan.FromSeconds(20));
            Assert.NotNull(t2);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task CanNotRenewIfSomeoneElse()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var other = new LockToken(t.ResourceId, "other", t.LeaseExpiry);
            var t2 = await m.RenewAsync(other, TimeSpan.FromSeconds(20));
            Assert.Null(t2);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task Race()
        {
            var m1 = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var m2 = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var m3 = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t1 = await m1.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t2 = await m2.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t3 = await m3.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));

            var i = 0;
            if (t1 != null)
                i++;
            if (t2 != null)
                i++;
            if (t3 != null)
                i++;

            Assert.Equal(1, i);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task RaceAsync()
        {
            var m1 = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var m2 = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var m3 = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t1 = m1.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t2 = m2.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t3 = m3.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));

            Task.WaitAll(t1, t2, t3);

            var i = 0;
            if (t1.Result != null)
                i++;
            if (t2.Result != null)
                i++;
            if (t3.Result != null)
                i++;

            Assert.Equal(1, i);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task CanReleaseAndAcquireAgain()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            await m.ReleaseLockAsync(t);
            var t2 = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            Assert.NotNull(t2);
        }

        [EnvVarIgnoreFact(EnvVarName)]
        public async Task CreatesTableCorrectly()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1, new DynamoDBMutexSettings { TableName = "LockTestDeleteMe", CreateTableIfNotExists = true} );
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            await m.ReleaseLockAsync(t);
            var t2 = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            Assert.NotNull(t2);
        }
    }
}

