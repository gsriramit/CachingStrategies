using System;
using System.Threading.Tasks;

namespace CacheStrategyImplementation.Contracts
{
    public interface IReadThroughStrategy
    {
        Task<T> ReadFromCacheAsync<T>(string key) where T : class;
    }
}
