using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aws.DistributedMutext.Tests
{
    public class EnvVarIgnoreFactAttribute : FactAttribute
    {
        public EnvVarIgnoreFactAttribute(string envVar)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
            {
                Skip = $"Please set {envVar} env var to run.";
            }
        }
    }

    public class EnvVarIgnoreTheoryAttribute : TheoryAttribute
    {
        public EnvVarIgnoreTheoryAttribute(string envVar)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
            {
                Skip = $"Please set {envVar} env var to run.";
            }
        }
    }

    public class DynamoDBTests
    {
        [EnvVarIgnoreFact("aws_access_key_id")]
        public async Task Run()
        {
            var c = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            var t = Table.LoadTable(c, "__lock__");
            var d = new Document();
            d["resourceId"] = Guid.NewGuid().ToString();
            await t.PutItemAsync(d);
        }
    }


}

