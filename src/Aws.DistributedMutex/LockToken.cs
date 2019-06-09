using System;
using System.Collections.Generic;
using System.Text;

namespace Aws.DistributedMutex
{
    public class LockToken
    {
        private readonly string _resourceId;

        public LockToken(string resourceId)
        {
            _resourceId = resourceId;
        }

        public string ResourceId => _resourceId;
    }
}
