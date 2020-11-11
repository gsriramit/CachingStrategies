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
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace CacheStrategyImplementation.Repos
{
    public class SqlCosmosRepository: ICosmosRepository
    {        
        private readonly IAsyncPolicy _resiliencyPolicy;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public SqlCosmosRepository(ICosmosFactory cosmosFactory)
        {
            _resiliencyPolicy = cosmosFactory.CreateResiliencyAsyncPolicy();
            _cosmosClient = cosmosFactory.CreateCosmosClient();
            _container = _cosmosClient.GetContainer(cosmosFactory.getDatabaseID(), cosmosFactory.getContainerID());            
        }

        public async Task<T> ReadItemAsync<T>(string documentId, string partitionKey)
        {
            ItemResponse<T> entityResponse = await _container.ReadItemAsync<T>(documentId, new Microsoft.Azure.Cosmos.PartitionKey(partitionKey));
            return entityResponse;

        }

        public async Task<bool> CreateItemAsync<T>(string documentId, T entity)
        {
            // Create an item in the container representing the Andersen family. 
            // Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<T> itemResponse = await _container.CreateItemAsync(
                entity);
            return itemResponse.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public async Task<bool> UpsertItemAsync<T>(string documentId, T entity)
        {
            // Create an item in the container representing the Andersen family. 
            // Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<T> itemResponse = await _container.UpsertItemAsync(
                entity);
            return itemResponse.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public async Task<bool> ReplaceItemAsync<T>(string documentId, T entity)
        {
            // Create an item in the container representing the Andersen family. 
            // Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<T> itemResponse = await _container.ReplaceItemAsync<T>(
                entity, documentId);
            return itemResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> DeleteItemAsync<T>(string documentId, PartitionKey partitionKey)
        {
            // Create an item in the container representing the Andersen family. 
            // Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<T> itemResponse = await _container.DeleteItemAsync<T>(
                documentId, partitionKey);
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}
