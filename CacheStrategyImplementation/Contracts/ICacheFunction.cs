using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CacheStrategyImplementation.Contracts
{
    public interface ICacheStrategy
    {
        Task<T> ReadFromCacheAsync<T>(string itemKey, string datastorePartitionId) where T : class;
        Task<bool> WriteToCacheAsync<T>(string key, T item) where T : class;
    }
}
