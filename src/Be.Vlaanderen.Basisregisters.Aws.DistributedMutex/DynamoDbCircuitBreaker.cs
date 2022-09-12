namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;

    public class DynamoDbCircuitBreaker : ICircuitBreaker
    {
        private readonly TimeSpan _duration;
        private readonly DynamoDBMutex _mutex;
        private LockToken? _lockToken;

        public string ResourceName { get; }

        public DynamoDbCircuitBreaker(string resourceName, TimeSpan duration, AmazonDynamoDBClient? dynamoDbClient = null)
        {
            var client = dynamoDbClient ?? new AmazonDynamoDBClient();
            var settings = new DynamoDBMutexSettings { CreateTableIfNotExists = true };
            _mutex = new DynamoDBMutex(client, settings);


            _duration = duration;
            ResourceName = resourceName;
        }

        public async Task Open()
        {
            _lockToken = await _mutex.AcquireLockAsync(ResourceName, _duration);
        }

        public async Task Close()
        {
            if (_lockToken is not null)
            {
                await _mutex.ReleaseLockAsync(_lockToken);
            }
        }

        public async Task<bool> IsOpen(CancellationToken cancellationToken = default) => await _mutex.IsActiveAsync(ResourceName, cancellationToken);
    }
}
