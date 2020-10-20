using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Repos
{
    public interface IRedisRepository
    {
        Task<T> ReadItemAsync<T>(string key) where T : class;
        Task<bool> WriteItemAsync<T>(string key, T item) where T : class;
        Task<bool> IfExistInCacheAsync(string key);
        Task  RemoveFromCacheAsync(string key);
        ITransaction CreateRedisTranscation();
    }
}
