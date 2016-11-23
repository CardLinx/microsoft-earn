//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Encapsulates calls to partners necessary to remove a card.
    /// </summary>
    public class RemoveCardInvoker
    {
        /// <summary>
        /// Removes the specified card for the specified user.
        /// </summary>
        /// <param name="context">
        /// The context object containing information on the card to remove.
        /// </param>
        public RemoveCardInvoker(CommerceContext context)
        {
            // Save the context.
            Context = context;
        }

        /// <summary>
        /// Invokes RemoveCardAsync with each currently registered partner.
        /// </summary>
        public async Task Invoke()
        {
            try
            {
                Card card = (Card)Context[Key.Card];

                RewardPrograms rewardPrograms = RewardPrograms.CardLinkOffers;
                if (Context.ContainsKey(Key.RewardProgramType) == true)
                {
                    rewardPrograms = (RewardPrograms)Context[Key.RewardProgramType];
                }

                ResultCode resultCode = ResultCode.Success;

                // Notify the partners of the card removal only if the card is enrolled to the 
                // specified reward program. If the card is enrolled with other reward programs, 
                // we still want to receive notifications from the partners for this card.
                if (card.RewardPrograms == rewardPrograms)
                {
                    List<ICommercePartner> partners = new List<ICommercePartner>();
                    foreach (PartnerCardInfo partnerCardInfo in card.PartnerCardInfoList)
                    {
                        switch (partnerCardInfo.PartnerId)
                        {
                            //case Partner.FirstData:
                            //    partners.Add(new FirstData(Context));
                            //    break;
                            case Partner.Amex:
                                partners.Add(new Amex(Context));
                                break;
                            case Partner.MasterCard:
                                partners.Add(new MasterCard(Context));
                                break;
                            case Partner.Visa:
                                partners.Add(new Visa(Context));
                                break;
                        }
                    }

                    foreach (ICommercePartner partner in partners)
                    {
                        string partnerName = partner.GetType().Name;
                        Context.Log.Verbose("Removing card from partner {0}.", partnerName);

                        ResultCode partnerResult = ResultCode.None;
                        try
                        {
                            partnerResult = await partner.RemoveCardAsync();
                        }
                        catch (Exception ex)
                        {
                            Context.Log.Error("Remove card call to partner {0} ended with an error.", ex, partnerName);
                        }

                        // Update overall result from result of call to partner.
                        switch (partnerResult)
                        {
                            case ResultCode.Success:
                                // No update to overall result is necessary.
                                break;
                            case ResultCode.UnknownError:
                                resultCode = ResultCode.UnknownError;
                                break;
                            default:
                                throw new InvalidOperationException("Call to partner invoker returned ResultCode.None.");
                        }
                    }
                }

                // Return result and updated objects to the caller.
                RemoveCardConcluder removeCardConcluder = new RemoveCardConcluder(Context);
                removeCardConcluder.Conclude(resultCode);
            }
            catch (Exception ex)
            {
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Gets or sets the context object containing information on the card to remove.
        /// </summary>
        private CommerceContext Context { get; set; }
    }
}