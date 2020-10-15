using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CacheStrategyImplementation.Configuration;
using CacheStrategyImplementation.Factories;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Polly;

namespace CacheStrategyImplementation.Repos
{
    public class SqlCosmosRepository: ICosmosRepository
    {
        private readonly IDocumentClient _documentClient;
        private readonly Uri _documentCollectionUri;
        private readonly Uri _databaseUri;
        private readonly IAsyncPolicy _resiliencyPolicy;
        public SqlCosmosRepository(ICosmosFactory cosmosFactory)
        {
            _documentClient = cosmosFactory.CreateCosmosConnection();
            _resiliencyPolicy = cosmosFactory.CreateResiliencyAsyncPolicy();

            // Open the connection to the cosmos database. This is done to avoid the initial delays in the query execution.
            // Part of the suggested best practices
            //_documentClient.OpenAsync();
        }

        public IDocumentQuery<T> CreateDocumentQuery<T>(FeedOptions queryFeedOptions,
            Expression<Func<T, bool>> filterPredicate)
        {
            IDocumentQuery<T> documentsQueryable = _documentClient
                .CreateDocumentQuery<T>(_documentCollectionUri, queryFeedOptions).Where(filterPredicate).AsDocumentQuery();
            return documentsQueryable;
        }

        public IDocumentQuery<T> CreateDocumentQuery<T>(FeedOptions queryFeedOptions,
            string sqlQuery)
        {
            IDocumentQuery<T> documentsQueryable = _documentClient
                .CreateDocumentQuery<T>(_documentCollectionUri, sqlQuery, queryFeedOptions).AsDocumentQuery();
            return documentsQueryable;
        }

       

       

    }
}
