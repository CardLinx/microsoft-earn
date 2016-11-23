//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Class to reprsent Merchant Details for building PTS file
    /// </summary>
    class PtsMerchantInfo
    {
        /// <summary>
        /// Gets or sets the PartnerMerchantId
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets the MerchantName
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the ReimbursementTender for deals referenced by this PtsMerchantInfo object.
        /// </summary>
        public ReimbursementTender ReimbursementTender { get; set; }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            int result = PartnerMerchantId.GetHashCode()
                       + MerchantName.GetHashCode()
                       + ReimbursementTender.GetHashCode();
            return result;
        }

        /// <summary>
        /// Determines whether the specified object has equal values to this object in all fields.
        /// </summary>
        /// <param name="obj">
        /// The object whose values to compare.
        /// </param>
        /// /// <returns>
        /// True if the two objects have the same values.
        /// </returns>
        public override bool Equals(object obj)
        {
            PtsMerchantInfo merchantInfo = (PtsMerchantInfo)obj;
            return merchantInfo.PartnerMerchantId == PartnerMerchantId &&
                   merchantInfo.MerchantName == MerchantName &&
                   merchantInfo.ReimbursementTender == ReimbursementTender;
        }
    }
}