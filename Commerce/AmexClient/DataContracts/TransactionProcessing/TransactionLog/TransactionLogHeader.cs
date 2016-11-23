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
    /// Represents Amex Transaction Log File
    /// </summary>
    public class TransactionLogHeader
    {
        /// <summary>
        /// Create a new instance of T-Log header
        /// </summary>
        /// <param name="header">
        /// Header Line item from file
        /// </param>
        public TransactionLogHeader(string header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header", "header cannot be null");
            }

            char[] charSeparators = { '|' };
            string[] tokens = header.Split(charSeparators, StringSplitOptions.None);

            if (tokens.Length != 7)
            {
                throw new InvalidDataException("T-Log Header Record is not in expected format");
            }

            Date = DateTime.ParseExact(tokens[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            SequenceNumber = int.Parse(tokens[2]);
        }

        /// <summary>
        /// Gets or sets Date in YYYY-MM-DD format
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets File Sequence Number
        /// </summary>
        public int SequenceNumber { get; set; }
    }
}