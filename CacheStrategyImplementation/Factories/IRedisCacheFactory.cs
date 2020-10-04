using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Factories
{
    public interface IRedisCacheFactory
    {
        Lazy<IConnectionMultiplexer> CreateRedisConnection();
    }
}
