using System.Threading;

namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon;
    using Amazon.DynamoDBv2.DocumentModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    // ReSharper disable once InconsistentNaming
    public class DynamoDBMutex : IMutex, IMutexState
    {
        private readonly DynamoDBMutexSettings _settings;
        private readonly IAmazonDynamoDB _client;
        private readonly string _id = Guid.NewGuid().ToString("N");

        private static class ColumnNames
        {
            public const string ResourceId = "resourceId";
            public const string LeaseExpiry = "leaseExpiry";
            public const string HolderId = "holderId";
        }

        /// <summary>
        /// Assumes access id and key are in the environment variables or config
        /// </summary>
        /// <param name="endpoint">region</param>
        /// <param name="settings">settings</param>
        public DynamoDBMutex(
            RegionEndpoint endpoint,
            DynamoDBMutexSettings? settings = null)
            : this(new AmazonDynamoDBClient(endpoint), settings)
        { }

        /// <summary>
        /// </summary>
        /// <param name="client">AWS DynamoDB client</param>
        /// <param name="settings">settings</param>
        public DynamoDBMutex(
            IAmazonDynamoDB client,
            DynamoDBMutexSettings? settings = null)
        {
            _settings = settings ?? new DynamoDBMutexSettings();
            _client = client;
        }

        /// <inheritdoc />
        public async Task<LockToken?> AcquireLockAsync(
            string resourceId,
            TimeSpan duration)
        {
            var table = await GetTableAsync();

            var doc = new Document
            {
                [ColumnNames.HolderId] = _id,
                [ColumnNames.ResourceId] = resourceId
            };

            var expiry = DateTimeOffset.UtcNow.Add(duration);
            doc[ColumnNames.LeaseExpiry] = expiry.ToString("O");

            var expr = new Expression
            {
                ExpressionStatement = $"attribute_not_exists({ColumnNames.ResourceId}) OR {ColumnNames.LeaseExpiry} < :NOW",
                ExpressionAttributeValues =
                {
                    [":NOW"] = DateTimeOffset.UtcNow.ToString("O")
                }
            };

            try
            {
                await table.PutItemAsync(doc, new PutItemOperationConfig
                {
                    ConditionalExpression = expr
                });

                return new LockToken(_id, resourceId, expiry);
            }
            catch (ConditionalCheckFailedException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<LockToken?> RenewAsync(
            LockToken token,
            TimeSpan duration)
        {
            var table = await GetTableAsync();

            var doc = new Document
            {
                [ColumnNames.HolderId] = token.HolderId,
                [ColumnNames.ResourceId] = token.ResourceId
            };

            var expiry = DateTimeOffset.UtcNow.Add(duration);
            doc[ColumnNames.LeaseExpiry] = expiry.ToString("O");

            var expr = new Expression
            {
                ExpressionStatement = $"{ColumnNames.ResourceId} = :ID OR {ColumnNames.LeaseExpiry} < :NOW",
                ExpressionAttributeValues =
                {
                    [":NOW"] = DateTimeOffset.UtcNow.ToString("O"),
                    [":ID"] = token.ResourceId
                }
            };

            try
            {
                await table.PutItemAsync(doc, new PutItemOperationConfig
                {
                    ConditionalExpression = expr
                });

                return token.WithNewExpiry(expiry);
            }
            catch (ConditionalCheckFailedException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task ReleaseLockAsync(LockToken? token)
        {
            if (token is null)
            {
                return;
            }

            var table = await GetTableAsync();

            var doc = new Document
            {
                [ColumnNames.HolderId] = token.HolderId,
                [ColumnNames.ResourceId] = token.ResourceId,
                [ColumnNames.LeaseExpiry] = token.LeaseExpiry.ToString("O")
            };

            await table.DeleteItemAsync(doc);
        }

        private async Task<Table> GetTableAsync()
        {
            if (!_settings.CreateTableIfNotExists)
            {
                return Table.LoadTable(_client, _settings.TableName);
            }

            await CreateTableAsync();

            return Table.LoadTable(_client, _settings.TableName);
        }

        private async Task CreateTableAsync()
        {
            try
            {
                if (await DoesTableExists())
                {
                    return;
                }

                await _client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = _settings.TableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeType = ScalarAttributeType.S,
                            AttributeName = ColumnNames.ResourceId
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            KeyType = KeyType.HASH,
                            AttributeName = ColumnNames.ResourceId
                        }
                    },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                });

                // need to wait a bit since the table has just been created
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            catch (ResourceInUseException)
            {
                // ignore, already exists
            }
        }

        public async Task<bool> DoesTableExists()
        {
            // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LowLevelDotNetWorkingWithTables.html#LowLeveldotNetListTables
            // Initial value for the first page of table names.
            string? lastEvaluatedTableName = null;
            do
            {
                // Create a request object to specify optional parameters.
                var request = new ListTablesRequest { ExclusiveStartTableName = lastEvaluatedTableName };

                var result = await _client.ListTablesAsync(request);
                if (result.TableNames.Any(t => t == _settings.TableName))
                {
                    return true;
                }

                lastEvaluatedTableName = result.LastEvaluatedTableName;
            } while (lastEvaluatedTableName != null);

            return false;
        }

        public async Task<bool> IsActiveAsync(string resourceId, CancellationToken cancellationToken = default)
        {
            var table = await GetTableAsync();
            var doc = new Document
            {
                [ColumnNames.HolderId] = _id,
                [ColumnNames.ResourceId] = resourceId
            };

            var response = await table.GetItemAsync(doc, new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { ColumnNames.LeaseExpiry },
                ConsistentRead = true
            }, cancellationToken);

            return response is not null
                && DateTimeOffset.TryParse(response[ColumnNames.LeaseExpiry], out var dateTimeOffset)
                && dateTimeOffset >= DateTimeOffset.UtcNow;
        }
    }
}
