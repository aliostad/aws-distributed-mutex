using System;

namespace Aws.DistributedMutex
{
    public class LockToken
    {
        private readonly string _resourceId;
        private readonly string _holderId;
        private readonly DateTimeOffset _leaseExpiry;

        public LockToken(string holderId, string resourceId, DateTimeOffset leaseExpiry)
        {
            _resourceId = resourceId;
            _holderId = holderId;
            _leaseExpiry = leaseExpiry;
        }

        public string ResourceId => _resourceId;

        public string HolderId => _holderId;

        public DateTimeOffset LeaseExpiry => _leaseExpiry;

        public LockToken WithNewExpiry(DateTimeOffset newExpiry)
        {
            return new LockToken(_holderId, _resourceId, newExpiry);
        }
    }
}
