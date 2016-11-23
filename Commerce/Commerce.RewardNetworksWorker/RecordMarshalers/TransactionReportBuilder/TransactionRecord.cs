//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker.RecordMarshalers.TransactionReportBuilder
{
    using System;
    using Lomo.Commerce.DataModels;

    public class TransactionRecord
    {
        public TransactionRecord()
        {
            MerchantId = MerchantName = CardMemberCity = CardMemberState = CardMemberZip = string.Empty;
            DineIndicator = "N";    //Initialize every member to "New" diner for now.
        }

        /// <summary>
        /// Gets or sets the sequence of the transaction record
        /// </summary>
        public int RecordSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Merchant Id
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the Merchant name
        /// </summary>
        public string MerchantName { get; set; }

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
        /// Gets or sets whether the card member is a regular or new diner
        /// </summary>
        public string DineIndicator { get; private set; }

        /// <summary>
        /// Gets or sets the unique transaction identifier
        /// </summary>
        public string TransactionIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the Card member city
        /// </summary>
        public string CardMemberCity { get; set; }

        /// <summary>
        /// Gets or sets the Card member state
        /// </summary>
        public string CardMemberState { get; set; }
        
        /// <summary>
        /// Gets or sets the Card member zip
        /// </summary>
        public string CardMemberZip { get; set; }
    }
}