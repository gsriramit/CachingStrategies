using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Repos;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Implementation
{
    /*
    *Client/Service (Success)-> Write to Cache & DB -> Cache and DB Updated
                    (Failed)-> Retry 3 times for cache or DB (depending on the write order)
                    (Failed)-> push failed write to a Queue 
    */
    public class WriteThroughCache : ICacheStrategy
    {
        private readonly IWriteThroughStrategy _writeStrategy;
        private readonly IReadThroughStrategy _readStrategy;

        public WriteThroughCache(IRedisRepository redisStore, ICosmosRepository cosmosStore, IWriteThroughStrategy writeStrategy, IReadThroughStrategy readStrategy)
        {
            _writeStrategy = writeStrategy;
            _readStrategy = readStrategy;
        }
        public async Task<T> ReadFromCacheAsync<T>(string key) where T:class
        {
            return await _readStrategy.ReadFromCacheAsync<T>(key);
        }

        public async Task<bool> WriteToCacheAsync<T>(string key, T item) where T:class
        {
            return await _writeStrategy.WriteToCacheAsync<T>(key, item);
        }
    }
}
