//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// //   Contract for Transactions per Merchant Store
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.DataContracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Contract for Transactions per Merchant Store
    /// </summary>
    public class MerchantStoreTransactionContract
    {
        /// <summary>
        ///  Gets or sets the merchant name.
        /// </summary>
        [JsonProperty(PropertyName = "merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the merchant store location.
        /// </summary>
        [JsonProperty(PropertyName = "store_location")]
        public MerchantLocationContract StoreLocation { get; set; }

        /// <summary>
        /// Gets or sets the transactions in this store.
        /// </summary>
        [JsonProperty(PropertyName = "transactions")]
        public MerchantTransactionContract[] Transactions { get; set; }
    }
}