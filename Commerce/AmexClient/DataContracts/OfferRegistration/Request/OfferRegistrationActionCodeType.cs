//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    /// <summary>
    /// Type of registration action for offer registration with Amex.
    /// </summary>
    public static class OfferRegistrationActionCodeType
    {
        /// <summary>
        /// Add a record
        /// </summary>
        public const string Add = "A";

        /// <summary>
        /// Update a record
        /// </summary>
        public const string Update = "U"; 

        /// <summary>
        /// Delete a record
        /// </summary>
        public const string Delete = "D";
    }
}