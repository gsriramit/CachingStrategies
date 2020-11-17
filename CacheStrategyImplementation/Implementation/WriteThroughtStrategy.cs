using System;
using System.Threading.Tasks;
using CacheStrategyImplementation.Contracts;
using CacheStrategyImplementation.Repos;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Implementation
{
    public class WriteThroughStrategy : IWriteThroughStrategy
    {
        private readonly IRedisRepository _redisStore;
        private readonly ICosmosRepository _cosmosStore;

        public WriteThroughStrategy(IRedisRepository redisStore, ICosmosRepository cosmosStore)
        {
            _redisStore = redisStore;
            _cosmosStore = cosmosStore;
        }

        /// <summary>
        /// Method to perform update cache and cosmos in an atomic transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns>update status of cache and cosmos write operations </returns>
        
        /// <remarks>
        /// Used in read heavy systems. Example ratio of write to read - 1: 50
        /// The above example indicates that we read 50 times every 1 time we write
        /// Scenario 1: database update succeeds and cache update fails( even after trying N times using polly)
        /// Cache returns stale data for 'S' seconds until the key expires and we update the cache using Read-Through strategy(if used)
        /// Scenario 2: database write fails, hence cache write is cancelled - that write operation fails overall and data is lost
        /// Design using multiple instances of cache for better availability and fault tolerance is to be considered
        /// </remarks>
        public async Task<bool> WriteToCacheAsync<T>(string key, T item) where T : class
        {
            var result = false;
            //Create a transaction to execute the redis write and cosmos write as an atomic operation
            var redisTransaction = _redisStore.CreateRedisTranscation();
            // Create the operation to be performed on redis store 
            // This is not an actual write operation and need not be awaited. Also using await with this statement causes the thread execution to exit            
            _ = redisTransaction.StringSetAsync(new RedisKey(key), JsonConvert.SerializeObject(item));
            // Condition to be checked before committing the redis operation- write updated value to backing database
            if(await _cosmosStore.UpsertItemAsync<T>(key, item))
            {
                // execute the transaction. return value indicates the status of update
                result = redisTransaction.Execute();
            }
           
            return result;
        }
    }
}
