//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Captures the day-time restrictions of a deal/discount 
    /// Day enumeration starts on Sunday with value 1 and ends on Saturday with value 7
    /// Example: DayAndTime for Sunday midnight (beginning of the day) = 10000, and for last minute of Sunday = 12359
    /// </summary>
    [DataContract]
    public class DayTimeRestriction
    {
        /// <summary>
        /// Gets or sets the week day and time when discount will start in integer format. Example: Friday at 18:30 -> 61830
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "start_day_and_time")]
        public int StartDayAndTime { get; set; }

        /// <summary>
        /// Gets or sets the week day and time when discount will end in integer format. Example: Sunday at 00:30 -> 10030
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "end_day_and_time")]
        public int EndDayAndTime { get; set; }
    }
}