using System;
using System.Threading.Tasks;

namespace CacheStrategyImplementation.Contracts
{
    public interface IWriteThroughStrategy
    {
        Task<bool> WriteToCacheAsync<T>(string key, T item) where T : class;
    }
}
