//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Caching;
using System.Diagnostics;

namespace Earn.Dashboard.Web.Models
{
    public class AuthorizedUsersCache
    {
        private const int CacheItemLifeTime = 10; //min
        private static readonly Lazy<AuthorizedUsersCache> instance = new Lazy<AuthorizedUsersCache>(() => new AuthorizedUsersCache());
        private readonly ObjectCache cache = MemoryCache.Default;

        private AuthorizedUsersCache() { }

        private CacheItemPolicy cacheItemPolicy
        {
            get
            {
                var policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(CacheItemLifeTime)
                };

                return policy;
            }
        }

        public static AuthorizedUsersCache Instance { get { return instance.Value; } }

        public object this[string key]
        {
            get
            {
                Debug.WriteLine(string.Format("get from Cache: key: {0}, value: {1}", key, cache[key]));
                return cache[key];
            }
        }

        public void AddToCache(string key, object value)
        {
            Debug.WriteLine(string.Format("add to Cache: ket: {0}, value: {1}", key, value));
            var policy = cacheItemPolicy;
            cache.Set(key, value, cacheItemPolicy);
        }

        public bool Contains(string key)
        {
            Debug.WriteLine("contains " + key);
            return cache.Contains(key);
        }

        public void Remove(string key)
        {
            Debug.WriteLine("remove from cache " + key);
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }
    }
}