//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Generator for FirstData Offer Ids
    /// </summary>
    public static class FirstDataOfferIdGenerator
    {
        /// <summary>
        /// Create a new offer Id for registering a deal with First Data
        /// </summary>
        /// <param name="context">
        /// Incoming Commerce Context
        /// </param>
        /// <returns>
        /// The Id of the new offer
        /// </returns>
        public static string GenerateNewOfferId(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            return General.GenerateShortGuid();
        }
    }
}