using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CacheStrategyImplementation.Repos
{
    public interface ICosmosRepository
    {       
        Task<T> ReadItemAsync<T>(string documentId, string partitionKey);
        Task<bool> CreateItemAsync<T>(string partitionKey, T entity);

        Task<bool> UpsertItemAsync<T>(string partitionKey, T entity);
    }

}
