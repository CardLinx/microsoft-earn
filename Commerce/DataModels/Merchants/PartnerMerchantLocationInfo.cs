//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    /// <summary>
    /// Represents partner information for a merchant location.
    /// </summary>
    public class PartnerMerchantLocationInfo
    {
        /// <summary>
        /// Initializes a new instance of the PartnerMerchantLocationInfo class.
        /// </summary>
        public PartnerMerchantLocationInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class derived from PartnerMerchantLocationInfo, using the fields from the specified other
        /// PartnerMerchantLocationInfo.
        /// </summary>
        /// <param name="partnerMerchantLocationInfo">
        /// The other PartnerMerchantLocationInfo whose fields to copy.
        /// </param>
        internal PartnerMerchantLocationInfo(PartnerMerchantLocationInfo partnerMerchantLocationInfo)
        {
            PartnerMerchantId = partnerMerchantLocationInfo.PartnerMerchantId;
            PartnerMerchantIdType = partnerMerchantLocationInfo.PartnerMerchantIdType;
            MerchantTimeZoneId = partnerMerchantLocationInfo.MerchantTimeZoneId;
        }

        /// <summary>
        /// Determines whether the specified object has equal values to this object in all fields.
        /// </summary>
        /// <param name="obj">
        /// The object whose values to compare.
        /// </param>
        /// <returns>
        /// True if the two objects have the same values.
        /// </returns>
        public override bool Equals(object obj)
        {
            PartnerMerchantLocationInfo partnerMerchantLocationInfo = (PartnerMerchantLocationInfo)obj;
            return PartnerMerchantId == partnerMerchantLocationInfo.PartnerMerchantId &&
                   PartnerMerchantIdType == partnerMerchantLocationInfo.PartnerMerchantIdType &&
                   MerchantTimeZoneId == partnerMerchantLocationInfo.MerchantTimeZoneId;
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        /// <remarks>
        /// * CA2218:
        ///   * If two objects are equal in value based on the Equals override, they must both return the same value for calls
        ///     to GetHashCode.
        ///   * GetHashCode must be overridden whenever Equals is overridden.
        /// * It is fine if the value overflows.
        /// </remarks>
        public override int GetHashCode()
        {
            int result = PartnerMerchantIdType.GetHashCode();

            if (PartnerMerchantId != null)
            {
                result += PartnerMerchantId.GetHashCode();
            }

            if (MerchantTimeZoneId != null)
            {
                result += MerchantTimeZoneId.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the merchant ID.
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets the type of the partner merchant ID.
        /// </summary>
        public PartnerMerchantIdType PartnerMerchantIdType { get; set; }

        /// <summary>
        /// Gets or sets the time zone id of the partner merchant.
        /// </summary>
        public string MerchantTimeZoneId { get; set; }
    }
}