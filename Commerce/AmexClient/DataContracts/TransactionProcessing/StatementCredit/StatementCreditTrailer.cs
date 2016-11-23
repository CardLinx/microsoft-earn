//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Text;

    public class StatementCreditTrailer
    {
        /// <summary>
        /// Gets or sets Trailer Count
        /// </summary>
        public int TrailerCount { get; set; }

        /// <summary>
        /// Gets or sets the total Trailer Amount
        /// </summary>
        public decimal TrailerAmount { get; set; }

        /// <summary>
        /// Construct string representation of the trailer of statment credit file
        /// </summary>
        /// <returns>
        /// Returns the string trailer
        /// </returns>
        public string BuildFileTrailer()
        {
            StringBuilder result = new StringBuilder();
            result.Append(AmexConstants.TrailerIdentifier);
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.FileType);
            result.Append(AmexConstants.Delimiter);
            result.Append(TrailerCount);
            result.Append(AmexConstants.Delimiter);
            result.Append(FormattedTrailerAmount());
            result.Append(AmexConstants.Delimiter);
            return result.ToString();
        }

        /// <summary>
        /// Formatted amount Decimal(15,2) with sign
        /// </summary>
        /// <returns>
        /// Formatted amount
        /// </returns>
        private string FormattedTrailerAmount()
        {
            const string fmt = "0000000000000.##";
            string formattedAmount = TrailerAmount.ToString(fmt);
            if (TrailerAmount > 0)
            {
                formattedAmount = "+" + formattedAmount;
            }
            return formattedAmount;
        }
    }
}