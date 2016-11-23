//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to an Add referral type API invocation.
    /// </summary>
    [DataContract]
    public class AddReferralTypeResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the ReferralCode.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_code")]
        public string ReferralCode { get; set; }
    }
}