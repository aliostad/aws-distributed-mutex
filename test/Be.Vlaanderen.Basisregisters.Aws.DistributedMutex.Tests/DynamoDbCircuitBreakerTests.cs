using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Be.Vlaanderen.Basisregisters.Containers.Testing;
using Xunit;

namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex.Tests
{
    public class DynamoDbCircuitBreakerTests
    {
        [Fact]
        public async Task OpenClose()
        {
            var composeFileName = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.yml");
            using var _ = Container.Compose(composeFileName, "bevlaanderenbasisregistersawsdistributedmutextests", 8000, "tcp");

            var credentials = new BasicAWSCredentials("fakeMyKeyId", "fakeSecretAccessKey");
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000",
                AuthenticationRegion="EU-West-1"
            };

            var dynamoDbClient = new AmazonDynamoDBClient(credentials, config);

            var sut = new DynamoDbCircuitBreaker(nameof(DynamoDbCircuitBreakerTests), TimeSpan.FromDays(1), dynamoDbClient);
            Assert.False(await sut.IsOpen());

            await sut.Close();
            Assert.False(await sut.IsOpen());

            await sut.Open();
            Assert.True(await sut.IsOpen());

            await sut.Close();
            Assert.False(await sut.IsOpen());
        }
    }
}
