//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the distribution history for a user.
    /// </summary>
    public class DistributionHistory
    {
        /// <summary>
        /// Gets or sets the date of this distribution.
        /// </summary>
        public DateTime DistributionDate { get; set; }

        /// <summary>
        /// Gets or sets the amount of the distribution.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency in which the distribution was made.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the voucher created from the distribution.
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}