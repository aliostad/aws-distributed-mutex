using System;
using System.Runtime.Serialization;

namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;

public class AcquireLockFailedException : InvalidOperationException
{
    public AcquireLockFailedException()
        : base("Failed to acquire lease.")
    {
    }

    protected AcquireLockFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
