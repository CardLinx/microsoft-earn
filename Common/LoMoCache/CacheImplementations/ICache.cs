//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Interface for Cache Implementations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMoCache.CacheImplementations
{
    /// <summary>
    /// Defines the behavior of the cache implementations used by the 
    /// cache-based lookup client.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Retrieves an object from the cache based on a string key.
        /// </summary>
        /// <param name="key">
        /// Cache key
        /// </param>
        /// <returns>
        /// The associated object, or none if no such key exists.
        /// </returns>
        object Get(string key);

        /// <summary>
        /// Adds an object to the cache based on a string key.
        /// </summary>
        /// <param name="key">
        /// Cache key
        /// </param>
        /// <param name="val">
        /// Object to store under the key.
        /// </param>
        /// <param name="cacheItemAbsoluteExpiration">Absolute expiration time for the cache item.Default is 10 minutes </param>
        void Put(string key, object val, double? cacheItemAbsoluteExpiration = null);
    }
}