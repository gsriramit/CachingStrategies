using System;
using System.IO;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Factories;
using CacheStrategyImplementation.Implementation;
using CacheStrategyImplementation.Repos;
using Microsoft.Extensions.Configuration;

namespace CacheTestStartUp
{
    class Program
    {
        private const string ConfigFileName = "appConfig.json";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            IConfiguration config =  new ConfigurationBuilder().
                SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "AppSettings"))
                .AddJsonFile(ConfigFileName)
                .Build();
            //Create the redis factory - substitute any factory in this place
            IRedisCacheFactory redisFactory = new RedisCacheFactory(config);
            //Create the cosmos factory
            ICosmosFactory cosmosFactory = new CosmosFactory(config);
            //Create the redis store instance
            IRedisRepository redisStore = new RedisRepository(redisFactory);
            //Create the cosmos store instance
            ICosmosRepository cosmosStore = new SqlCosmosRepository(cosmosFactory);
            //create the write-through cache strategy
            ICacheStrategy cacheStrategy = new WriteThroughCache(redisStore, cosmosStore);
            //create an instance of the test execution class
            var cacheOperationsTest = new ExecuteCacheStrategy(cacheStrategy);
            
            // Read Values from command line or read data from file

        }
       
    }

    public class ExecuteCacheStrategy
    {
        private readonly ICacheStrategy _cacheStrategy;
        public ExecuteCacheStrategy(ICacheStrategy cacheStrategy)
        {
            _cacheStrategy = cacheStrategy;
        }


    }
}
