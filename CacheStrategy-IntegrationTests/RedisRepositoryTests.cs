using CacheStrategy_IntegrationTests.Models;
using CacheStrategyImplementation.Factories;
using CacheStrategyImplementation.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace CacheStrategy_IntegrationTests
{
    [TestClass]
    public class RedisRepositoryTests
    {
        //System under test
        private static IRedisRepository _redisRepository;
        private const string testKeyId = "";
        private const string newWriteKeyId = "";
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            IConfiguration configurationProvider = CreateConfigurationProvider();
            IRedisCacheFactory redisCacheFactory = new RedisCacheFactory(configurationProvider);
            _redisRepository = new RedisRepository(redisCacheFactory);

        }

        [TestMethod]
        public async Task Test_ReadItemAsync()
        {
            //Arrange

            //Act
            Volcano readObject = await _redisRepository.ReadItemAsync<Volcano>(testKeyId);
            //Assert
            Assert.IsNotNull(readObject);
            Assert.IsInstanceOfType(readObject, typeof(Volcano));

        }

        [TestMethod]
        public async Task Test_WriteItemAsync()
        {
            //Arrange - Prepare a test object 
            // Move this test object JSON to a separate test data file
            Volcano writeTestObject = new Volcano()
            {
                VolcanoName = "Abu",
                Country = "Japan",
                Region = "Honshu-Japan",
                Location = {
                type= "Point",
                coordinates= new float[2]{131.6F, 34.5F }
                },
                Elevation = 571,
                Type = "Shield volcano",
                Status = "Holocene",
                LastKnownEruption = "Unknown",
                id = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5766"
            };
            //Act
            bool cacheWriteStatus = await _redisRepository.WriteItemAsync<Volcano>(writeTestObject.id, writeTestObject);
            //Assert
            Assert.IsTrue(cacheWriteStatus);

        }

        [TestMethod]
        public async Task Test_IfExistInCacheAsync()
        {
            //Arrange

            //Act
            bool checkKeyExists = await _redisRepository.IfExistInCacheAsync(testKeyId);
            //Assert
            Assert.IsTrue(checkKeyExists);
        }

        [TestMethod]
        public async Task Test_RemoveFromCacheAsync()
        {
            //Arrange

            //Act
            bool deletionStatus = await _redisRepository.RemoveFromCacheAsync(testKeyId);
            //Assert
            Assert.IsTrue(deletionStatus);
        }

        [TestMethod]
        public void Test_CreateRedisTranscation()
        {
            //Arrange

            //Act
            var transaction = _redisRepository.CreateRedisTranscation();
            //Assert
            Assert.IsInstanceOfType(transaction, typeof(ITransaction));

        }


        private static IConfiguration CreateConfigurationProvider()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath("")
            .AddJsonFile("appsettings.json")
            .Build();
            return configuration;
        }

    }
}
