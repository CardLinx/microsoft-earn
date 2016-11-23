//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents a card stored in the data store.
    /// </summary>
    public class Card : InternalCard
    {
        /// <summary>
        /// Initializes a new instance of the Card class for serialization.
        /// </summary>
        public Card()
        {
            PartnerCardInfoList = new Collection<PartnerCardInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the Card class from the specified UserId.
        /// </summary>
        /// <param name="userId">
        /// ID of the user to whom the card belongs.
        /// </param>
        public Card(Guid userId) : base(userId)
        {
            PartnerCardInfoList = new Collection<PartnerCardInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the Card class, using the fields from the specified other Card.
        /// </summary>
        /// <param name="card">
        /// The other Card whose fields to copy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter card cannot be null.
        /// </exception>
        public Card(Card card) : base(card)
        {
            PartnerCardInfoList = new Collection<PartnerCardInfo>();
            foreach (PartnerCardInfo partnerCardInfo in card.PartnerCardInfoList)
            {
                PartnerCardInfoList.Add(new PartnerCardInfo(partnerCardInfo));
            }
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
            return base.Equals(obj) == true &&
                   PartnerCardInfoList.Except(card.PartnerCardInfoList).Any() == false &&
                   card.PartnerCardInfoList.Except(PartnerCardInfoList).Any() == false;
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
            int result = base.GetHashCode();

            foreach (PartnerCardInfo partnerCardInfo in PartnerCardInfoList)
            {
                result += partnerCardInfo.GetHashCode();
            }
                   
            return result;
        }

        /// <summary>
        /// Gets or sets a list of information about the card from partners.
        /// </summary>
        public Collection<PartnerCardInfo> PartnerCardInfoList { get; private set; }
    }
}