//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents operations on Card objects within the data store.
    /// </summary>
    public class CardOperations : CommerceOperations, ICardOperations
    {
        /// <summary>
        /// Adds the card in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
// AddOrReactivateCard
        public ResultCode AddCard()
        {
            ResultCode result;

            // Get the user and card objects from the context.
            Guid globalUserId = (Guid)Context[Key.GlobalUserId];
            Card card = (Card)Context[Key.Card];

            // Get the token assigned to the card by the partner.
            Partner partner = Partner.None;
            switch (card.CardBrand)
            {
                case CardBrand.AmericanExpress:
                    partner = Partner.Amex;
                    break;
                case CardBrand.MasterCard:
                    partner = Partner.MasterCard;
                    break;
                case CardBrand.Visa:
                    partner = Partner.Visa;
                    break;
            }
            string partnerToken = String.Empty;
            PartnerCardInfo partnerCardInfo = card.PartnerCardInfoList.FirstOrDefault(partnerCard => partnerCard.PartnerId == partner);
            if (partnerCardInfo != null)
            {
                partnerToken = partnerCardInfo.PartnerCardId;
            }
             
            // Get the token assigned to the card by First Data.
            // NOTE: This will only be necessary until Visa's WSDL v7 is implemented.
            string fdcToken = null;
            PartnerCardInfo fdcCardInfo = card.PartnerCardInfoList.FirstOrDefault(fdcCard => fdcCard.PartnerId == Partner.FirstData);
            if (fdcCardInfo != null)
            {
                fdcToken = fdcCardInfo.PartnerCardId;
            }

            // Add the card to the data layer.
            result = SqlProcedure("AddOrReactivateCard",
                                  new Dictionary<string, object>
                                  {
                                      { "@globalUserId", globalUserId },
                                      { "@cardBrand", card.CardBrand },
                                      { "@lastFourDigits", card.LastFourDigits },
                                      { "@partnerToken", partnerToken },
                                      { "@fdcToken", fdcToken }
                                  },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        card.Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardId"));
                    }
                });

            return result;
        }

//RetrieveCardById
        /// <summary>
        /// Retrieves the card with the ID in the context.
        /// </summary>
        /// <returns>
        /// * The specified card if it exists.
        /// * Else returns null.
        /// </returns>
        public Card RetrieveCard()
        {
            Card result = null;

            // Look for the card in the data store.
            int cardId = (int)Context[Key.CardId];
            SqlProcedure("GetCardById",
                         new Dictionary<string, object>
                         {
                             { "@cardId", cardId }
                         },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        // Populate a new Card object with information from the data store.
                        result = new Card
                        {
                            Id = cardId,
//TODO: To what extent can we transition to only using the int ID? And of course, want to move away from list of partners and add active, etc..
                            GlobalUserId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalUserID")),
                            CardBrand = (CardBrand)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardBrand")),
                            LastFourDigits = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits"))//,
//                            Active = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Active"))
                        };

//TODO: Remove these when refactoring DataModels
                        result.Expiration = new DateTime(9999, 12, 31);
                        result.NameOnCard = String.Empty;
                        result.RewardPrograms = RewardPrograms.EarnBurn;

                        // Determine the card's partner company and add an appropriate PartnerCardInfo object.
                        string partnerToken = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerToken"));
                        if (String.IsNullOrWhiteSpace(partnerToken) == false)
                        {
                            Partner partner = Partner.None;
                            switch(result.CardBrand)
                            {
                                case CardBrand.AmericanExpress:
                                    partner = Partner.Amex;
                                    break;
                                case CardBrand.MasterCard:
                                    partner = Partner.MasterCard;
                                    break;
                                case CardBrand.Visa:
                                    partner = Partner.Visa;
                                    break;
                            }
                            result.PartnerCardInfoList.Add(new PartnerCardInfo
                            {
                                PartnerId = partner,
                                PartnerCardId = partnerToken,
                                PartnerCardSuffix = "00"
                            });
                        }

                        // Get the FDC token if one was returned and add an FDC PartnerCardInfo object if necessary.
                        // NOTE: This will only be necessary until Visa's WSDL v7 is implemented.
                        string fdcToken = String.Empty;
                        int fdcTokenColumnId = sqlDataReader.GetOrdinal("FDCToken");
                        if (sqlDataReader.IsDBNull(fdcTokenColumnId) == false)
                        {
                            fdcToken = sqlDataReader.GetString(fdcTokenColumnId);
                        }
                        if (String.IsNullOrWhiteSpace(fdcToken) == false)
                        {
                            result.PartnerCardInfoList.Add(new PartnerCardInfo
                            {
                                PartnerId = Partner.FirstData,
                                PartnerCardId = fdcToken,
                                PartnerCardSuffix = "00"
                            });
                        }
                    }
                });

            if (result != null)
            {
                Context.Log.Verbose("GetCardById retrieved the specified Card.");
            }
            else
            {
                Context.Log.Verbose("GetCardById could not find the specified Card.");
            }

            return result;
        }

// RetrieveCardsByUser
        /// <summary>
        /// Retrieves the list of cards that belong to the user in the context.
        /// </summary>
        /// <returns>
        /// The list of cards that belong to the user in the context.
        /// </returns>
        public IEnumerable<InternalCard> RetrieveCards()
        {
            List<InternalCard> result = new List<InternalCard>();

            int userId = ((User)Context[Key.User]).Id;

            SqlProcedure("GetCardsByUser",
                         new Dictionary<string, object>
                         {
                             { "@userId", userId }
                         },
                (sqlDataReader) =>
                {
                    while (sqlDataReader.Read() == true)
                    {
                        result.Add(new InternalCard
                        {
                            GlobalUserId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalUserID")),
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardId")),
                            CardBrand = (CardBrand)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardBrand")),
                            LastFourDigits = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastFourDigits")),
                            NumberOfCardholders = 1
                        });
                    }
                });

            Context.Log.Verbose("GetCardById retrieved {0} cards for the specified User.", result.Count);

            return result;
        }

//DeactivateCard
        /// <summary>
        /// Removes the card from the specified reward programs.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        public ResultCode RemoveCardFromRewardPrograms()
        {
            User user = (User)Context[Key.User];
            return SqlProcedure(
                "DeactivateCard",
                new Dictionary<string, object>
                {
                    { "@userId", user.Id },
                    { "@cardId", ((Card)Context[Key.Card]).Id }
                });
        }

// RetrieveCardByPartnerToken
        /// <summary>
        /// Retrieve the list of cards that are currently active
        /// and have same partner card id.
        /// </summary>
        /// <returns>
        /// The list of cards
        /// </returns>
        public IEnumerable<InternalCard> RetrieveCardsByPartnerCardId()
        {
            List<InternalCard> result = new List<InternalCard>();

            string partnerCardId = (string)Context[Key.PartnerCardId];
            CardBrand cardBrand = (CardBrand)Context[Key.CardBrand];
            SqlProcedure("GetCardByPartnerToken",
                         new Dictionary<string, object>
                         {
                             { "@cardBrand", cardBrand },
                             { "@partnerToken", partnerCardId }
                         },

                (sqlDataReader) =>
                {
                    while (sqlDataReader.Read() == true)
                    {
                        result.Add(new InternalCard
                        {
                            Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("CardId")),
                            GlobalUserId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalUserID"))
                        });
                    }
                });

            Context.Log.Verbose("GetCardByPartnerToken retrieved {0} cards for the specified PartnerCardId.", result.Count);

            return result;
        }

//RetrieveUnfilteredMasterCards
        /// <summary>
        /// Retrieves the PartnerCardIds for any MasterCard that has not yet been filtered.
        /// </summary>
        /// <returns>
        /// The list of PartnerCardIds for MasterCard cards that have not been filtered.
        /// </returns>
        public IEnumerable<string> RetrieveUnfilteredMasterCards()
        {
            List<string> result = new List<string>();

            SqlProcedure("GetUnfilteredMasterCards",
                         null,
                (sqlDataReader) =>
                {
                    while (sqlDataReader.Read() == true)
                    {
                        result.Add(sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerToken")));
                    }
                });

            Context.Log.Verbose("GetUnfilteredMasterCards retrieved {0} unfiltered MasterCard cards.", result.Count);

            return result;
        }

//MarkMasterCardsAsFiltered
        /// <summary>
        /// Adds the date on which the specified list of PartnerCardIds for MasterCard cards was filtered within MasterCard's system.
        /// </summary>
        /// <param name="partnerCardIds">
        /// The list of PartnerCardIds for MasterCard cards whose filtered date to add.
        /// </param>
        /// <returns>
        /// The ResultCode corresponding to the result of the data store call.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter partnerCardIds cannot be null.
        /// </exception>
        public ResultCode AddFilteredMasterCards(IEnumerable<string> partnerCardIds)
        {
            if (partnerCardIds == null)
            {
                throw new ArgumentNullException("partnerCardIds", "Parameter partnerCardIds cannot be null.");
            }

            ResultCode result = ResultCode.Success;

            using (DataTable partnerTokensTable = new DataTable("ListOfStrings"))
            {
                // Build the PartnerTokens table parameter.
                partnerTokensTable.Locale = CultureInfo.InvariantCulture;
                partnerTokensTable.Columns.Add("Id", typeof(string));
                foreach (string partnerToken in partnerCardIds)
                {
                    partnerTokensTable.Rows.Add(partnerToken);
                }

                // Mark cards with specified partner tokens as having been sent to MasterCard for filtering.
                result = SqlProcedure("MarkMasterCardsAsFiltered",
                                      new Dictionary<string, object>
                                      {
                                          { "@partnerTokens", partnerTokensTable }
                                      });
            }

            return result;
        }

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
        /// <remarks>
        /// This method is deprecated but not yet removable. When Visa WSDL 6 implementation is complete, we should no longer need to build PTS files to process Visa statement
        ///  credits. At that time, this entire codepath can be removed.
        /// </remarks>
        [Obsolete] // BUT DO NOT REMOVE YET
        public string RetrieveCardTokenForPartner(Partner fromPartner, string fromPartnerCardToken, Partner toPartner)
        {
            string resultToken = null;

            SqlProcedure("GetCardTokenForAnotherPartner",
                         new Dictionary<string, object>
                         {
                             { "@partnerIdFrom", (int)fromPartner },
                             { "@partnerIdTo", (int)toPartner},
                             { "@cardTokenFrom", fromPartnerCardToken}
                         },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read())
                    {
                        resultToken = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerCardId"));
                    }
                });

            if (resultToken != null)
            {
                Context.Log.Verbose("GetCardTokenForAnotherPartner retrieved the specified Card Token.");
            }
            else
            {
                Context.Log.Verbose("GetCardTokenForAnotherPartner could not find the specified Card.");
            }

            return resultToken;

        }


        /// <summary>
        /// Attempts to retrieve the Visa partner card ID for the card in the context.
        /// </summary>
        /// <returns>
        /// * The Visa partner card ID if successful.
        /// * Else returns null.
        /// </returns>
        /// <remarks>
        /// This method is deprecated but not yet removable. When Visa WSDL 7 implementation is complete, we should no longer need FDC tokens to attempt to determine the Visa
        ///  token for a card that had been previously registered. At that time, this entire codepath can be removed.
        /// </remarks>
        [Obsolete] // BUT DO NOT REMOVE YET
        public string RetrieveVisaPartnerCardId()
        {
            string result = null;

            Card card = (Card)Context[Key.Card];
            if (String.IsNullOrWhiteSpace(card.PanToken) == false)
            {
                SqlProcedure("GetVisaPartnerCardId",
                             new Dictionary<string, object>
                         {
                             { "@panToken", card.PanToken }
                         },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        result = sqlDataReader.GetString(sqlDataReader.GetOrdinal("VisaPartnerCardId"));
                    }
                });
            }

            if (String.IsNullOrWhiteSpace(result) == true)
            {
                Context.Log.Verbose("Unable to find a Visa partner card ID for the card being registered.");
            }

            return result;
        }
    }
}