//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents internal information about a card stored in the data store.
    /// </summary>
    public class InternalCard
    {
        /// <summary>
        /// Initializes a new instance of the InternalCard class from the specified UserId.
        /// </summary>
        /// <param name="userId">
        /// ID of the user to whom the card belongs.
        /// </param>
        public InternalCard(Guid userId)
            : this()
        {
            GlobalUserId = userId;
            NameOnCard = String.Empty;
            LastFourDigits = String.Empty;
            RewardPrograms = RewardPrograms.CardLinkOffers;
        }

        /// <summary>
        /// Initializes a new instance of the InternalCard class for serialization.
        /// </summary>
        public InternalCard()
        {
            PanToken = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the InternalCard class, using the fields from the specified other InternalCard.
        /// </summary>
        /// <param name="internalCard">
        /// The other InternalCard whose fields to copy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter card cannot be null.
        /// </exception>
        public InternalCard(InternalCard internalCard)
            : this()
        {
            if (internalCard == null)
            {
                throw new ArgumentNullException("internalCard", "Parameter internalCard cannot be null.");
            }

            Id = internalCard.Id; 
            GlobalUserId = internalCard.GlobalUserId;
            NameOnCard = internalCard.NameOnCard;
            LastFourDigits = internalCard.LastFourDigits;
            Expiration = internalCard.Expiration;
            CardBrand = internalCard.CardBrand;
            RewardPrograms = internalCard.RewardPrograms;
            PanToken = internalCard.PanToken;
        }

        /// <summary>
        /// Determines whether the specified object has equal values to this object in all fields.
        /// </summary>
        /// <param name="obj">
        /// The object whose values to compare.
        /// </param>
        /// <returns>
        /// True if the two objects have the same values.
        /// </returns>
        public override bool Equals(object obj)
        {
            Card card = (Card) obj;
            return Id == card.Id &&
                   GlobalUserId == card.GlobalUserId &&
                   NameOnCard == card.NameOnCard &&
                   LastFourDigits == card.LastFourDigits &&
                   General.DateTimesComparable(Expiration, card.Expiration) == true &&
                   CardBrand == card.CardBrand &&
                   RewardPrograms == card.RewardPrograms &&
                   PanToken == card.PanToken;
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        /// <remarks>
        /// * CA2218:
        ///   * If two objects are equal in value based on the Equals override, they must both return the same value for calls
        ///     to GetHashCode.
        ///   * GetHashCode must be overridden whenever Equals is overridden.
        /// * It is fine if the value overflows.
        /// </remarks>
        public override int GetHashCode()
        {
            int result = Id.GetHashCode() +
                         GlobalUserId.GetHashCode() +
                         Expiration.GetHashCode() +
                         CardBrand.GetHashCode() +
                         RewardPrograms.GetHashCode();

            if (NameOnCard != null)
            {
                result += NameOnCard.GetHashCode();
            }

            if (LastFourDigits != null)
            {
                result += LastFourDigits.GetHashCode();
            }

            if (PanToken != null)
            {
                result += PanToken.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets integer id for this card.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user to whom the card belongs.
        /// </summary>
        public Guid GlobalUserId { get; set; }

        /// <summary>
        /// Gets or sets the name on the credit card.
        /// </summary>
        public string NameOnCard { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the card number.
        /// </summary>
        public string LastFourDigits { get; set; }

        /// <summary>
        /// Gets or sets the expiration of the card.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets or sets the brand of the card.
        /// </summary>
        public CardBrand CardBrand { get; set; }

        /// <summary>
        /// Gets or sets the Number of users who have this card
        /// </summary>
        public int NumberOfCardholders { get; set; }

        /// <summary>
        /// Gets or sets the Reward programs the card is enrolled for
        /// </summary>
        public RewardPrograms RewardPrograms { get; set; }

        /// <summary>
        /// Gets or sets the token that uniquely represents the PAN on the card.
        /// </summary>
        /// <remarks>
        /// At this time, the First Data partner card ID is being used as the PAN token. An independent solution is needed long term.
        /// This is challenging because we (obviously) don't want to store the PAN and security review indicated we can't store a hash either.
        /// </remarks>
        public string PanToken { get; set; }
    }
}