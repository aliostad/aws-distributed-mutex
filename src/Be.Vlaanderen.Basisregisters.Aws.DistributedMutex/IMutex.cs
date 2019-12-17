namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using System;
    using System.Threading.Tasks;

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
        /// Renews a lease if still valid or if expired, resource not leased by another holder
        /// </summary>
        /// <param name="token">original token</param>
        /// <param name="duration">duration</param>
        /// <returns>Null if unsuccessful otherwise the token</returns>
        Task<LockToken> RenewAsync(LockToken token, TimeSpan duration);

        /// <summary>
        /// Terminates the lease/lock only if the holder is still in possession of the lock
        /// </summary>
        /// <param name="token">original token</param>
        /// <returns></returns>
        Task ReleaseLockAsync(LockToken token);
    }
}
