using CacheStrategyImplementation.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Authentication;
using System.Text;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        private readonly IConfiguration _configuration;
        public AppConfiguration(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        /// <summary>
        /// Configuration options needed to initialize connection with redis server
        /// </summary>
        /// <returns></returns>
        public ConfigurationOptions GetRedisConfigurationOptions()
        {

            var cacheName = _configuration["Cache.CacheName"];
            var cachePassword = _configuration["eCache.CachePassword"];
            var cacheConnectTimeout = Convert.ToInt16(_configuration["eCache.ConnectTimeout"]);
            var cacheSyncTimeout = Convert.ToInt16(_configuration["Cache.SyncTimeout"]);
            var cacheMaxConnectRetry = Convert.ToInt16(_configuration["Cache.MaxConnectRetry"]);
            var cacheKeepAlive = Convert.ToInt16(_configuration["Cache.KeepAlive"]);
            var cacheConnectRetryWaitTime = Convert.ToInt16(_configuration["Cache.RetryBackOffTime"]);
            var cacheConnectMaxRetryWaitTime = Convert.ToInt16(_configuration["Cache.RetryBackOffMaxTime"]);
            string tlsSetting = _configuration["Cache.TLSVersion"];
            //Check for any enforced version of SSL or TLS. If nothing is configured, set protocol to TLS 1.2
            bool isValidProtocol = Enum.TryParse<SslProtocols>(tlsSetting, out var cacheConnectionProtocol);


            var cacheConfiguration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                Ssl = true,
                SslProtocols = isValidProtocol ? cacheConnectionProtocol : SslProtocols.Tls12
            };

            cacheConfiguration.EndPoints.Add(cacheName);
            cacheConfiguration.Password = cachePassword;
            cacheConfiguration.SyncTimeout = cacheSyncTimeout;
            cacheConfiguration.ConnectRetry = cacheMaxConnectRetry;
            cacheConfiguration.ReconnectRetryPolicy = new ExponentialRetry(cacheConnectRetryWaitTime, cacheConnectMaxRetryWaitTime);
            cacheConfiguration.ConnectTimeout = cacheConnectTimeout;
            cacheConfiguration.KeepAlive = cacheKeepAlive;

            return cacheConfiguration;
        }

        /// <summary>
        /// Configuration options to initialize connection with cosmos data store
        /// </summary>
        /// <returns></returns>
        public CosmosDbContext GetCosmosDbContext()
        {
            return new CosmosDbContext
            {
                CosmosStoreEndpointUri = _configuration["Store.EndpointUri"],
                CosmosStoreAuthKey = _configuration["Store.Key"],
                CosmosStoreDatabaseId = _configuration["Store.DatabaseId"],
                CosmosStoreContainerId = _configuration["Store.ContainerId"],
                CosmosStoreMaxRetryWaitTime =
                Convert.ToInt16(_configuration[
                "Store.ConnectionPolicy.MaxRetryWaitTimeInSeconds"]),
                CosmosStoreMaxRetryAttempts =
                Convert.ToInt16(_configuration[
                "Store.ConnectionPolicy.MaxRetryAttemptsOnThrottledRequests"])
            };
        }

    }
}
