//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the distribution history for a user.
    /// </summary>
    [DataContract]
    public class DistributionHistoryDataContract
    {
        /// <summary>
        /// Gets or sets the date of this distribution.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "distribution_date")]
        public DateTime DistributionDate { get; set; }

        /// <summary>
        /// Gets or sets the amount of the distribution.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency in which the distribution was made.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the voucher created from the distribution.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "expiration_date")]
        public DateTime ExpirationDate { get; set; }
    }
}