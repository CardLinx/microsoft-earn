//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents a claimed deal stored in the data store.
    /// </summary>
    public class ClaimedDeal
    {
        /// <summary>
        /// Initializes a new instance of the ClaimedDeal class.
        /// </summary>
        public ClaimedDeal()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the canonical ID for this claimed deal.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the deal that was claimed.
        /// </summary>
        public Guid GlobalDealId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who claimed the deal.
        /// </summary>
        public Guid GlobalUserId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the card with which the deal was claimed.
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// Gets or sets the Partner with whom the deal was claimed.
        /// </summary>
        public Partner Partner { get; set; }
    }
}