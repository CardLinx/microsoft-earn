//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a partner merchant and its number of redemptions.
    /// </summary>
    [DataContract]
    public class PartnerMerchantDealRedemptions
    {
        /// <summary>
        /// Gets or sets the Partner from which the associated partner merchant ID originated.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partner")]
        public string Partner { get; set; }

        /// <summary>
        /// Gets or sets the partner merchant ID whose number of redemptions is reported in this object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partner_merchant_id")]
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets the number of redemptions reported for this partner merchant.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "number_of_redemptions")]
        public int NumberOfRedemptions { get; set; }
    }
}