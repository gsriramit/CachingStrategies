using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Repos;

namespace CacheStrategyImplementation.Implementation
{
    /*
    *Client/Service (Success)-> Write to Cache & DB -> Cache and DB Updated
                    (Failed)-> Retry 3 times for cache or DB (depending on the write order)
                    (Failed)-> push failed write to a Queue 
    */
    public class WriteThroughCache : ICacheStrategy
    {
        private readonly IRedisRepository _redisStore;
        private readonly ICosmosRepository _cosmosStore;
        public WriteThroughCache(IRedisRepository redisStore, ICosmosRepository cosmosStore)
        {
            _redisStore = redisStore;
            _cosmosStore = cosmosStore;
        }
        public Task<T> ReadFromCacheAsync<T>(string key) where T:class
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteToCacheAsync<T>(string key, T item) where T:class
        {
            // Implement a write through cache mechanism
            return Task.FromResult(false);
        }
    }
}
