//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Get Earn History API invocation.
    /// </summary>
    [DataContract]
    public class GetEarnBurnTransactionHistoryResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the RedemptionHistoryItems object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "redemption_history")]
        public IEnumerable<EarnBurnTransactionItemDataContract> RedemptionHistory { get; set; }

        /// <summary>
        /// Gets or sets the credit balance avaialabe for a user to burn. Applicable only for the Earn/Burn program
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "credit_balance")]
        public string CreditBalance { get; set; }
    }
}