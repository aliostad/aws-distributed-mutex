using System.Threading;
using System.Threading.Tasks;

namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    public interface IMutexState
    {
        Task<bool> IsActiveAsync(string resourceId, CancellationToken cancellationToken = default);
    }
}
