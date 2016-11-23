//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Azure Cache. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using LoMoCache.CacheImplementations;

namespace Lomo.AzureCaching
{
    using System;
    using Microsoft.WindowsAzure;
    using Microsoft.ApplicationServer.Caching;
    using Microsoft.Azure;

    /// <summary>
    /// The azure cache.
    /// </summary>
    public class AzureCache : ICache
    {
        #region Fields

        /// <summary>
        /// The azure cache.
        /// </summary>
        private readonly DataCache _azureCache;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCache"/> class.
        /// </summary>
        public AzureCache()
        {
            // Create a DataCacheFactoryConfiguration object
            var config = new DataCacheFactoryConfiguration();
            var cacheEndPoint = CloudConfigurationManager.GetSetting("CacheEndPoint");
            var cacheSecurityKey = CloudConfigurationManager.GetSetting("CacheSecurityKey");

            // Enable the AutoDiscoveryProperty
            config.AutoDiscoverProperty = new DataCacheAutoDiscoverProperty(true, cacheEndPoint);

            // Configure the access key and sslEnabled setting.
            config.SecurityProperties = new DataCacheSecurity(cacheSecurityKey, false);

            bool localCacheEnabled;
            if (!bool.TryParse(CloudConfigurationManager.GetSetting("LocalCacheEnabled"), out localCacheEnabled))
            {
                localCacheEnabled = true;
            }

            // Configure the Local Cache if its enabled
            if (localCacheEnabled)
            {
                int localCacheItemTimeout;
                int localCacheObjectCount;
                if (!int.TryParse(CloudConfigurationManager.GetSetting("LocalCacheTimeoutMinutes"), out localCacheItemTimeout))
                {
                    localCacheItemTimeout = 10;
                }
                if (!int.TryParse(CloudConfigurationManager.GetSetting("LocalCacheObjectCount"), out localCacheObjectCount))
                {
                    localCacheObjectCount = 500000;
                }

                config.LocalCacheProperties = new DataCacheLocalCacheProperties(localCacheObjectCount, TimeSpan.FromMinutes(localCacheItemTimeout), DataCacheLocalCacheInvalidationPolicy.TimeoutBased);
            }

            // Create a DataCacheFactory object with the configuration settings:
            var factory = new DataCacheFactory(config);

            var cacheName = CloudConfigurationManager.GetSetting("CacheName");
            _azureCache = string.IsNullOrEmpty(cacheName) ? factory.GetDefaultCache() : factory.GetCache(cacheName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves an object from the cache based on a string key.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>The associated object, or none if no such key exists.</returns>
        public object Get(string key)
        {
            return _azureCache.Get(key);
        }

        /// <summary>
        /// Retrieves an object from the cache based on a string key.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <param name="val">Object to store under the key.</param>
        /// <param name="cacheItemAbsoluteExpiration">cache timeout in minutes. If null uses default cache settings</param>
        public void Put(string key, object val, double? cacheItemAbsoluteExpiration = null)
        {
            if (cacheItemAbsoluteExpiration == null)
            {
                _azureCache.Put(key, val);
            }
            else
            {
                _azureCache.Put(key, val, TimeSpan.FromMinutes(cacheItemAbsoluteExpiration.Value));
            }
        }
        #endregion
    }
}