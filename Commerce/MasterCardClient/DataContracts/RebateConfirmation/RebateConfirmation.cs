//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a MasterCard rebate confirmation file.
    /// </summary>
    /// <remarks>
    /// Rebate confirmation files contain records only for failing rebates.
    /// </remarks>
    public class RebateConfirmation
    {
        /// <summary>
        /// Gets or sets the rebate confirmation file's header.
        /// </summary>
        public RebateConfirmationHeader Header { get; set; }

        /// <summary>
        /// Gets the rebate confirmation file's data records.
        /// </summary>
        public Collection<RebateConfirmationData> DataRecords
        {
            get
            {
                return dataRecords;
            }
        }
        private Collection<RebateConfirmationData> dataRecords = new Collection<RebateConfirmationData>();

        /// <summary>
        /// Gets or sets the rebate confirmation file's trailer.
        /// </summary>
        public RebateConfirmationTrailer Trailer { get; set; }
    }
}