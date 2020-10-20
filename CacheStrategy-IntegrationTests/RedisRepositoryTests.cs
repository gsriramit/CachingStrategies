using CacheStrategy_IntegrationTests.Models;
using CacheStrategyImplementation.Factories;
using CacheStrategyImplementation.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
