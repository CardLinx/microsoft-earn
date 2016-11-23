//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataModels;
    
    /// <summary>
    /// Generator for FirstData Offer Ids
    /// </summary>
    public static class PartnerOfferIdGeneratorFactory
    {
        public static string GenerateOfferId(PartnerDealInfo partnerDealInfo, CommerceContext context)
        {
            if (partnerDealInfo == null)
            {
                throw new ArgumentNullException("partnerDealInfo","partnerDealInfo cannot be null");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context", "context cannot be null");
            }

            string result;
            switch (partnerDealInfo.PartnerId)
            {
                case Partner.FirstData:
                    result = FirstDataOfferIdGenerator.GenerateNewOfferId(context);
                    break;
                case Partner.Amex:
                    result =  AmexOfferIdGenerator.GenerateNewOfferId(context).ToString();
                    break;
                case Partner.Visa:
                    result = VisaOfferIdGenerator.GenerateNewOfferId(context);
                    break;
                case Partner.MasterCard:
                    result = MasterCardOfferIdGenerator.GenerateNewOfferId(context);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported partner specified.");
            }

            return result;
        }
    }
}