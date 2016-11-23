//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using Newtonsoft.Json;

    /// <summary>
    /// Auth email generation content data
    /// </summary>
    public class AuthSmsNotificationData
    {
        /// <summary>
        /// Gets or sets merchant name
        /// </summary>
        [JsonProperty(PropertyName = "merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets discount sumary
        /// </summary>
        [JsonProperty(PropertyName = "discount_summary")]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the percent of the discount offered within the deal.
        /// </summary>
        [JsonProperty(PropertyName = "percent")]
        public float Percent { get; set; }

        /// <summary>
        /// Get or sets credit amount
        /// </summary>
        [JsonProperty(PropertyName = "credit_amount")]
        public string CreditAmount { get; set; }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>
        /// string representation of the data
        /// </returns>
        public override string ToString()
        {
            return string.Format("Merchantname : {0} \r\n" +
                                 "Discount Summary: {1} \r\n",
                                 MerchantName, DiscountSummary);
        }
    }
}