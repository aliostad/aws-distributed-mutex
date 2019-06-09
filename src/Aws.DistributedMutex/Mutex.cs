using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aws.DistributedMutex
{
    public class Mutex : IMutex
    {
        public Mutex()
        {

        }

        public Task<LockToken> AcquireLockAsync(string resourceId, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseLockAsync(LockToken token)
        {
            throw new NotImplementedException();
        }
    }
}
