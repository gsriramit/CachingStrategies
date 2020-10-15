using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Documents;
using Polly;

namespace CacheStrategyImplementation.Factories
{
    public interface ICosmosFactory
    {
        IDocumentClient CreateCosmosConnection();
        IAsyncPolicy CreateResiliencyAsyncPolicy();
    }
}
