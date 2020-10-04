using System;
using System.Collections.Generic;
using System.Text;

namespace CacheStrategyImplementation.Configuration
{
    public class CosmosDbContext
    {
        public string CosmosStoreEndpointUri { get; set; }
        public string CosmosStoreAuthKey { get; set; }
        public string CosmosStoreDatabaseId { get; set; }
        public string CosmosStoreContainerId { get; set; }
        public int CosmosStoreMaxRetryWaitTime { get; set; }
        public int CosmosStoreMaxRetryAttempts { get; set; }

    }
}
