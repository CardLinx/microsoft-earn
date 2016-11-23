//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents a partner deal stored in the data store.
    /// </summary>
    public class PartnerDealInfo
    {
        /// <summary>
        /// Initializes a new instance of the PartnerDealInfo.
        /// </summary>
        public PartnerDealInfo()
        {
            PartnerMerchantLocations = new Collection<PartnerMerchantLocationInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the class derived from PartnerDealInfo, using the fields from the specified other
        /// PartnerDealInfo.
        /// </summary>
        /// <param name="partnerDealInfo">
        /// The other PartnerDealInfo whose fields to copy.
        /// </param>
        internal PartnerDealInfo(PartnerDealInfo partnerDealInfo)
        {
            PartnerId = partnerDealInfo.PartnerId;
            PartnerDealId = partnerDealInfo.PartnerDealId;
            PartnerDealRegistrationStatusId = partnerDealInfo.PartnerDealRegistrationStatusId;
            PartnerMerchantLocations = new Collection<PartnerMerchantLocationInfo>();
            foreach (PartnerMerchantLocationInfo partnerMerchantLocationInfo in partnerDealInfo.PartnerMerchantLocations)
            {
                PartnerMerchantLocations.Add(new PartnerMerchantLocationInfo
                {
                    PartnerMerchantId = partnerMerchantLocationInfo.PartnerMerchantId,
                    PartnerMerchantIdType = partnerMerchantLocationInfo.PartnerMerchantIdType
                });
            }
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
            PartnerDealInfo partnerDealInfo = (PartnerDealInfo) obj;
            return PartnerId == partnerDealInfo.PartnerId &&
                   PartnerDealId == partnerDealInfo.PartnerDealId &&
                   PartnerMerchantLocations.Except(partnerDealInfo.PartnerMerchantLocations).Any() == false &&
                   partnerDealInfo.PartnerMerchantLocations.Except(PartnerMerchantLocations).Any() == false;
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

            foreach (PartnerMerchantLocationInfo partnerMerchantLocationInfo in PartnerMerchantLocations)
            {
                result += partnerMerchantLocationInfo.GetHashCode();
            }

            if (PartnerDealId != null)
            {
                result += PartnerDealId.GetHashCode();
            }
            
            return result;
        }

        /// <summary>
        /// Gets or sets the partner ID for the deal.
        /// </summary>
        public Partner PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the deal.
        /// </summary>
        public string PartnerDealId { get; set; }

        /// <summary>
        /// Gets or sets the partner deal registration status ID for the deal.
        /// </summary>
        public PartnerDealRegistrationStatus PartnerDealRegistrationStatusId { get; set; }

        /// <summary>
        /// Gets or sets the partner merchant locations for the deal.
        /// </summary>
        public Collection<PartnerMerchantLocationInfo> PartnerMerchantLocations { get; private set; }
    }
}