//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on Card objects within the data store.
    /// </summary>
    public interface ICardOperations
    {
        /// <summary>
        /// Adds the card in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        ResultCode AddCard();

        /// <summary>
        /// Retrieves the card with the ID in the context.
        /// </summary>
        /// <returns>
        /// * The specified card if it exists.
        /// * Else returns null.
        /// </returns>
        Card RetrieveCard();

        /// <summary>
        /// Retrieves the list of cards that belong to the user in the context.
        /// </summary>
        /// <returns>
        /// The list of cards that belong to the user in the context.
        /// </returns>
        IEnumerable<InternalCard> RetrieveCards();

        /// <summary>
        /// Retrieves the list of cards that belong to the user in the context
        /// and enrolled in the specified reward program.
        /// </summary>
        /// <returns>
        /// The list of cards.
        /// </returns>
        IEnumerable<InternalCard> RetrieveCardsByRewardPrograms();

        /// <summary>
        /// Retrieve the list of cards that are currently active.
        /// </summary>
        /// <returns>
        /// The list of cards
        /// </returns>
        IEnumerable<InternalCard> RetrieveAllActiveCards();
            
        /// <summary>
        /// Removes the card from the specified reward programs.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        ResultCode RemoveCardFromRewardPrograms();

        /// <summary>
        /// Enrolls a user's cards of the specified brand(s) to the specified reward program(s).
        /// </summary>
        /// <param name="userGlobalId">The user's global id.</param>
        /// <param name="rewardPrograms">The reward programs to be enrolled in.</param>
        /// <param name="cardBrands">The card brands to be enrolled.</param>
        ResultCode EnrollCardsInRewardPrograms(Guid userGlobalId, RewardPrograms rewardPrograms, ICollection<CardBrand> cardBrands);

        /// <summary>
        /// Retrieve the list of cards that are currently active
        /// and have same partner card id.
        /// </summary>
        /// <returns>
        /// The list of cards
        /// </returns>
        IEnumerable<InternalCard> RetrieveCardsByPartnerCardId();

        /// <summary>
        /// Retrieve card token for another partner given a card token for a partner
        /// </summary>
        /// <param name="fromPartner">
        /// The Partner whose token is given
        /// </param>
        /// <param name="fromPartnerCardToken">
        /// Card Token for "from Partner"
        /// </param>
        /// <param name="toPartner">
        /// The Partner whose token is needed
        /// </param>
        /// <returns>
        /// The card token for the "to Partner"
        /// </returns>
        string RetrieveCardTokenForPartner(Partner fromPartner, string fromPartnerCardToken, Partner toPartner);

        /// <summary>
        /// Retrieves the PartnerCardIds for any MasterCard that has not yet been filtered.
        /// </summary>
        /// <returns>
        /// The list of PartnerCardIds for MasterCard cards that have not been filtered.
        /// </returns>
        IEnumerable<string> RetrieveUnfilteredMasterCards();

        /// <summary>
        /// Adds the date on which the specified list of PartnerCardIds for MasterCard cards was filtered within MasterCard's system.
        /// </summary>
        /// <param name="partnerCardIds">
        /// The list of PartnerCardIds for MasterCard cards whose filtered date to add.
        /// </param>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        ResultCode AddFilteredMasterCards(IEnumerable<string> partnerCardIds);
        
        /// <summary>
        /// Attempts to retrieve the Visa partner card ID for the card in the context.
        /// </summary>
        /// <returns>
        /// * The Visa partner card ID if successful.
        /// * Else returns null.
        /// </returns>
        string RetrieveVisaPartnerCardId();
 
        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}