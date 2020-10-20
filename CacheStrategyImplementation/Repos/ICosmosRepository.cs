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
        IDocumentQuery<T> CreateDocumentQuery<T>(FeedOptions queryFeedOptions,
            Expression<Func<T, bool>> filterPredicate);
        IDocumentQuery<T> CreateDocumentQuery<T>(FeedOptions queryFeedOptions,
            string sqlQuery);
        Task<T> ReadItemAsync<T>(string partitionKey);
        Task<bool> CreateItemAsync<T>(string partitionKey, T entity);
    }

}
