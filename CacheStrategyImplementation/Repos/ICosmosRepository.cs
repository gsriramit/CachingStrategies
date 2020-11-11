using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CacheStrategyImplementation.Repos
{
    public interface ICosmosRepository
    {       
        Task<T> ReadItemAsync<T>(string documentId, string partitionKey);
        Task<bool> CreateItemAsync<T>(string documentId, T entity);
        Task<bool> UpsertItemAsync<T>(string documentId, T entity);
        Task<bool> ReplaceItemAsync<T>(string documentId, T entity);
        Task<bool> DeleteItemAsync<T>(string documentId, PartitionKey partitionKey);
    }

}
