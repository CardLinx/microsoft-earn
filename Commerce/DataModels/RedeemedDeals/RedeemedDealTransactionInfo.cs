//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    public class RedeemedDealTransactionInfo
    {
        /// <summary>
        /// Gets or sets the Merchant name
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the Redemption Id
        /// </summary>
        public Guid RedemptionId { get; set; }

        /// <summary>
        /// Gets or sets the transaction date
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction amount
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the Last 4 digits (PAN) of the card used in this transaction
        /// </summary>
        public string CardLastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the type of card used in the transaction
        /// </summary>
        public CardBrand CardType { get; set; }

        /// <summary>
        /// Gets or sets the deal id associated with this transaction.
        /// </summary>
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the provider id of the offer associated with this transaction.
        /// </summary>
        public int ProviderId { get; set; }
    }
}