//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a merchant stored in the data store.
    /// </summary>
    public class Merchant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Merchant"/> class.
        /// </summary>
        public Merchant()
        {
            PartnerMerchantAuthorizationIDs = new List<PartnerMerchantAuthorizationID>();
            PartnerMerchantSettlementIDs = new List<PartnerMerchantSettlementID>();
        }

        /// <summary>
        /// The Earn program ID for the merchant.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID for this merchant in the wider services space.
        /// </summary>
        public string GlobalID { get; set; }

        /// <summary>
        /// The name of this merchant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The global ID of the provider from which this Merchant was sourced.
        /// </summary>
        public string GlobalProviderID { get; set; }

        /// <summary>
        /// Specifies whether partner merchant IDs are included in the Merchant object.
        /// </summary>
        public bool IncludePartnerMerchantIDs { get; set; }

        /// <summary>
        /// An IEnumerable of the PartnerMerchantAuthorizationID objects associated with this merchant.
        /// </summary>
        public IEnumerable<PartnerMerchantAuthorizationID> PartnerMerchantAuthorizationIDs { get; set; }

        /// <summary>
        /// An IEnumerable of the PartnerMerchantSettlementID objects associated with this merchant.
        /// </summary>
        public IEnumerable<PartnerMerchantSettlementID> PartnerMerchantSettlementIDs { get; set; }
    }
}