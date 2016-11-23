//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The analytics worker counter names.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PerformanceCounters
{
    /// <summary>
    /// The analytics worker counter names.
    /// </summary>
    public static class AnalyticsWorkerCounterNames
    {
        /// <summary>
        /// The total message count.
        /// </summary>
        public const string TotalMessageCount = "Total Message Count";

        /// <summary>
        /// The total task count.
        /// </summary>
        public const string TotalTaskCount = "Total Task Count";

        /// <summary>
        /// The total queue count.
        /// </summary>
        public const string TotalQueueCount = "Total Queue Count";

        /// <summary>
        /// The analytics worker failure rate.
        /// </summary>
        public const string ProcessingSuccessRate = "Processing Success Rate";

        /// <summary>
        /// The analytics worker failure rate.
        /// </summary>
        public const string FindParentEventSuccessRate = "Find Parent Event Success Rate";

        /// <summary>
        /// message processed per second.
        /// </summary>
        public const string MessageProcessedPerSec = "Message Processed Per Sec";

        /// <summary>
        /// The message process average time.
        /// </summary>
        public const string MessageProcessAvgTime = "Message Process Avg Time";

        /// <summary>
        /// The entity processed per sec.
        /// </summary>
        public const string EntityProcessedPerSec = "Entity Processed Per Sec";
    }
}