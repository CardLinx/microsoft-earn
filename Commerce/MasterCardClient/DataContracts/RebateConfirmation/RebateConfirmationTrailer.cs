//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a trailer record for a MasterCard transaction rebate confirmation file.
    /// </summary>
    public class RebateConfirmationTrailer
    {
        /// <summary>
        /// Gets or sets the number of rebate records that were not processed due to an exception.
        /// </summary>
        public long ExceptionRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the number of rebate records that were processed successfully.
        /// </summary>
        public long SuccessRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of rebate records processed to create this rebate confirmation file.
        /// </summary>
        public long TotalProcessedRecordCount { get; set; }

        /// <summary>
        /// Gets or sets the member ICA for the intended file recipient.
        /// </summary>
        public string MemberIca { get; set; }
    }
}