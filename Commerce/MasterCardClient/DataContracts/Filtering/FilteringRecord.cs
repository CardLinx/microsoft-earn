//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a record for a MasterCard transaction filtering file.
    /// </summary>
    public class FilteringRecord
    {
        /// <summary>
        /// Gets or sets the bank customer number (i.e. the PartnerCardId) for the card to associate with the merchant set ID.
        /// </summary>
        public string BankCustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the authorization set ID to associate with the bank customer number.
        /// </summary>
        public string AuthorizationSetId { get; set; }

        /// <summary>
        /// Gets or sets the clearing set ID to associate with the bank customer number.
        /// </summary>
        public string ClearingSetId { get; set; }

        /// <summary>
        /// Gets or sets the threshold transaction amount at which a transaction will trigger authorization and clearing events.
        /// </summary>
        public decimal Threshold { get; set; }

        /// <summary>
        ///  Gets or sets the date at which the relationship between the card and the merchant set came into effect.
        /// </summary>
        public DateTime EffectiveDate { get; set; }
    }
}