//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // This class represents a counter
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace PerformanceCounters
{
    using System.Diagnostics;

    /// <summary>
    /// This class represents a counter. Ideally this class should be named as "Counter". But there's already a class with the same name
    /// Renaming it would require changes to a lot of place in DealsServer where it's being used now
    /// </summary>
    public class CounterInfo
    {
        /// <summary>
        /// Name of the counter
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Type of the counter being created
        /// </summary>
        public PerformanceCounterType Type { get; internal set; }

        /// <summary>
        /// Descriptive text for the counter
        /// </summary>
        public string Description { get; internal set; }
    }
}