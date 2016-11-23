//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the merchant record for a First Data PTS file.
    /// </summary>
    public class MerchantRecord
    {
        /// <summary>
        /// Gets or sets the partner ID of the merchant referenced in this record.
        /// </summary>
        public string PartnerMerchantId { get; set; }

        /// <summary>
        /// Gets or sets merchant name
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets the list of detail records for the merchant within this record.
        /// </summary>
        public Collection<DetailRecord> DetailRecords
        {
            get
            {
                return detailRecords;
            }
        }
        private Collection<DetailRecord> detailRecords = new Collection<DetailRecord>();
    }
}