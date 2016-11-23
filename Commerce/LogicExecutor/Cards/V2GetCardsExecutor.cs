//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute a V2 get cards request.
    /// </summary>
    public class V2GetCardsExecutor
    {
        /// <summary>
        /// Initializes a new instance of the V2GetCardsExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public V2GetCardsExecutor(CommerceContext context)
        {
            Context = context;
            CardOperations = CommerceOperationsFactory.CardOperations(Context);
        }

        /// <summary>
        /// Executes the get cards invocation.
        /// </summary>
        public void Execute()
        {
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));

            User user = sharedUserLogic.RetrieveUser();
            Context[Key.User] = user;

            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            if (user != null)
            {
                resultSummary.SetResultCode(GetCards());
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.UnregisteredUser);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for Card operations.
        /// </summary>
        internal ICardOperations CardOperations { get; set; }

        /// <summary>
        /// Retrieves the list of cards that belong to the specified user.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode GetCards()
        {
            List<V2CardDataContract> cards = new List<V2CardDataContract>();

            // Retrieve the list of InternalCards.
            Context.Log.Verbose("Retrieving list of cards belonging to user {0}", ((User)Context[Key.User]).GlobalId);
            IList<InternalCard> internalCards = CardOperations.RetrieveCardsByRewardPrograms().ToList();
            Context.Log.Verbose("{0} cards were retrieved from the data store.", internalCards.Count());

            // Build the list of CardDataContracts from the list of InternalCards.
            foreach(InternalCard internalCard in internalCards)
            {
                cards.Add(BuildCardDataContract(internalCard));
            }

            ((V2GetCardsResponse)Context[Key.Response]).Cards = cards;

            return ResultCode.Success;
        }

        /// <summary>
        /// Builds a V2CardDataContract from the specified InternalCard.
        /// </summary>
        /// <param name="internalCard">
        /// The InternalCard from which to build the CardDataContract.
        /// </param>
        /// <returns>
        /// The CardDataContract built from the specified InternalCard.
        /// </returns>
        private V2CardDataContract BuildCardDataContract(InternalCard internalCard)
        {
            Context.Log.Verbose("Building new CardDataContract from InternalCard.");
            return new V2CardDataContract
            {
                Id = General.GuidFromInteger(internalCard.Id),
                LastFourDigits = internalCard.LastFourDigits,
                NumberOfCardholders = internalCard.NumberOfCardholders,
                CardBrand = internalCard.CardBrand.ToString()
            };
        }
    }
}