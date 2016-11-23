//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    public class PartnerMerchantAuthorizationID
    {
        // Initializes a new instance of the <see cref="PartnerMerchantAuthorizationID"> class.
        public PartnerMerchantAuthorizationID()
        {
            AddOrUpdate = true;
        }

        /// <summary>
        /// The Partner company from which the information in this object originated.
        /// </summary>
        public CardBrand Partner { get; set; }

        /// <summary>
        /// The ID used at authorization time to determine if the transaction occurred at a valid location for the merchant to which this object belongs.
        /// </summary>
        public string AuthorizationID { get; set; }

        /// <summary>
        /// Specifies whether this record is to be added or updated (true) or deactivated (false).
        /// </summary>
        public bool AddOrUpdate { get; set; }
    }
}