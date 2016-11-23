//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a Get MSS Voucher Distribution History API invocation.
    /// </summary>
    [DataContract]
    public class GetMssVoucherDistributionHistoryResponse : CommerceResponse
    {
        /// <summary>
        /// Gets or sets the remaining credit amount.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "amount_remaining")]
        public decimal AmountRemaining { get; set; }

        /// <summary>
        /// Gets or sets the distribution history
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "distribution_history")]
        public IEnumerable<DistributionHistoryDataContract> DistributionHistory { get; set; }

        /// <summary>
        /// Gets or sets the transaction history
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "transaction_history")]
        public IEnumerable<TransactionHistoryDataContract> TransactionHistory { get; set; }
    }
}