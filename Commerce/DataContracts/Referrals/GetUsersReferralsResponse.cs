//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to Get users referrals API invocation.
    /// </summary>
    [DataContract]
    public class GetUsersReferralsResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the list of referral codes and their usage counts per event.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "referral_code_reports")]
        public IEnumerable<ReferralCodeReportDataContract> ReferralCodeReports { get; set; }
    }
}