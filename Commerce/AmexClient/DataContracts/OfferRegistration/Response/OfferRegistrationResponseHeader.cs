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
    /// Represents Amex Merchant Registration Response Header Record
    /// </summary>
    public class OfferRegistrationResponseHeader
    {
        /// <summary>
        /// Create a new instance of response header
        /// </summary>
        /// <param name="header">
        /// Header Line item from file
        /// </param>
        public OfferRegistrationResponseHeader(string header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header", "header cannot be null");
            }

            char[] charSeparators = { '|' };
            string[] tokens = header.Split(charSeparators, StringSplitOptions.None);

            if (tokens.Length < 9)
            {
                throw new InvalidDataException("Response Header Record is not in expected format");
            }

            // token[0] is constant header identifier H
            Date = DateTime.ParseExact(tokens[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            SequenceNumber = int.Parse(tokens[2]);
            // token[3] is constant FromTo A2P
            // token[4] is constant FileType 11
            // token[5] is FileDescription
            // token[5] is constant PartnerId
            ResponseCode = tokens[7];
            ResponseCodeMessage = tokens[8];
            // tokens[9] is Filler
        }

        /// <summary>
        /// Gets or sets Action Code A=Accepted, R=Rejected
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets ResponseCode Message
        /// </summary>
        public string ResponseCodeMessage { get; set; }

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