using System;
using System.Threading.Tasks;
using CacheStrategy_IntegrationTests.Models;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Factories;
using CacheStrategyImplementation.Implementation;
using CacheStrategyImplementation.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;

namespace CacheStrategy_IntegrationTests
{
    [TestClass]
    public class WriteThroughStrategyTest
    {
        //System under test
        private static ICosmosRepository _cosmosRepository;
        private static IRedisRepository _redisRepository;
        private static IWriteThroughStrategy _writeThroughStrategy;        
        private string _itemId = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5768";
        Volcano writeTestObject = new Volcano()
        {
            VolcanoName = "Abu",
            Country = "Japan",
            Region = "Honshu-Japan- modified",
            Location = new Location()
            {
                type = "Point",
                coordinates = new float[2] { 131.6F, 34.5F }
            },
            Elevation = 571,
            Type = "Shield volcano",
            Status = "Holocene",
            LastKnownEruption = "Unknown",
            id = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5768"
        };
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            IConfiguration configurationProvider = CreateConfigurationProvider();
            ICosmosFactory cosmosfactory = new CosmosFactory(configurationProvider);
            _cosmosRepository = A.Fake<SqlCosmosRepository>(arg=> arg.WithArgumentsForConstructor(new object[] { cosmosfactory }));
            IRedisCacheFactory redisCacheFactory = new RedisCacheFactory(configurationProvider);
            _redisRepository = new RedisRepository(redisCacheFactory);
            _writeThroughStrategy =  A.Fake<WriteThroughStrategy>(x =>
                                    x.WithArgumentsForConstructor(new Object[] { _redisRepository, _cosmosRepository }));

        }

        [TestMethod]
        public async Task Test_WriteThroughCache_SuccessUpdate()
        {
            //Act
            var updateResult = await _writeThroughStrategy.WriteToCacheAsync<Volcano>(_itemId, writeTestObject);
            //Assert
            Assert.IsTrue(updateResult);

        }

        [TestMethod]
        public async Task Test_WriteThroughCache_CacheFails()
        {
            //Use Fault Injection Techniques to simulate real-time behaviors
            //https://github.com/Polly-Contrib/Simmy
            //http://josephwoodward.co.uk/2020/01/chaos-engineering-your-dot-net-application-simmy

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
