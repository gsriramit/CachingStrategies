using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Text;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace CacheStrategyImplementation.Factories
{
    public class RedisCacheFactory: IRedisCacheFactory
    {
        private readonly IConfiguration _appConfiguration;        
        private Lazy<ConnectionMultiplexer> _cacheConnection;
        
        public RedisCacheFactory(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            ConfigurationOptions cacheConfiguration = GetRedisConfigurationOptions();
            _cacheConnection = new Lazy<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(cacheConfiguration));
        }

        public ConnectionMultiplexer CreateRedisConnection()
        {
            return _cacheConnection.Value;
        }      


        /// <summary>
        /// Configuration options needed to initialize connection with redis server
        /// </summary>
        /// <returns></returns>
        private ConfigurationOptions GetRedisConfigurationOptions()
        {

            var cacheName = _appConfiguration["Cache.CacheName"];
            var cachePassword = _appConfiguration["Cache.CachePassword"];
            var cacheConnectTimeout = Convert.ToInt16(_appConfiguration["Cache.ConnectTimeout"]);
            var cacheSyncTimeout = Convert.ToInt16(_appConfiguration["Cache.SyncTimeout"]);
            var cacheMaxConnectRetry = Convert.ToInt16(_appConfiguration["Cache.MaxConnectRetry"]);
            var cacheKeepAlive = Convert.ToInt16(_appConfiguration["Cache.KeepAlive"]);
            var cacheConnectRetryWaitTime = Convert.ToInt16(_appConfiguration["Cache.RetryBackOffTime"]);
            var cacheConnectMaxRetryWaitTime = Convert.ToInt16(_appConfiguration["Cache.RetryBackOffMaxTime"]);
            string cacheTlsSetting = _appConfiguration["Cache.TLSVersion"];
            //Check for any enforced version of SSL or TLS. If nothing is configured, set protocol to TLS 1.2
            bool cacheIsValidProtocol = Enum.TryParse<SslProtocols>(cacheTlsSetting, out var cacheConnectionProtocol);


            var cacheConfiguration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                Ssl = true,
                SslProtocols = cacheIsValidProtocol ? cacheConnectionProtocol : SslProtocols.Tls12
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

    }
}
