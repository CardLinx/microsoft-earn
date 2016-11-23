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
    /// Represents a service referral type.
    /// </summary>
    [DataContract]
    public class ServiceReferralTypeDataContract : ReferralTypeDataContract
    {
        /// <summary>
        /// Gets or sets the ID of the entity making this referral type.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referrer_id")]
        public Guid ReferrerId { get; set; }

        /// <summary>
        /// Gets or sets the type of entity making this referral type.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referrer_type")]
        public string ReferrerType { get; set; }
    }
}