using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using System.Text;
using CacheStrategyImplementation.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Timeout;

namespace CacheStrategyImplementation.Factories
{
    public class CosmosFactory: ICosmosFactory
    {
        private readonly IConfiguration _appConfiguration;
        private IDocumentClient _documentClient;
        private Uri _documentCollectionUri;
        private Uri _databaseUri;

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
                    }, RequestTimeout = cosmosStoreContext.OperationTimeOut
                });

            _documentCollectionUri = cosmosStoreContext.GetDocumentCollectionUri();
            _databaseUri = cosmosStoreContext.GetDatabaseUri();

            return _documentClient;
        }

        public IAsyncPolicy CreateResiliencyAsyncPolicy()
        {
            return GetTimeOutCircuitBreakerResiliencyPolicy(
                int.Parse(_appConfiguration["OperationTimeOutInSeconds"]),
                int.Parse(_appConfiguration["CircuitBreakerThresholdExceptions"]),
                int.Parse(_appConfiguration["CircuitBreakerOpenDurationInSeconds"]),
                TimeoutStrategy.Pessimistic);
        }

        private IAsyncPolicy GetTimeOutCircuitBreakerResiliencyPolicy(int operationTimeoutInSec,
            int circuitBreakerThresholdExceptions, int circuitBreakerOpenDurationInSeconds, TimeoutStrategy timeoutStrategy)
        {

            var overallOperationTimeOutSpan = TimeSpan.FromSeconds(operationTimeoutInSec);


            var asyncBreaker = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: circuitBreakerThresholdExceptions,
                    durationOfBreak: TimeSpan.FromSeconds(circuitBreakerOpenDurationInSeconds)
                );

            var asyncPerCallTimeout =
                Policy.TimeoutAsync(overallOperationTimeOutSpan, timeoutStrategy);


            return Policy.WrapAsync(asyncBreaker, asyncPerCallTimeout);

        }

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
                        "Store.ConnectionPolicy.MaxRetryAttemptsOnThrottledRequests"]),
                OperationTimeOut = TimeSpan.FromSeconds(
                    int.Parse(_appConfiguration["OperationTimeOutInSeconds"])),

            };
        }

    }
}
