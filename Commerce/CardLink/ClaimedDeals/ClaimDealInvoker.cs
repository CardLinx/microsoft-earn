//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Linq;
    using Lomo.Commerce.DataModels;
    using System.Threading.Tasks;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Configuration;

    /// <summary>
    /// Encapsulates calls to partners necessary to claim a deal.
    /// </summary>
    public class ClaimDealInvoker
    {
        /// <summary>
        /// Claims the specified deal for redemption with the specified card.
        /// </summary>
        /// <param name="context">
        /// The context object containing information on the deal to claim.
        /// </param>
        public ClaimDealInvoker(CommerceContext context)
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
                ResultCode resultCode = ResultCode.None;

                // Determine the partner through whom the deal will be claimed.
                ICommercePartner partner = SelectPartner();

                if (partner == null)
                {
                    resultCode = ResultCode.UnregisteredDeal;
                }
                else
                {
                    resultCode = await partner.ClaimDealAsync().ConfigureAwait(false);
                }

                // Check the invoker's response.
                if (resultCode == ResultCode.None)
                {
                    throw new InvalidOperationException("Call to partner invoker returned ResultCode.None.");
                }

                // Return result and updated objects to the caller.
                ClaimDealConcluder claimDealConcluder = new ClaimDealConcluder(Context);
                claimDealConcluder.Conclude(resultCode);
            }
            catch(Exception ex)
            {
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Selects the partner through whom to claim the deal.
        /// </summary>
        /// <returns>
        /// The partner through whom the deal will be claimed.
        /// </returns>
        /// <remarks>
        /// * If the card is an Amex and the deal contains Amex info, claim the deal with Amex.
        /// * If the card is a Visa
        ///     * If the deal contains First Data info, claim the deal with First Data.
        ///     * Else if the deal contains Visa info and Visa is enabled, claim the deal with Visa.
        /// * If the card is a MasterCard
        ///     * If the deal contains First Data info, claim the deal with First Data.
        ///     * Else if the deal contains MasterCard info and MasterCard is enabled, claim the deal with MasterCard.
        /// </remarks>
        private ICommercePartner SelectPartner()
        {
            ICommercePartner result = null;

            // Find partner information within the deal.
            Card card = (Card)Context[Key.Card];
            Deal deal = (Deal)Context[Key.Deal];
            PartnerDealInfo amexDealInfo = null;
            PartnerDealInfo firstDataPartnerDealInfo = null;
            PartnerDealInfo visaDealInfo = null;
            PartnerDealInfo masterCardDealInfo = null;
            foreach(PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
            {
                switch (partnerDealInfo.PartnerId)
                {
                    case Partner.Amex:
                        amexDealInfo = partnerDealInfo;
                        break;
                    case Partner.FirstData:
                        firstDataPartnerDealInfo = partnerDealInfo;
                        break;
                    case Partner.Visa:
                        visaDealInfo = partnerDealInfo;
                        break;
                    case Partner.MasterCard:
                        masterCardDealInfo = partnerDealInfo;
                        break;
                }
            }

            // Determine the partner with whom to claim the deal.
            switch (card.CardBrand)
            {
                case CardBrand.AmericanExpress:
                    if (amexDealInfo != null)
                    {
                        result = new Amex(Context);
                    }
                    break;
                case CardBrand.Visa:
                    if (firstDataPartnerDealInfo != null)
                    {
                        result = new FirstData(Context);
                    }
                    else if (visaDealInfo != null)
                    {
                        result = new Visa(Context);
                    }
                    break;
                case CardBrand.MasterCard:
                    if (firstDataPartnerDealInfo != null)
                    {
                        result = new FirstData(Context);
                    }
                    else if (masterCardDealInfo != null)
                    {
                        result = new MasterCard(Context);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context object containing information on the deal to claim.
        /// </summary>
        private CommerceContext Context { get; set; }
    }
}