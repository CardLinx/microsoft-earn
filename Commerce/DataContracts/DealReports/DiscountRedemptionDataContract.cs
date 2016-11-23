//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents information about the redemption of a discount.
    /// </summary>
    [DataContract]
    public class DiscountRedemptionDataContract
    {
        /// <summary>
        /// Gets or sets the date and time of the redemption.
        /// </summary>
        /// <remarks>
        /// Date and time are in UTC.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "date_and_time")]
        public DateTime DateAndTime { get; set; }

        /// <summary>
        /// Gets or sets the settled amount of the transaction as it appears on the customer's statement.
        /// </summary>
        /// <remarks>
        /// * This is the gross amount, i.e. before the discount is applied.
        /// * Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "settlement_amount")]
        public int SettlementAmount { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount applied for this redemption.
        /// </summary>
        /// <remarks>
        /// Value is in the smallest unit of currency, e.g. cents in USD.
        /// </remarks>
        [DataMember(EmitDefaultValue = false, Name = "discount")]
        public int Discount { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the card used during the redemption.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "last_four_digits")]
        public string LastFourDigits { get; set; }
    }
}