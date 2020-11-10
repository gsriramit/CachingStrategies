using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CacheStrategyImplementation.Factories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Repos
{
    public class RedisRepository: IRedisRepository
    {       
        private static IConnectionMultiplexer Connection;
        public RedisRepository(IRedisCacheFactory cacheFactory)
        { 
            Connection = cacheFactory.CreateRedisConnection();
        }

        private static IDatabase GetCacheDatabase()
        {            
            return Connection.GetDatabase();
        }

        public async Task<T> ReadItemAsync<T>(string key) where T:class
        {
            RedisValue cacheValue = await GetCacheDatabase().StringGetAsync(key);
            T result = JsonConvert.DeserializeObject<T>(cacheValue.ToString());
            return result;
        }

        public async Task<bool> WriteItemAsync<T>(string key, T entity) where T : class
        {
            RedisKey updateKey= new RedisKey(key: key);
            bool updateStatus = await GetCacheDatabase().StringSetAsync(updateKey, JsonConvert.SerializeObject(entity));
            return updateStatus;
        }

        public async Task<bool> IfExistInCacheAsync(string key)
        {
            bool result = await GetCacheDatabase().KeyExistsAsync(key);
            return result;
        }

        public async Task<bool> RemoveFromCacheAsync(string key)
        {
            var cacheItemDeletionStatus = await GetCacheDatabase().KeyDeleteAsync(key);
            return cacheItemDeletionStatus;
        }

        public ITransaction CreateRedisTranscation()
        {
            var transaction = GetCacheDatabase().CreateTransaction();
            return transaction;
        }
    }
}
