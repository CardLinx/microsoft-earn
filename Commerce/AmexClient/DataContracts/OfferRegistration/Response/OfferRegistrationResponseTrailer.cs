//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents Amex Merchant Registration Response File Footer
    /// </summary>
    public class OfferRegistrationResponseTrailer
    {
        /// <summary>
        /// Create a new instance of response trailer
        /// </summary>
        /// <param name="footer">
        /// Footer Line item from file
        /// </param>
        public OfferRegistrationResponseTrailer(string footer)
        {
            if (footer == null)
            {
                throw new ArgumentNullException("footer", "footer cannot be null");
            }

            char[] charSeparators = { '|' };
            string[] tokens = footer.Split(charSeparators, StringSplitOptions.None);

            if (tokens.Length < 3)
            {
                throw new InvalidDataException("Response Trailer Record is not in expected format");
            }

            // token[0] is constant trailer identifier T
            // token[1] is constant FileType 02
            TrailerCount = int.Parse(tokens[2]);
            // tokens[3] is Filler
        }

        /// <summary>
        /// Gets or sets Trailer Count
        /// </summary>
        public int TrailerCount { get; set; }

    }
}