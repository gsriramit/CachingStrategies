using System;
using System.Collections.Generic;
using System.Text;
using CacheStrategyImplementation.Configuration;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Contracts
{
    public interface IAppConfiguration
    {
        CosmosDbContext GetCosmosDbContext();
        ConfigurationOptions GetRedisConfigurationOptions();
    }
}
