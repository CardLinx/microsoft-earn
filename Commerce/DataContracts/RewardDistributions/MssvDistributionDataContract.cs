//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a Microsoft Store voucher distribution.
    /// </summary>
    [DataContract]
    public class MssvDistributionDataContract
    {
        /// <summary>
        /// Gets or sets the amount to distribute.
        /// </summary>
        /// <remarks>
        /// Note that the amount to specify here is in "real" currency value, e.g. dollars and cents instead of just cents.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "distribution_amount")]
        public Decimal DistributionAmount { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time at which the voucher being issued by this distribution will expire.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "voucher_expiration_utc")]
        public DateTime VoucherExpirationUtc { get; set; }

        /// <summary>
        /// Gets or sets notes to store regarding the distribution, if any.
        /// </summary>
        /// <remarks>
        /// Notes are first HTML encoded and then truncated at 500 characters.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "notes")]
        public string Notes { get; set; }
    }
}