using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Documents;

namespace CacheStrategyImplementation.Factories
{
    public interface ICosmosFactory
    {
        IDocumentClient CreateCosmosConnection();
        Uri DocumentCollectionUri { get; }
        Uri DatabaseUri { get; }
    }
}
