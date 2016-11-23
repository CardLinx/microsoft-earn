//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Represents Amex Transaction Log Detail Record
    /// </summary>
    public class TransactionLogDetail
    {
        /// <summary>
        /// Create new instance of T-Log Detail record from line item
        /// </summary>
        /// <param name="record">
        /// String record
        /// </param>
        public TransactionLogDetail(string record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "record cannot be null");
            }

            char[] charSeparators = { '|' };
            string[] tokens = record.Split(charSeparators, StringSplitOptions.None);

            if (tokens.Length != 9)
            {
                throw new InvalidDataException("Response Detail Record is not in expected format");
            }

            // token[0] is constant detail identifier
            PartnerId = tokens[1].Trim();
            TransactionId = tokens[2].Trim();
            OfferId = tokens[3].Trim();
            TransactionDate = DateTime.ParseExact(tokens[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TransactionAmount = decimal.Parse(tokens[5]);
            CardToken = tokens[6].Trim();
            MerchantNumber = tokens[7].Trim();
        }

        /// <summary>
        /// Gets or sets Partner Id 
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets TransactionID
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets OfferID
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets TransactionDate
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets the card token.
        /// </summary>
        public string CardToken { get; set; }

        /// <summary>
        /// Gets or sets the merchant number.
        /// </summary>
        public string MerchantNumber { get; set; }
    }
}