using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon;
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
        private readonly IAmazonDynamoDB _client;
        private readonly string _id = Guid.NewGuid().ToString("N");

        private class ColumnNames
        {
            public const string ResourceId = "resourceId";
            public const string LeaseExpiry = "leaseExpiry";
            public const string HolderId = "holderId";
        }

        /// <summary>
        /// Assumes access id and key are in the env vars or config
        /// </summary>
        /// <param name="endpoint">region</param>
        /// <param name="settings">settings</param>
        public DynamoDBMutex(RegionEndpoint endpoint, DynamoDBMutexSettings settings = null)
            : this (new AmazonDynamoDBClient(endpoint), settings)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">AWS DynamoDB client</param>
        /// <param name="settings">settings</param>
        public DynamoDBMutex(IAmazonDynamoDB client, DynamoDBMutexSettings settings = null)
        {
            _settings = settings ?? new DynamoDBMutexSettings();
            _client = client;
        }

        /// <inheritdoc />
        private async Task<Table> GetTableAsync()
        {
            if (_settings.CreateTableIfNotExists)
            {
                try
                {
                    await _client.CreateTableAsync(new CreateTableRequest()
                    {
                        TableName = _settings.TableName,
                        AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition()
                        {
                            AttributeType = ScalarAttributeType.S,
                            AttributeName = ColumnNames.ResourceId
                        },
                        new AttributeDefinition()
                        {
                            AttributeType = ScalarAttributeType.S,
                            AttributeName = ColumnNames.LeaseExpiry
                        },
                        new AttributeDefinition()
                        {
                            AttributeType = ScalarAttributeType.S,
                            AttributeName = ColumnNames.HolderId
                        }
                    },
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement()
                            {
                                KeyType = KeyType.HASH,
                                AttributeName = ColumnNames.ResourceId
                            }
                        },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 1,
                            WriteCapacityUnits = 1
                        }
                        });
                }
                catch (ResourceInUseException e)
                {
                    // ignore, already exists
                }
            }

            return Table.LoadTable(_client, _settings.TableName);
        }

        /// <inheritdoc />
        public async Task<LockToken> AcquireLockAsync(string resourceId, TimeSpan duration)
        {
            var table = await GetTableAsync();
            var doc = new Document();
            doc[ColumnNames.HolderId] = _id;
            doc[ColumnNames.ResourceId] = resourceId;
            var expiry = DateTimeOffset.UtcNow.Add(duration);
            doc[ColumnNames.LeaseExpiry] = expiry.ToString("O");

            var expr = new Expression();
            expr.ExpressionStatement = $"attribute_not_exists({ColumnNames.ResourceId}) OR {ColumnNames.LeaseExpiry} < :NOW";
            expr.ExpressionAttributeValues[":NOW"] = DateTimeOffset.UtcNow.ToString("O");

            try
            {
                await table.PutItemAsync(doc, new PutItemOperationConfig()
                {
                    ConditionalExpression = expr
                });

                return new LockToken(_id, resourceId, expiry);
            }
            catch (ConditionalCheckFailedException e)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task ReleaseLockAsync(LockToken token)
        {
            var table = await GetTableAsync();
            var doc = new Document();
            doc[ColumnNames.HolderId] = token.HolderId;
            doc[ColumnNames.ResourceId] = token.ResourceId;
            doc[ColumnNames.LeaseExpiry] = token.LeaseExpiry.ToString("O");
            await table.DeleteItemAsync(doc);
        }

        /// <inheritdoc />
        public async Task<LockToken> RenewAsync(LockToken token, TimeSpan duration)
        {
            var table = await GetTableAsync();
            var doc = new Document();
            doc[ColumnNames.HolderId] = token.HolderId;
            doc[ColumnNames.ResourceId] = token.ResourceId;
            var expiry = DateTimeOffset.UtcNow.Add(duration);
            doc[ColumnNames.LeaseExpiry] = expiry.ToString("O");

            var expr = new Expression();
            expr.ExpressionStatement = $"{ColumnNames.ResourceId} = :ID OR {ColumnNames.LeaseExpiry} < :NOW";
            expr.ExpressionAttributeValues[":NOW"] = DateTimeOffset.UtcNow.ToString("O");
            expr.ExpressionAttributeValues[":ID"] = token.ResourceId;

            try
            {
                await table.PutItemAsync(doc, new PutItemOperationConfig()
                {
                    ConditionalExpression = expr
                });

                return token.WithNewExpiry(expiry);
            }
            catch (ConditionalCheckFailedException e)
            {
                return null;
            }

        }
    }
}
