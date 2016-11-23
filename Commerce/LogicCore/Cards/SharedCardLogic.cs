//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Linq;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using System.Collections.Generic;

    /// <summary>
    /// Contains methods to perform shared business logic for card objects.
    /// </summary>
    public class SharedCardLogic
    {
        /// <summary>
        /// Initializes a new instance of the SharedCardLogic class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="cardOperations">
        /// The object to use to perform operations on cards.
        /// </param>
        public SharedCardLogic(CommerceContext context,
                               ICardOperations cardOperations)
        {
            Context = context;
            CardOperations = cardOperations;
        }

        /// <summary>
        /// Retrieves the card with the specified ID.
        /// </summary>
        /// <returns>
        /// * The specified card if it exists.
        /// * Else returns null.
        /// </returns>
        public Card RetrieveCard()
        {
            Card result;

            Context.Log.Verbose("Retrieving specified Card from the data store.");
            result = CardOperations.RetrieveCard();
            if (result != null)
            {
                Context.Log.Verbose("Specified Card retrieved from data store.");
            }
            else
            {
                Context.Log.Verbose("Specified Card does not exist within the data store.");
            }

            return result;
        }

        /// <summary>
        /// Retrieves the existing cards for the user in the context.
        /// </summary>
        /// <returns>
        /// The list of InternalCard objects corresponding to the user's existing cards.
        /// </returns>
        public IEnumerable<InternalCard> RetrieveUserCards()
        {
            Context.Log.Verbose("Retrieving list of cards belonging to user {0}", ((User)Context[Key.User]).GlobalId);
            IEnumerable<InternalCard> result = CardOperations.RetrieveCards();
            Context.Log.Verbose("{0} cards were retrieved from the data store.", result.Count());

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on cards.
        /// </summary>
        private ICardOperations CardOperations { get; set; }
    }
}