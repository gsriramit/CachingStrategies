using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CacheStrategyImplementation.Configuration;
using CacheStrategyImplementation.Factories;
using Microsoft.Azure.Cosmos;
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
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public SqlCosmosRepository(ICosmosFactory cosmosFactory)
        {
            _documentClient = cosmosFactory.CreateCosmosConnection();
            _resiliencyPolicy = cosmosFactory.CreateResiliencyAsyncPolicy();
            _cosmosClient = cosmosFactory.CreateCosmosClient();
            _container = _cosmosClient.GetContainer(cosmosFactory.getDatabaseID(), cosmosFactory.getContainerID());

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

        public async Task<T> ReadItemAsync<T>(string partitionKey)
        {
            //ContainerResponse container = await  _cosmosClient.GetDatabase(_databaseID).CreateContainerIfNotExistsAsync(_containerID, "/country");
            //Container container = _cosmosClient.GetContainer(_databaseID, _containerID);
            ItemResponse<T> entityResponse = await _container.ReadItemAsync<T>(partitionKey, new Microsoft.Azure.Cosmos.PartitionKey(partitionKey));
            return entityResponse;

        }

        public async Task<bool> CreateItemAsync<T>(string partitionKey, T entity)
        {
            // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<T> itemResponse = await _container.CreateItemAsync<T>(entity, new Microsoft.Azure.Cosmos.PartitionKey(entity.GetType().GetProperty("country").ToString()));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.Created;
        }


    }
}
