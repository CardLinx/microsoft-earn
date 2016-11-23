//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.Text;

    public class StatementCreditHeader
    {
        /// <summary>
        /// Gets or sets Date in YYYY-MM-DD format
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets File Sequence Number
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Construct string representation of the header file
        /// </summary>
        /// <returns>
        /// Returns the string header
        /// </returns>
        public string BuildFileHeader()
        {
            StringBuilder result = new StringBuilder();
            result.Append(AmexConstants.HeaderIdentifier);
            result.Append(AmexConstants.Delimiter);
            result.Append(Date.ToString("yyyy-MM-dd"));
            result.Append(AmexConstants.Delimiter);
            result.Append(SequenceNumber.ToString("D10"));
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.FromTo);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.FileType);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.PartnerId);
            result.Append(AmexConstants.Delimiter);
            result.Append(FileDescription);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.CountryCode);
            result.Append(AmexConstants.Delimiter);
            return result.ToString();
        }

        /// <summary>
        /// File Description
        /// </summary>
        private string FileDescription = "STATEMENT CREDIT FILE SMP";
    }
}