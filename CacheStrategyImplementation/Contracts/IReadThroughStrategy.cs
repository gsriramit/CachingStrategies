using System;
using System.Threading.Tasks;

namespace CacheStrategyImplementation.Contracts
{
    public interface IReadThroughStrategy
    {
        Task<T> ReadFromCacheAsync<T>(string partitionKey, string itemKey) where T : class;
    }
}
