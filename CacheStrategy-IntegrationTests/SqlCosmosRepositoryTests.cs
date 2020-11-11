using System;
using System.Threading.Tasks;
using CacheStrategy_IntegrationTests.Models;
using CacheStrategyImplementation.Factories;
using CacheStrategyImplementation.Repos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheStrategy_IntegrationTests
{
    [TestClass]
    public class SqlCosmosRepositoryTests
    {
        //System under test
        private static ICosmosRepository _cosmosRepository;
        private const string testKeyId = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5767";
        private const string newWriteKeyId = "";
        private Volcano writeTestObject = new Volcano()
        {
            VolcanoName = "Abu",
            Country = "Japan",
            Region = "Honshu-Japan",
            Location = new Location()
            {
                type = "Point",
                coordinates = new float[2] { 131.6F, 34.5F }
            },
            Elevation = 571,
            Type = "Shield volcano",
            Status = "Holocene",
            LastKnownEruption = "Unknown",
            id = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5766"
        };

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            IConfiguration configurationProvider = CreateConfigurationProvider();
            ICosmosFactory cosmosactory = new CosmosFactory(configurationProvider);
            _cosmosRepository = new SqlCosmosRepository(cosmosactory);

        }

        [TestMethod]
        public async Task Test_CreateItemAsync()
        {
            //Arrange - Prepare a test object 


            //Act
            bool cosmosWriteStatus = await _cosmosRepository.CreateItemAsync(writeTestObject.id, writeTestObject);
            //Assert
            Assert.IsTrue(cosmosWriteStatus);

        }

        [TestMethod]
        public async Task Test_UpsertItemAsync()
        {
            //Arrange - Prepare a test object 
            // Move this test object JSON to a separate test data file
            writeTestObject.id = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5767";

            //Act
            bool cosmosWriteStatus = await _cosmosRepository.UpsertItemAsync(writeTestObject.id, writeTestObject);
            //Assert
            Assert.IsTrue(cosmosWriteStatus);

        }

        [TestMethod]
        public async Task Test_ReplaceItemAsync()
        {
            //Arrange - Prepare a test object 
            // Move this test object JSON to a separate test data file
            
            writeTestObject.VolcanoName = "Some name to replace";

            //Act
            bool cosmosWriteStatus = await _cosmosRepository.ReplaceItemAsync(writeTestObject.id, writeTestObject);
            //Assert
            Assert.IsTrue(cosmosWriteStatus);

        }

        [TestMethod]
        public async Task Test_DeleteItemAsync()
        {
            //Arrange - Prepare a test object 
            PartitionKey partition = new PartitionKey("Japan");
            //Act
            bool cosmosWriteStatus = await _cosmosRepository.DeleteItemAsync<Volcano>(testKeyId, partition);
            //Assert
            Assert.IsTrue(cosmosWriteStatus);

        }

        private static IConfiguration CreateConfigurationProvider()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            return configuration;
        }
    }
}
