using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Polly;

namespace CacheStrategyImplementation.Factories
{
    public interface ICosmosFactory
    {
        IAsyncPolicy CreateResiliencyAsyncPolicy();
        CosmosClient CreateCosmosClient();
        string getDatabaseID();
        string getContainerID();
    }
}
