using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using System.Text;
using CacheStrategyImplementation.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace CacheStrategyImplementation.Factories
{
    public class CosmosFactory: ICosmosFactory
    {
        private readonly IConfiguration _appConfiguration;
        private IDocumentClient _documentClient;
        public CosmosFactory(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }
        
        public IDocumentClient CreateCosmosConnection()
        {
            CosmosDbContext cosmosStoreContext = this.GetCosmosDbContext();
            _documentClient = new DocumentClient(new Uri(cosmosStoreContext.CosmosStoreEndpointUri),
                cosmosStoreContext.CosmosStoreAuthKey,
                new ConnectionPolicy()
                {
                    RetryOptions = new RetryOptions()
                    {
                        MaxRetryAttemptsOnThrottledRequests = cosmosStoreContext.CosmosStoreMaxRetryAttempts
                    }
                });

            this.DocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(cosmosStoreContext.CosmosStoreDatabaseId,
                cosmosStoreContext.CosmosStoreContainerId);
            this.DatabaseUri = UriFactory.CreateDatabaseUri(cosmosStoreContext.CosmosStoreDatabaseId);

            return _documentClient;
        }

        public Uri DocumentCollectionUri { get; private set; }
        public Uri DatabaseUri { get; private set; }

        private CosmosDbContext GetCosmosDbContext()
        {
            return new CosmosDbContext
            {
                CosmosStoreEndpointUri = _appConfiguration["Store.EndpointUri"],
                CosmosStoreAuthKey = _appConfiguration["Store.Key"],
                CosmosStoreDatabaseId = _appConfiguration["Store.DatabaseId"],
                CosmosStoreContainerId = _appConfiguration["Store.ContainerId"],
                CosmosStoreMaxRetryWaitTime =
                    Convert.ToInt16(_appConfiguration[
                        "Store.ConnectionPolicy.MaxRetryWaitTimeInSeconds"]),
                CosmosStoreMaxRetryAttempts =
                    Convert.ToInt16(_appConfiguration[
                        "Store.ConnectionPolicy.MaxRetryAttemptsOnThrottledRequests"])
            };
        }
    }
}
