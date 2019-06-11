using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aws.DistributedMutex
{
    public interface IMutex
    {
        /// <summary>
        /// Acquires lease (lock) on the resource
        /// </summary>
        /// <param name="resourceId">a string representation of the resource</param>
        /// <param name="duration">duration</param>
        /// <returns>Null if unsuccessful otherwise the token</returns>
        Task<LockToken> AcquireLockAsync(string resourceId, TimeSpan duration);

        /// <summary>
        /// Renews a lease if still valid or if expired, resource not leased by anoyther holder
        /// </summary>
        /// <param name="token">original token</param>
        /// <param name="duration">duration</param>
        /// <returns>Null if unsuccessful otherwise the token</returns>
        Task<LockToken> RenewAsync(LockToken token, TimeSpan duration);

        /// <summary>
        /// Terminates the lease/lock only if the holder is still in possession of the lock
        /// </summary>
        /// <param name="token">originakl token</param>
        /// <returns></returns>
        Task ReleaseLockAsync(LockToken token);
    }
}
