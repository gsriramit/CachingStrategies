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
        private readonly IRedisCacheFactory _cacheFactory;
        private static Lazy<IConnectionMultiplexer> _connectionMultiplexer;
        public RedisRepository(IRedisCacheFactory cacheFactory)
        {
            _cacheFactory = cacheFactory;
            _connectionMultiplexer = _cacheFactory.CreateRedisConnection();
        }

        public static IConnectionMultiplexer Connection => _connectionMultiplexer.Value;

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

        public async Task RemoveFromCacheAsync(string key)
        {
            await GetCacheDatabase().KeyDeleteAsync(key);
        }
    }
}
