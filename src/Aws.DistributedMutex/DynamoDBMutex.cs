using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aws.DistributedMutex
{
    public class DynamoDBMutex : IMutex
    {
        private readonly DynamoDBMutexSettings _settings;

        public DynamoDBMutex(DynamoDBMutexSettings settings = null)
            : this(new AmazonDynamoDBClient(), settings)
        {
            
        }

        public DynamoDBMutex(IAmazonDynamoDB client, DynamoDBMutexSettings settings = null)
        {
            _settings = settings ?? new DynamoDBMutexSettings();

        }

        public Task<LockToken> AcquireLockAsync(string resourceId, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseLockAsync(LockToken token)
        {
            throw new NotImplementedException();
        }
    }
}
