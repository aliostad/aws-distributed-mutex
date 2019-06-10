using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Aws.DistributedMutex;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aws.DistributedMutex.Tests
{
    public class EnvVarIgnoreFactAttribute : FactAttribute
    {
        public EnvVarIgnoreFactAttribute(string envVar)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
            {
                Skip = $"Please set {envVar} env var to run.";
            }
        }
    }

    public class EnvVarIgnoreTheoryAttribute : TheoryAttribute
    {
        public EnvVarIgnoreTheoryAttribute(string envVar)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
            {
                Skip = $"Please set {envVar} env var to run.";
            }
        }
    }

    public class DynamoDBMutexTests
    {
        [EnvVarIgnoreFact("aws_access_key_id")]
        public async Task CanGetLock()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var t = await m.AcquireLockAsync(Guid.NewGuid().ToString("N"), TimeSpan.FromSeconds(10));
            Assert.NotNull(t);
        }

        [EnvVarIgnoreFact("aws_access_key_id")]
        public async Task CanNotGetLockOnTheSameItemTwice()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t2 = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            Assert.Null(t2);
        }

        [EnvVarIgnoreFact("aws_access_key_id")]
        public async Task CanRenew()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var t2 = await m.RenewAsync(t, TimeSpan.FromSeconds(20));
            Assert.NotNull(t2);
        }

        [EnvVarIgnoreFact("aws_access_key_id")]
        public async Task CanNotRenewIfSomeoneElse()
        {
            var m = new DynamoDBMutex(RegionEndpoint.EUWest1);
            var resId = Guid.NewGuid().ToString("N");
            var t = await m.AcquireLockAsync(resId, TimeSpan.FromSeconds(10));
            var other = new LockToken(t.ResourceId, "other", t.LeaseExpiry);
            var t2 = await m.RenewAsync(other, TimeSpan.FromSeconds(20));
            Assert.Null(t2);
        }


        [EnvVarIgnoreFact("aws_access_key_id")]
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

    }
}

