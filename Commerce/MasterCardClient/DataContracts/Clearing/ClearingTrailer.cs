//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a trailer record for a MasterCard transaction clearing file.
    /// </summary>
    public class ClearingTrailer
    {
        /// <summary>
        /// Gets or sets the total number of records in the file, including header and trailer.
        /// </summary>
        public long RecordCount { get; set; }

        /// <summary>
        /// Gets or sets the member ICA for the intended file recipient.
        /// </summary>
        public string MemberIca { get; set; }
    }
}