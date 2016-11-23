//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents a partner user stored in the data store.
    /// </summary>
    public class PartnerUserInfo
    {
        /// <summary>
        /// Initializes a new instance of the PartnerUserInfo.
        /// </summary>
        public PartnerUserInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class derived from PartnerUserInfo class, using the fields from the specified other
        /// PartnerUserInfo.
        /// </summary>
        /// <param name="partnerUserInfo">
        /// The other PartnerUserInfo whose fields to copy.
        /// </param>
        internal PartnerUserInfo(PartnerUserInfo partnerUserInfo)
        {
            PartnerId = partnerUserInfo.PartnerId;
            PartnerUserId = partnerUserInfo.PartnerUserId;
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
            PartnerUserInfo partnerUserInfo = (PartnerUserInfo) obj;
            return PartnerId == partnerUserInfo.PartnerId &&
                   PartnerUserId == partnerUserInfo.PartnerUserId;
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

            if (PartnerUserId != null)
            {
                result += PartnerUserId.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the partner ID for the user.
        /// </summary>
        public Partner PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the partner ID for the user.
        /// </summary>
        public string PartnerUserId { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the partner user ID was assigned by the partner.
        /// </summary>
        /// <remarks>
        /// Only IDs assigned by the partner are stored in the data store. Others are generated programatically.
        /// </remarks>
        public bool AssignedByPartner { get; set; }
    }
}