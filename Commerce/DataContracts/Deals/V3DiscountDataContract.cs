//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a discount.
    /// </summary>
    [DataContract]
    public class V3DiscountDataContract
    {
        /// <summary>
        /// Gets or sets the canonical ID for this discount.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the merchant offering the deal.
        /// </summary>
        /// <remarks>
        /// This merchant name will appear in customer notifications and on credit card statements.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the MerchantID of the merchant offering the deal.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the type of the discount.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discount_type")]
        public string DiscountType { get; set; }

        /// <summary>
        /// Gets or sets the date at which the discount offer begins.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date at which the discount offer ends.
        /// </summary>
        /// <remarks>
        /// * Default value is DateTime.MaxValue. Because this must be a pure data contract, OnDeserializing cannot be
        /// implemented to provide a default value when none is specified. Because of this, DateTime.MinValue will be replaced by
        /// DateTime.MaxValue deeper within the system.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the summary description of the deal.
        /// </summary>
        /// <remarks>
        /// This string may appear on the user's receipt, credit card statement, or both.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "discount_summary")]
        public string DiscountSummary { get; set; }

        /// <summary>
        /// Gets or sets the property bag that contains discount details.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
         Justification = "Property in data contract must be settable by callers or a constructor must be created. These are " +
                         " supposed to be pure data contracts, so constructors aren't allowed.")]
        [DataMember(EmitDefaultValue = false, Name = "properties")]
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Gets or sets list of day - time restrictions for this deal
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "day_time_restrictions")]
        public IEnumerable<DayTimeRestriction> DayTimeRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the list of partners and partner merchant IDs at which a discount is available.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partner_merchant_ids")]
        public IEnumerable<DiscountPartnerMerchantIds> PartnerMerchantIds { get; set; }
    }
}