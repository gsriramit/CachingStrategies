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
    public class ReadThroughStrategyTest
    {
        //System under test
        private static ICosmosRepository _cosmosRepository;
        private static IRedisRepository _redisRepository;
        private static IReadThroughStrategy _readThroughStrategy;
        private string _partitionKey = "Japan";
        private string _itemId = "4cb67ab0-ba1a-0e8a-8dfc-d48472fd5766";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            IConfiguration configurationProvider = CreateConfigurationProvider();
            ICosmosFactory cosmosfactory = new CosmosFactory(configurationProvider);
            _cosmosRepository = A.Fake<ICosmosRepository>();
            IRedisCacheFactory redisCacheFactory = new RedisCacheFactory(configurationProvider);
            _redisRepository = new RedisRepository(redisCacheFactory);
            _readThroughStrategy =  A.Fake<ReadThroughStrategy>(x =>
                                    x.WithArgumentsForConstructor(new Object[] { _redisRepository, _cosmosRepository }));

        }

        [TestMethod]
        public async Task Test_ReadFromCache_CacheHit()
        {

            //Act
           var volcanoObj = await _readThroughStrategy.ReadFromCacheAsync<Volcano>(_partitionKey, _itemId);

            //Assert
            A.CallTo(() => _cosmosRepository.ReadItemAsync<Volcano>(_itemId, _partitionKey)).MustNotHaveHappened();
            Assert.AreEqual(volcanoObj.id, _itemId);
        }

        [TestMethod]
        public async Task Test_ReadFromCache_CacheMiss()
        {

            //Act
            var volcanoObj = await _readThroughStrategy.ReadFromCacheAsync<Volcano>(_partitionKey, _itemId);

            //Assert
            A.CallTo(() => _cosmosRepository.ReadItemAsync<Volcano>(_itemId, _partitionKey)).MustHaveHappened();
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
