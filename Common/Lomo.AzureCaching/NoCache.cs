//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoMoCache.CacheImplementations;

namespace Lomo.AzureCaching
{
    public class NoCache : ICache
    {
        public object Get(string key)
        {
            return null;
        }

        public void Put(string key, object val, double? cacheItemAbsoluteExpiration = null)
        {
            //do nothing
        }
    }
}