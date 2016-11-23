//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;

    /// <summary>
    /// Represents a header record for a MasterCard transaction rebate confirmation file.
    /// </summary>
    public class RebateConfirmationHeader
    {
        /// <summary>
        /// Gets or sets the date and time at which the rebate confirmation file was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the member ICA for the intended file recipient.
        /// </summary>
        public string MemberIca { get; set; }

        /// <summary>
        /// Gets or sets the name of the rebate file to which this rebate confirmation file is linked.
        /// </summary>
        /// <remarks>
        /// Rebate confirmation files are generated once per day regardless of the number of rebate files generated. If more than one rebate file is generated, the name
        /// that appears in the rebate confirmation file is the name of the first processed rebate file.
        /// </remarks>
        public string FileName { get; set; }
    }
}