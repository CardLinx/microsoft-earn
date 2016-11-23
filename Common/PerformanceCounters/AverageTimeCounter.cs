//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The average time counter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PerformanceCounters
{
    using System.Diagnostics;

    /// <summary>
    /// The average time counter.
    /// </summary>
    public class AverageTimeCounter
    {
        #region Fields

        /// <summary>
        /// The average.
        /// </summary>
        private readonly PerformanceCounter average;

        /// <summary>
        /// The base counter.
        /// </summary>
        private readonly PerformanceCounter baseCounter;

        /// <summary>
        /// The stopwatch.
        /// </summary>
        private readonly Stopwatch stopwatch;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageTimeCounter"/> class.
        /// </summary>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="readOnly">
        /// The read only.
        /// </param>
        public AverageTimeCounter(string counterName, bool readOnly)
        {
            average = Counter.GetCounter(counterName, readOnly);
            baseCounter = Counter.GetCounter(counterName + "base", readOnly);

            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// The update time.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        public void UpdateTime(int count = 1)
        {
            average.IncrementBy(stopwatch.ElapsedTicks);
            baseCounter.IncrementBy(count);
        }

        #endregion Methods
    }
}