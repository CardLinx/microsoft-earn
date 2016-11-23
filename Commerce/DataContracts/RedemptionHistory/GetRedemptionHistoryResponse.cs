//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Get Redemption History API invocation.
    /// </summary>
    [DataContract]
    public class GetRedemptionHistoryResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the RedemptionHistoryItems object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "redemption_history")]
        public IEnumerable<RedemptionHistoryItemDataContract> RedemptionHistory { get; set; }
    }
}