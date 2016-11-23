//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    
    /// <summary>
    /// Contains a record describing a reward that needs to be paid out.
    /// </summary>
    public class RewardPayoutRecord
    {
        /// <summary>
        /// Gets or sets the ID of the reward payout record.
        /// </summary>
        public Guid RewardPayoutId { get; set; }

        /// <summary>
        /// Gets or sets the referral type code for which this referral is being made.
        /// </summary>
        public RewardType RewardType { get; set; }

        /// <summary>
        /// Gets or sets the properties for the reward being paid out.
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// The ID of the recipient of the reward.
        /// </summary>
        public Guid PayeeId { get; set; }

        /// <summary>
        /// The type of the payee.
        /// </summary>
        public PayeeType PayeeType { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the reward has been rescinded.
        /// </summary>
        public bool Rescinded { get; set; }
    }
}