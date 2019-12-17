namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using System;

    public class LockToken
    {
        public string ResourceId { get; }

        public string HolderId { get; }

        public DateTimeOffset LeaseExpiry { get; }

        public LockToken(
            string holderId,
            string resourceId,
            DateTimeOffset leaseExpiry)
        {
            ResourceId = resourceId;
            HolderId = holderId;
            LeaseExpiry = leaseExpiry;
        }

        public LockToken WithNewExpiry(DateTimeOffset newExpiry)
            => new LockToken(HolderId, ResourceId, newExpiry);
    }
}
