//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    /// <summary>
    /// Represents a partner card stored in the data store.
    /// </summary>
    public class PartnerCardInfo
    {
        /// <summary>
        /// Initializes a new instance of the PartnerCardInfo.
        /// </summary>
        public PartnerCardInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class derived from PartnerCardInfo, using the fields from the specified other
        /// PartnerCardInfo.
        /// </summary>
        /// <param name="partnerCardInfo">
        /// The other PartnerCardInfo whose fields to copy.
        /// </param>
        internal PartnerCardInfo(PartnerCardInfo partnerCardInfo)
        {
            PartnerId = partnerCardInfo.PartnerId;
            PartnerCardId = partnerCardInfo.PartnerCardId;
            PartnerCardSuffix = partnerCardInfo.PartnerCardSuffix;
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
            PartnerCardInfo partnerCardInfo = (PartnerCardInfo) obj;
            return PartnerId == partnerCardInfo.PartnerId &&
                   PartnerCardId == partnerCardInfo.PartnerCardId &&
                   PartnerCardSuffix == partnerCardInfo.PartnerCardSuffix;
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
            int result = PartnerId.GetHashCode();

            if (PartnerCardId != null)
            {
                result += PartnerCardId.GetHashCode();
            }

            if (PartnerCardSuffix != null)
            {
                result += PartnerCardSuffix.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the partner ID for the card.
        /// </summary>
        public Partner PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the card.
        /// </summary>
        public string PartnerCardId { get; set; }

        /// <summary>
        /// Gets or sets the partner suffix for the card.
        /// </summary>
        public string PartnerCardSuffix { get; set; }
    }
}