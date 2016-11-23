//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a MasterCard clearing file.
    /// </summary>
    public class Clearing
    {
        /// <summary>
        /// Gets or sets the clearing file's header.
        /// </summary>
        public ClearingHeader Header { get; set; }

        /// <summary>
        /// Gets the clearing file's data records.
        /// </summary>
        public Collection<ClearingData> DataRecords
        {
            get
            {
                return dataRecords;
            }
        }
        private Collection<ClearingData> dataRecords = new Collection<ClearingData>();

        /// <summary>
        /// Gets or sets the clearing file's trailer.
        /// </summary>
        public ClearingTrailer Trailer { get; set; }
    }
}