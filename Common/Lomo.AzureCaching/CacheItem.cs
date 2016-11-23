//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using LoMoCache.CacheImplementations;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Lomo.AzureCaching
{
    public class CacheItem
    {
        private static ICache Cache;

        private static ICache DefaultCache
        {
            get
            {
                if (Cache == null)
                {
                    bool cacheEnabled;
                    bool.TryParse(CloudConfigurationManager.GetSetting("CacheEnabled"), out cacheEnabled);
                    if (cacheEnabled)
                    {
                        if (RoleEnvironment.IsAvailable)
                        {
                            Cache = new AzureCache();
                        }
                        else
                        {
                            Cache = LocalCache.Instance;
                        }
                    }
                    else
                    {
                        Cache = new NoCache();
                    }
                }
                return Cache;
            }
        }

        public static T Get<T>(string key, Closure<T> closure)
        {
            var itemValue = default(T);
            var cache = DefaultCache;
            if (cache != null)
            {
                var cacheValue = cache.Get(key);
                if (cacheValue == null)
                {
                    itemValue = closure();
                    if (itemValue != null)
                    {
                        cache.Put(key, itemValue);
                    }
                }
                else
                {
                    itemValue = (T) cacheValue;
                }
            }
            return itemValue;
        }
    }
}