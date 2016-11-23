//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Encapsulates calls to partners necessary to add a card.
    /// </summary>
    public class AddCardInvoker
    {
        /// <summary>
        /// Adds the specified card for the specified user.
        /// </summary>
        /// <param name="context">
        /// The context object containing information on the card to add.
        /// </param>
        public AddCardInvoker(CommerceContext context)
        {
            // Save the context.
            Context = context;
        }

        /// <summary>
        /// Invokes AddCard with each currently registered partner.
        /// </summary>
        public async Task Invoke()
        {
            try
            {
                ResultCode resultCode = ResultCode.Created;

                NewCardInfo newCardInfo = (NewCardInfo)Context[Key.NewCardInfo];
                Card card = (Card)Context[Key.Card];
                List<ICommercePartner> partners = new List<ICommercePartner>();

                //partners.Add(new FirstData(Context));
                CardBrand cardBrand = (CardBrand)Enum.Parse(typeof(CardBrand), newCardInfo.CardBrand);
                switch (cardBrand)
                {
                    case CardBrand.AmericanExpress:
                        partners.Add(new Amex(Context));
                        break;
                    case CardBrand.MasterCard:
                        partners.Add(new MasterCard(Context));
                        break;
                    case CardBrand.Visa:
                        partners.Add(new Visa(Context));
                        break;
                }

                foreach (ICommercePartner partner in partners)
                {
                    string partnerName = partner.GetType().Name;
                    Context.Log.Verbose("Adding card to partner {0}.", partnerName);

                    ResultCode partnerResult = ResultCode.None;
                    try
                    {
                        partnerResult = await partner.AddCardAsync();
                    }
                    catch (Exception ex)
                    {
                        Context.Log.Error("Add card call to partner {0} ended with an error.", ex, partnerName);
                    }

                    // Update overall result from result of call to partner.
                    switch (partnerResult)
                    {
                        case ResultCode.Created:
                            // At this time, FirstData PartnerCardId is used as the PanToken for the card.
                            if (partner is FirstData)
                            {
                                PartnerCardInfo firstDataInfo = card.PartnerCardInfoList.SingleOrDefault(info => info.PartnerId == Partner.FirstData);
                                if (firstDataInfo != null)
                                {
                                    card.PanToken = firstDataInfo.PartnerCardId;
                                }
                            }
                            break;
                        case ResultCode.InvalidCard:
                            resultCode = ResultCode.InvalidCard;
                            break;
                        case ResultCode.UnknownError:
                            resultCode = ResultCode.UnknownError;
                            break;
                        case ResultCode.InvalidCardNumber:
                            resultCode = ResultCode.InvalidCardNumber;
                            break;
                        case ResultCode.CorporateOrPrepaidCardError:
                            resultCode = ResultCode.CorporateOrPrepaidCardError;
                            break;
                        case ResultCode.MaximumEnrolledCardsLimitReached:
                            resultCode = ResultCode.MaximumEnrolledCardsLimitReached;
                            break;
                        case ResultCode.CardRegisteredToDifferentUser:
                            resultCode = ResultCode.CardRegisteredToDifferentUser;
                            break;
                        case ResultCode.CardExpired:
                            resultCode = ResultCode.CardExpired;
                            break;

                        default:
                            throw new InvalidOperationException("Call to partner invoker returned ResultCode.None.");
                    }
                }
                AddCardConcluder addCardConcluder = new AddCardConcluder(Context);
                addCardConcluder.Conclude(resultCode);
            }
            catch(Exception ex)
            {
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Gets or sets the context object containing information on the card to add.
        /// </summary>
        private CommerceContext Context { get; set; }
    }
}