//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    /// <summary>
    /// The different types of an offer that can be redeemed.
    /// </summary>
    public enum OfferType
    {
        /// <summary>
        /// Indicates the redeemed offer was a pre-purchased deal.
        /// </summary>
        Purchased,

        /// <summary>
        /// Indicates the redeemed offer required no upfront purchase (coupon-like).
        /// </summary>
        ConsumerClipped,

        /// <summary>
        /// Indicates the redeemed offer was funded by the merchant.
        /// </summary>
        MerchantFunded
    }
}