namespace Aws.DistributedMutex
{
    public class DynamoDBMutexSettings
    {
        public string TableName { get; set; } = "__lock__";

        public bool CreateTableIfNotExists { get; set; } = false;
    }
}
