using System;
using System.Threading.Tasks;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Repos;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Implementation
{
    public class WriteThroughStrategy : IWriteThroughStrategy
    {
        private readonly IRedisRepository _redisStore;
        private readonly ICosmosRepository _cosmosStore;

        public WriteThroughStrategy(IRedisRepository redisStore, ICosmosRepository cosmosStore)
        {
            _redisStore = redisStore;
            _cosmosStore = cosmosStore;
        }

        public Task<bool> WriteToCacheAsync<T>(string key, T item) where T : class
        {
            var result = false;
            var redisTransaction = _redisStore.CreateRedisTranscation().Result;
            
            redisTransaction.StringSetAsync(new RedisKey(key), JsonConvert.SerializeObject(item));
            if(_cosmosStore.CreateItemAsync<T>(key, item).Result)
            {
                result = redisTransaction.Execute();
            }
           
            return Task.FromResult(result);
        }
    }
}
