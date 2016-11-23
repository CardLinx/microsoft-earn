//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Voucher Info Model
    /// </summary>
    public class DealPreviewVoucherInfoModel
    {
        /// <summary>
        /// Gets or sets voucher value
        /// </summary>
        [JsonProperty(PropertyName = "voucher_value")]
        public float? DiscountValue { get; set; }

        /// <summary>
        /// Gets or sets discount percentage
        /// </summary>
        [JsonProperty(PropertyName = "voucher_discount_percent")]
        public float? DiscountPercentage { get; set; }
    }
}