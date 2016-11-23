//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a partner merchant ID and its type.
    /// </summary>
    [DataContract]
    public class ComplexPartnerMerchantId
    {
        /// <summary>
        /// Gets or sets the merchant ID.
        /// </summary>
        /// <remarks>
        /// Used in conjunction with the acquirer ICA to identify the merchant during authorization operations.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the merchant ID type.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id_type")]
        public string IdType { get; set; }

        /// <summary>
        /// Gets or sets the time zone id for specific merchant.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "time_zone")]
        public string TimeZoneId { get; set; }
    }
}