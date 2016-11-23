//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the list of referral codes and their usage counts per event.
    /// </summary>
    [DataContract]
    public class ReferralCodeReportDataContract
    {
        /// <summary>
        /// Gets or sets the referral code whose event counts appear in this object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_code")]
        public string ReferralCode { get; set; }

        /// <summary>
        /// Gets or sets the list of referral events and their counts.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_event_counts")]
        public IEnumerable<ReferralCountsDataContract> ReferralEventCounts { get; set; }
    }
}