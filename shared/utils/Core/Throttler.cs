//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core
{
    using System;
    using System.Collections.Generic;

    // This class is thread-safe
    public static class Throttler
    {
        private static readonly Dictionary<string, int> _delays = new Dictionary<string, int>();
        private static readonly Dictionary<string, DateTime> _nextRequestTime = new Dictionary<string, DateTime>();

        public static void RegisterResource(string resourceName, int delayMilliseconds)
        {
            lock (_delays)
            {
                if (_delays.ContainsKey(resourceName))
                    _delays[resourceName] = delayMilliseconds;
                else
                    _delays.Add(resourceName, delayMilliseconds);
            }
        }

        public static int GetSleepIntervalFor(string resourceName)
        {
            lock (_nextRequestTime)
            {
                if (!_nextRequestTime.ContainsKey(resourceName))
                    _nextRequestTime.Add(resourceName, DateTime.Now);
                var nextRequestTime = _nextRequestTime[resourceName];
                _nextRequestTime[resourceName] = nextRequestTime + TimeSpan.FromMilliseconds(_delays[resourceName]);
                var delay = (int)(nextRequestTime - DateTime.Now).TotalMilliseconds;
                return delay < 0 ? 0 : delay;
            }
        }
    }
}