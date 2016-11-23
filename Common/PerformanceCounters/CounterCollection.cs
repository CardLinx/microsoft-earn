//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The counter collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// The counter collection.
    /// </summary>
    public class CounterCollection
    {
        #region Fields

        /// <summary>
        /// The list.
        /// </summary>
        private readonly Dictionary<string, CounterValues> list = new Dictionary<string, CounterValues>();

        #endregion Fields

        #region Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        public void Add(string counterName)
        {
            list.Add(counterName, new CounterValues(counterName));
        }

        /// <summary>
        /// The get average value.
        /// </summary>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetAverageValue(string counterName)
        {
            CounterValues cv = list[counterName];
            return cv.AverageValue;
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public float GetValue(string counterName)
        {
            CounterValues cv = list[counterName];
            return cv.GetValue();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            foreach (CounterValues pc in list.Values)
            {
                pc.Initialize();
            }
        }

        /// <summary>
        /// The start sampling.
        /// </summary>
        /// <param name="totalSamples">
        /// The total samples.
        /// </param>
        /// <param name="readsPerSample">
        /// The reads per sample.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public void StartSampling(int totalSamples, int readsPerSample, Action<Dictionary<string, CounterValues>> action)
        {
            int delay = 1000 / readsPerSample;

            for (int i = 0; i < totalSamples; i++)
            {
                for (int j = 0; j < readsPerSample; j++)
                {
                    this.TakeSamples();
                    System.Threading.Thread.Sleep(delay);
                }

                action(this.list);
            }
        }

        /// <summary>
        /// The take samples.
        /// </summary>
        public void TakeSamples()
        {
            foreach (CounterValues pc in list.Values)
            {
                pc.TakeSample();
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// The counter values.
    /// </summary>
    public class CounterValues
    {
        #region Fields

        /// <summary>
        /// The average value.
        /// </summary>
        double averageValue;

        /// <summary>
        /// The counter.
        /// </summary>
        readonly PerformanceCounter counter;

        /// <summary>
        /// The counter name.
        /// </summary>
        private string counterName;

        /// <summary>
        /// The new sample.
        /// </summary>
        CounterSample newSample;

        /// <summary>
        /// The old sample.
        /// </summary>
        CounterSample oldSample;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterValues"/> class.
        /// </summary>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        public CounterValues(string counterName)
        {
            this.counterName = counterName;
            counter = Counter.GetCounter(counterName, true);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the average value.
        /// </summary>
        public double AverageValue
        {
            get { return averageValue; }
        }

        /// <summary>
        /// Gets or sets the counter name.
        /// </summary>
        public string CounterName
        {
            get { return counterName; }
            set { counterName = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The get value.
        /// </summary>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        internal float GetValue()
        {
            return CounterSampleCalculator.ComputeCounterValue(this.oldSample, this.newSample);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        internal void Initialize()
        {
            newSample = counter.NextSample();
            oldSample = newSample;
            averageValue = CounterSampleCalculator.ComputeCounterValue(newSample);
        }

        /// <summary>
        /// The take sample.
        /// </summary>
        internal void TakeSample()
        {
            oldSample = newSample;
            newSample = counter.NextSample();
            averageValue = (averageValue + GetValue()) / 2.0;
        }

        #endregion Methods
    }
}