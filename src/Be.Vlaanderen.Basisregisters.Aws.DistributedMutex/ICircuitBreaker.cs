namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICircuitBreaker
    {
        Task Open();
        Task Close();
        Task<bool> IsOpen(CancellationToken cancellationToken = default);
    }
}
