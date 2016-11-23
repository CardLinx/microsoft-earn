//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
// Defines the cargo for reporting partner transaction
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the cargo for reporting partner transaction
    /// </summary>
    [DataContract]
    public class PartnerTransactionReportingCargo
    {
        /// <summary>
        /// Gets or sets the Transaction Date for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_reference")]
        public string TransactionReference { get; set; }

        /// <summary>
        /// Gets or sets the Transaction Date for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_date")]
        public string TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the Deal Id for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "deal_id")]
        public string DealId { get; set; }

        /// <summary>
        /// Gets or sets the Settlement Amount for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "transaction_amount")]
        public string TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the Settlement Amount for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Deal Id: {0}; Transaction Reference: {1}", this.DealId, this.TransactionReference);
        }
    }
}