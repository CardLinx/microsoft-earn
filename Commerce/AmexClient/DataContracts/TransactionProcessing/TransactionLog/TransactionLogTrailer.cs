//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.IO;

    public class TransactionLogTrailer
    {
        /// <summary>
        /// Create a new instance of T-Log trailer
        /// </summary>
        /// <param name="footer">
        /// Footer Line item from file
        /// </param>
        public TransactionLogTrailer(string footer)
        {
            if (footer == null)
            {
                throw new ArgumentNullException("footer", "footer cannot be null");
            }

            char[] charSeparators = { '|' };
            string[] tokens = footer.Split(charSeparators, StringSplitOptions.None);

            if (tokens.Length != 4)
            {
                throw new InvalidDataException("T-Log Trailer Record is not in expected format");
            }
            // tokens[0] and tokens[1] are constants
            TrailerCount = int.Parse(tokens[2]);
        }

        /// <summary>
        /// Gets or sets Trailer Count
        /// </summary>
        public int TrailerCount { get; set; }
    }
}