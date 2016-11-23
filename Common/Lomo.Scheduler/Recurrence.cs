//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the recurrence information for a scheduled job
    /// </summary>
    public class Recurrence : IEquatable<Recurrence>, IComparable<Recurrence>, IComparable
    {
        /// <summary>
        /// second, minute, hour, day, none (no recurrence)
        /// </summary>
        [JsonProperty("unit", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(RecurrenceFrequency.None)]
        public RecurrenceFrequency Frequency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interval
        /// </summary>
        [JsonProperty("interval")]
        public int Interval
        {
            get
            {
                return this.interval.GetValueOrDefault(0);
            }

            set
            {
                this.interval = value;
            }
        }

        /// <summary>
        /// Number of times the job is supposed to run
        /// If not specified, we presume job will run indefinitely
        /// </summary>
        [JsonProperty("count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the current <see cref="Recurrence"/> object into an equivalent <see cref="TimeSpan"/> based on
        /// the <see cref="Frequency"/> and <see cref="Interval"/> values.
        /// </summary>
        /// <remarks>
        /// The <see cref="TimeSpan"/> is calculated by adding the current <see cref="Interval"/> value to
        /// <see cref="DateTime.MinValue"/> and computing the difference. For this reason, it is important
        /// not to make assumptions about leap years or daylight savings time when using the resulting value.
        /// </remarks>
        public TimeSpan ToTimeSpan()
        {
            DateTime epoch = DateTime.MinValue;
            switch (this.Frequency)
            {
                case RecurrenceFrequency.Second:
                    return epoch.AddSeconds(this.Interval).Subtract(epoch);
                case RecurrenceFrequency.Minute:
                    return epoch.AddMinutes(this.Interval).Subtract(epoch);
                case RecurrenceFrequency.Hour:
                    return epoch.AddHours(this.Interval).Subtract(epoch);
                case RecurrenceFrequency.Day:
                    return epoch.AddDays(this.Interval).Subtract(epoch);
                default:
                    return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Recurrence Validation
        /// </summary>
        /// <param name="validationError">
        /// Error Message
        /// </param>
        /// <returns>
        /// true/false indicating whether validation succeeded
        /// </returns>
        internal bool Validate(out string validationError)
        {
            if (Interval < 0)
            {
                validationError = "Interval Must be Positive";
                return false;
            }

            if (Count < 0)
            {
                validationError = "Count Must be Positive";
                return false;
            }

            validationError = null;
            return true;
        }

        /// <summary>
        /// Equals implementation - Recurrence vs another Recurrence
        /// </summary>
        /// <param name="other">
        /// Other Recurrence to equate to
        /// </param>
        /// <returns>
        /// true/false indicating equality
        /// </returns>
        public bool Equals(Recurrence other)
        {
            return
                other != null &&
                other.Interval == this.Interval &&
                other.Count == this.Count &&
                other.Frequency == this.Frequency;
        }

        /// <summary>
        /// Equals implementation - Recurrence vs Object
        /// </summary>
        /// <param name="other">
        /// Other object to equate to
        /// </param>
        /// <returns>
        /// true/false indicating equality
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Recurrence);
        }

        /// <summary>
        /// Hashcode implementation
        /// </summary>
        /// <returns>
        /// integer result
        /// </returns>
        public override int GetHashCode()
        {
            // overflow is fine, just wrap
            unchecked
            {
                int hash = 17;
                hash = (hash * 29) + this.Interval.GetHashCode();
                hash = (hash * 29) + this.Count.GetHashCode();
                hash = (hash * 29) + this.Frequency.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Comparing one Recurrence to another Recurrence
        /// </summary>
        /// <param name="other">
        /// Other Recurrence to compare to
        /// </param>
        /// <returns>
        /// 0,1 or -1 for equal, greater or less
        /// </returns>
        public int CompareTo(Recurrence other)
        {
            return this.ToTimeSpan().CompareTo(other != null ? other.ToTimeSpan() : TimeSpan.Zero);
        }

        /// <summary>
        /// Comparing one Recurrence to another onject
        /// </summary>
        /// <param name="other">
        /// Other oject to compare to
        /// </param>
        /// <returns>
        /// returns the underlying compare to result
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Recurrence);
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>
        /// String representation of Recurrence
        /// </returns>
        public override string ToString()
        {
            return string.Format("Frequency : {0} \r\n" +
                                 "Count : {1} \r\n" +
                                 "Interva : {2}", Frequency.ToString(), Count, Interval);
        }

        /// <summary>
        /// Interval apart when job has to run
        /// </summary>
        int? interval;
    }
}