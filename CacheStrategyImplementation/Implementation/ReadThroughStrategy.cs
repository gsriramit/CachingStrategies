using System;
using System.Threading.Tasks;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Repos;

namespace CacheStrategyImplementation.Implementation
{
    public class ReadThroughStrategy : IReadThroughStrategy
    {
        private readonly IRedisRepository _redisStore;
        private readonly ICosmosRepository _cosmosStore;

        public ReadThroughStrategy(IRedisRepository redisStore, ICosmosRepository cosmosStore)
        {
            _redisStore = redisStore;
            _cosmosStore = cosmosStore;
        }

        public async Task<T> ReadFromCacheAsync<T>(string partitionKey, string itemKey) where T : class
        {            
            var cacheData = await _redisStore.ReadItemAsync<T>(itemKey);
            if (cacheData == null)
            {
                // take from CosmoDB and update cache
                var cosmoData = await  _cosmosStore.ReadItemAsync<T>(itemKey, partitionKey);
                await _redisStore.WriteItemAsync<T>(itemKey, cosmoData);
                return cosmoData;
            }

            return cacheData;
        }
    }
}
