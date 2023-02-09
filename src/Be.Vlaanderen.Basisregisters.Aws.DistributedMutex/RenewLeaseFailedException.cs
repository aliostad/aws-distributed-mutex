using System;
using System.Runtime.Serialization;

namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;

[Serializable]
public class RenewLeaseFailedException : InvalidOperationException
{
    public RenewLeaseFailedException()
        : base("Failed to renew lease.")
    {
    }

    protected RenewLeaseFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
