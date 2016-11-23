//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Text;

    /// <summary>
    /// Represents Amex Merchant Registration File Footer
    /// </summary>
    public class OfferRegistrationTrailer
    {
        /// <summary>
        /// Gets or sets Trailer Count
        /// </summary>
        public int TrailerCount { get; set; }

        /// <summary>
        /// Construct string representation of the trailer file
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
            return result.ToString();
        }
    }
}