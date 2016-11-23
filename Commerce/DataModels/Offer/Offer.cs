//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents an offer stored in the data store.
    /// </summary>
    public class Offer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Offer"/> class.
        /// </summary>
        public Offer()
        {
            Active = true;
        }

        /// <summary>
        /// The Earn program ID for the offer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID assigned within the wider system to this offer.
        /// </summary>
        public Guid GlobalID { get; set; }

        /// <summary>
        /// The global ID of the provider from which this offer was sourced.
        /// </summary>
        public string GlobalProviderID { get; set; }

        /// <summary>
        /// The offer's type, e.g. Earn or Burn.
        /// </summary>
        public OfferType OfferType { get; set; }

        /// <summary>
        /// The percent of the settlement amount to apply as Earn credits, or the percent of dollars spent for which Earn credits
        ///  can be used instead.
        /// </summary>
        public decimal PercentBack { get; set; }

        /// <summary>
        /// Specifies whether this offer is currently active.
        /// </summary>
        public bool Active { get; set; }
    }
}