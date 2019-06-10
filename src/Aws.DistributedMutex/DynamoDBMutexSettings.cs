using System;
using System.Collections.Generic;
using System.Text;

namespace Aws.DistributedMutex
{
    public class DynamoDBMutexSettings
    {
        public string TableName { get; set; } = "__lock__";

        public bool CreateTableIfNotExists { get; set; } = false;
    }
}
