//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Local Cache
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMoCache.CacheImplementations
{
    using System;
    using System.Web;
    using System.Web.Caching;

    /// <summary>
    /// The local cache.
    /// </summary>
    public sealed class LocalCache : ICache
    {
        #region Fields
        
        /// <summary>
        /// The only instance of the <see cref="LocalCache"/> class.
        /// </summary>
        private static readonly LocalCache LocalCacheInstance = new LocalCache();

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly Cache cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="LocalCache"/> class from being created.
        /// </summary>
        private LocalCache()
        {
            cache = HttpContext.Current.Cache;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of the <see cref="LocalCache"/> class.
        /// </summary>
        public static LocalCache Instance
        {
            get { return LocalCacheInstance; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves an object from the cache based on a string key.
        /// </summary>
        /// <param name="key">
        /// Cache key
        /// </param>
        /// <returns>
        /// The associated object, or none if no such key exists.
        /// </returns>
        public object Get(string key)
        {
            object val = cache[key];

            return val;
        }

        /// <summary>
        /// Retrieves an object from the cache based on a string key.
        /// </summary>
        /// <param name="key">
        /// Cache key
        /// </param>
        /// <param name="val">
        /// Object to store under the key.
        /// </param>
        /// <param name="cacheItemAbsoluteExpiration">Absolute expiration time for the cache item.Default is 10 minutes </param>
        public void Put(string key, object val, double? cacheItemAbsoluteExpiration = null)
        {
            var timeOut = cacheItemAbsoluteExpiration == null ? 10D : cacheItemAbsoluteExpiration.Value;
            cache.Insert(key, val, null, DateTime.Now.AddMinutes(timeOut), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        #endregion
    }
}