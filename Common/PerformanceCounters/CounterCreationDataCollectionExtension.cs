//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The counter creation data collection extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PerformanceCounters
{
    using System.Diagnostics;

    /// <summary>
    /// The counter creation data collection extension.
    /// </summary>
    public static class CounterCreationDataCollectionExtension
    {
        #region Methods

        /// <summary>
        /// The add average timer 32.
        /// </summary>
        /// <param name="counterCollection">
        /// The counter collection.
        /// </param>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="help">
        /// The help.
        /// </param>
        public static void AddAverageTimer32(this CounterCreationDataCollection counterCollection, string counterName, string help = "")
        {
            var counterCreationData = new CounterCreationData();
            counterCreationData.CounterName = counterName;
            counterCreationData.CounterHelp = help;
            counterCreationData.CounterType = PerformanceCounterType.AverageTimer32;
            counterCollection.Add(counterCreationData);

            // required base counter
            var averageTimer32Base = new CounterCreationData();
            averageTimer32Base.CounterType = PerformanceCounterType.AverageBase;
            averageTimer32Base.CounterName = counterName + "base";
            counterCollection.Add(averageTimer32Base);
        }

        /// <summary>
        /// The add raw fraction.
        /// </summary>
        /// <param name="counterCollection">
        /// The counter collection.
        /// </param>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="help">
        /// The help.
        /// </param>
        public static void AddRawFraction(
            this CounterCreationDataCollection counterCollection, string counterName, string help = "")
        {
            var counterCreationData = new CounterCreationData();
            counterCreationData.CounterName = counterName;
            counterCreationData.CounterHelp = help;
            counterCreationData.CounterType = PerformanceCounterType.RawFraction;
            counterCollection.Add(counterCreationData);

            // required base counter
            var rawFractionBase = new CounterCreationData();
            rawFractionBase.CounterType = PerformanceCounterType.RawBase;
            rawFractionBase.CounterName = counterName + "Base";
            counterCollection.Add(rawFractionBase);
        }

        /// <summary>
        /// The add number of items 32.
        /// </summary>
        /// <param name="counterCollection">
        /// The counter collection.
        /// </param>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="help">
        /// The help.
        /// </param>
        public static void AddNumberOfItems32(this CounterCreationDataCollection counterCollection, string counterName, string help = "")
        {
            var counterCreationData = new CounterCreationData();
            counterCreationData.CounterName = counterName;
            counterCreationData.CounterHelp = help;
            counterCreationData.CounterType = PerformanceCounterType.NumberOfItems32;
            counterCollection.Add(counterCreationData);
        }

        /// <summary>
        /// The add rate of counts per second 32.
        /// </summary>
        /// <param name="counterCollection">
        /// The counter collection.
        /// </param>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="help">
        /// The help.
        /// </param>
        public static void AddRateOfCountsPerSecond32(this CounterCreationDataCollection counterCollection, string counterName, string help = "")
        {
            var counterCreationData = new CounterCreationData();
            counterCreationData.CounterName = counterName;
            counterCreationData.CounterHelp = help;
            counterCreationData.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
            counterCollection.Add(counterCreationData);
        }

        #endregion Methods
    }
}