//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Transaction Detail for the transaction to be sent downstream.
    /// </summary>
    [DataContract]
    public class TransactionDetail
    {
        /// <summary>
        /// Gets or sets the Transaction Date for the transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "transaction_date")]
        public string TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the Discount Id for the transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_id")]
        public string DiscountId { get; set; }

        /// <summary>
        /// Gets or sets the Deal Id for the transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "deal_id")]
        public string DealId { get; set; }

        /// <summary>
        /// Gets or sets the Settlement Amount for the transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "settlement_amount")]
        public int SettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the Discount Amount for the transaction.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_amount")]
        public int DiscountAmount { get; set; }
    }
}