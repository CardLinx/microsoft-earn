//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the result of a get MSS voucher distribution database call.
    /// </summary>
    public class MssVoucherDistributionHistory
    {
        /// <summary>
        /// Gets or sets the remaining credit amount.
        /// </summary>
        public int AmountRemaining { get; set; }

        /// <summary>
        /// Gets or sets the distribution history
        /// </summary>
        public IEnumerable<DistributionHistory> DistributionHistory { get; set; }

        /// <summary>
        /// Gets or sets the transaction history
        /// </summary>
        public IEnumerable<TransactionHistory> TransactionHistory { get; set; }
    }
}