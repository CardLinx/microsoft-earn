//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Represents a redemption detail record for a First Data extract file.
    /// </summary>
    public class RedemptionDetail
    {
        /// <summary>
        /// Gets or sets the ID of the provider to which the extract file belongs.
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the transaction described in this record.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction represented by the record.
        /// </summary>
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the transaction in EST.
        /// </summary>
        public DateTime TransactionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of the transaction.
        /// </summary>
        public DateTime LocalDateTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the First Data publisher.
        /// </summary>
        public string PublisherName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the First Data publisher.
        /// </summary>
        public string PublisherId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the consumer who made the transaction.
        /// </summary>
        public string ConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the suffix of the card used in the transaction.
        /// </summary>
        public string CardSuffix { get; set; }

        /// <summary>
        /// Gets or sets the ID of the offer involved in the transaction.
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the type of the offer involved in the transaction.
        /// </summary>
        public OfferType OfferType { get; set; }

        /// <summary>
        /// Gets or sets the ID assigned to the acceptance of the offer involved in the transaction.
        /// </summary>
        public string OfferAcceptanceId { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the offer involved in the transaction was accepted.
        /// </summary>
        public DateTime OfferAcceptanceDateTime { get; set; }

        /// <summary>
        /// Gets or sets the MID of the merchant at which the offer was registered.
        /// </summary>
        public string RegistrationMid { get; set; }

        /// <summary>
        /// Gets or sets the MID of the merchant at which the offer was redeemed.
        /// </summary>
        public string RedemptionMid { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the offer starts.
        /// </summary>
        public DateTime OfferStartDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the offer ends.
        /// </summary>
        public DateTime OfferEndDate { get; set; }

        /// <summary>
        /// Gets or sets the price of the purchase involved in the transaction.
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// Gets or sets the minimum amount of the transaction required for the deal to take effect.
        /// </summary>
        public decimal MinimumTransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the percentage off for the offer involved in the transaction.
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Gets or sets the amount of the discount involved in the transaction.
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// The total amount of the transaction.
        /// </summary>
        public decimal TotalTransactionAmount { get; set; }

        /// <summary>
        /// The applied amount of the discount from the transaction.
        /// </summary>
        public decimal RedemptionDiscountAmount { get; set; }
    }
}