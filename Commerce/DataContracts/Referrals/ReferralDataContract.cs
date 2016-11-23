//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a referral.
    /// </summary>
    [DataContract]
    public class ReferralDataContract
    {
        /// <summary>
        /// Gets or sets the referral type code for which this referral is being made.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_type_code")]
        public string ReferralTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the event which triggered this referral.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_event")]
        public int ReferralEvent { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who was referred.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "user_id")]
        public string UserId { get; set; }
    }
}