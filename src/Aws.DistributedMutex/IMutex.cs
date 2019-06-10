using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aws.DistributedMutex
{
    public interface IMutex
    {
        Task<LockToken> AcquireLockAsync(string resourceId, TimeSpan duration);

        /// <summary>
        /// Before expiry
        /// </summary>
        /// <returns></returns>
        Task<LockToken> RenewAsync(LockToken token, TimeSpan duration);

        Task ReleaseLockAsync(LockToken token);
    }
}
