//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Encapsulates calls to partners necessary to register a deal.
    /// </summary>
    public class RegisterDealInvoker
    {
        /// <summary>
        /// Registers the specified deal to all applicable partners.
        /// </summary>
        /// <param name="context">
        /// The context object containing information on the deal to register.
        /// </param>
        public RegisterDealInvoker(CommerceContext context)
        {
            // Save the context.
            Context = context;

            // Initialize partner operations objects.
            Partners = new ICommercePartner[4]
            {
                new FirstData(Context),
                new Amex(Context), 
                new Visa(Context),
                new MasterCard(Context)
            };
        }

        /// <summary>
        /// Invokes RegisterDeal with each currently registered partner.
        /// </summary>
        public async Task<ResultCode> InvokeAndReturn()
        {
            ResultCode result = ResultCode.Success;

            // Determine the partner(s) with whom to register the deal.
            Deal deal = (Deal)Context[Key.Deal];
            PartnerDealInfo amexPartnerDealInfo = null;
            PartnerDealInfo firstDataPartnerDealInfo = null;
            PartnerDealInfo visaPartnerDealInfo = null;
            PartnerDealInfo masterCardPartnerDealInfo = null;
            foreach (PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
            {
                switch (partnerDealInfo.PartnerId)
                {
                    case Partner.Amex:
                        amexPartnerDealInfo = partnerDealInfo;
                        break;
                    case Partner.FirstData:
                        firstDataPartnerDealInfo = partnerDealInfo;
                        break;
                    case Partner.Visa:
                        visaPartnerDealInfo = partnerDealInfo;
                        break;
                    case Partner.MasterCard:
                        masterCardPartnerDealInfo = partnerDealInfo;
                        break;
                }
            }

            // Invoke the register deal operation for each applicable partner.
            bool isError = false;
            bool isQueued = false;
            foreach (ICommercePartner partner in Partners)
            {
                if ((partner is FirstData && firstDataPartnerDealInfo != null) ||
                    (partner is Amex && amexPartnerDealInfo != null) ||
                    (partner is Visa && visaPartnerDealInfo != null) ||
                    (partner is MasterCard && masterCardPartnerDealInfo != null))
                {
                    string partnerName = partner.GetType().Name;
                    Context.Log.Verbose("Registering deal with partner {0}.", partnerName);

                    ResultCode partnerResult = ResultCode.None;
                    try
                    {
                        partnerResult = await partner.RegisterDealAsync();
                    }
                    catch(Exception ex)
                    {
                        Context.Log.Error("Register deal call to partner {0} ended with an error.", ex, partnerName);
                    }

                    // capture partner results
                    switch (partnerResult)
                    {
                        case ResultCode.Success:
                            // No update to overall result is necessary.
                            break;
                        case ResultCode.JobQueued:
                            isQueued = true;
                            break;
                        case ResultCode.UnknownError:
                            isError = true;
                            break;
                        default:
                            throw new InvalidOperationException("Call to partner invoker returned ResultCode.None.");
                    }
                }
            }

            // Update overall result from result of call to partner.
            // if one of the partner says job queued, the entire result should say job queued
            // and if one partner had an error, everything had an error.
            if (isError)
            {
                result = ResultCode.UnknownError;
            }
            else if(isQueued)
            {
                result = ResultCode.JobQueued;
            }

            return result;
        }
        
        /// <summary>
        /// Gets or sets the context object containing information on the deal to register.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the Partners for which this operation applies.
        /// </summary>
        private ICommercePartner[] Partners { get; set; }
    }
}