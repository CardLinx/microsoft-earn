//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using OfferManagement.DataModel.Partners.Visa;
using services.visa.com.realtime.realtimeservice.datacontracts.v6;
using System.Collections.Generic;
using System.Linq;
using OfferManagement.DataModel;
using Utilities;

namespace OfferManagement.VisaClient
{
    public static class VisaOfferExtensions
    {
        public static CreateOfferRequest GetCreateOfferRequest(this VisaOffer offer, Merchant merchant, string visaCommunityCode)
        {
            var offerGuid = Guid.Parse(offer.Id);
            var offerId = offerGuid.ToByteArray().ToBase62();

            var request = new CreateOfferRequest();
            request.CommunityCode = visaCommunityCode;
            request.Name = offer.Name;
            //request.Description = offer.Name;
            request.CampaignName = offer.CampaignName;
            request.StartDate = RemoveTime(offer.StartDate);
            request.EndDate = RemoveTime(offer.EndDate);
            request.TemplateName = offer.Template;
            request.ExternalOfferId = offerId;


            var visaPayments = merchant.GetVisaPayments();
            var events = new List<Event>();
            
            foreach (var payment in visaPayments)
            {
                var mid = payment.PaymentMids[MerchantConstants.VisaMid];
                var sid = payment.PaymentMids[MerchantConstants.VisaSid];

                EventField[] eventFields =
                {
                    new EventField
                    {
                        Name = "VisaMerchantId",
                        Value = mid
                    },

                    new EventField
                    {
                        Name = "VisaStoreId",
                        Value = sid
                    }

                    //new EventField
                    //{
                    //    Name = "MerchantCountryCode",
                    //    Value = offer.CountryCode
                    //}
                };

                var offerEvent = new Event
                {
                    EventFields = eventFields

                };

                events.Add(offerEvent);
            }

            request.Events = events.ToArray();
            return request;
        }


        public static void UpdateDefaultValues (this VisaOffer offer, Merchant merchant)
        {
            /*
            if (string.IsNullOrEmpty(offer.Name))
            {
                offer.Name = merchant.Name;
            }

            if (string.IsNullOrEmpty(offer.CampaignName))
            {
                offer.CampaignName = "Rewards Network";
            }
            */

            if (string.IsNullOrEmpty(offer.CountryCode))
            {
                offer.CountryCode = "UNITED STATES";
            }

            if (string.IsNullOrEmpty(offer.Template))
            {
                offer.Template = "MSNCGMG";
            }
        }


        public static void ValidateOffer(this VisaOffer offer, Merchant merchant)
        {
            if (string.IsNullOrEmpty(offer?.Id) || string.IsNullOrEmpty(offer.Name) || string.IsNullOrEmpty(offer.CampaignName) || string.IsNullOrEmpty(offer.Template) || string.IsNullOrEmpty(offer.CountryCode))
            {
                throw new Exception("Offer Id, Name, CampaignName, Template, CountryCode are required fields");
            }

            Guid result;
            if (!Guid.TryParse(offer.Id, out result))
            {
                throw new Exception("Offer Id should be a guid");
            }

            if (offer.StartDate == DateTime.MinValue || offer.EndDate == DateTime.MinValue)
            {
                throw new Exception("Offer StartDate or EndDate not correct");
            }

            if (offer.StartDate > offer.EndDate)
            {
                throw new Exception("Offer StartDate cannot be greater than EndDate");
            }

            if (merchant == null || merchant.Payments == null || !merchant.Payments.Any())
            {
                throw new Exception("Missing merchant mids and sids");
            }


            var visaPayments = merchant.GetVisaPayments();
            if (!visaPayments.Any())
            {
                throw new Exception("Unable to find any non empty mid and sid pair");
            }
        }


        public static string GetOfferInfo(this VisaOffer offer, Merchant merchant)
        {
            var visaPayments = merchant.GetVisaPayments();
            var mids = string.Join(",", visaPayments.Select(p => p.PaymentMids[MerchantConstants.VisaMid] + ":" + p.PaymentMids[MerchantConstants.VisaSid]).ToArray());
            return string.Format("Campaign:{0} Offer:{1} Mids:{2}", offer.CampaignName, offer.Name, mids);
        }

        private static DateTime RemoveTime(DateTime inpuDateTime)
        {
            var newDateTime = new DateTime(inpuDateTime.Year, inpuDateTime.Month, inpuDateTime.Day);
            return newDateTime;
        }
    }
}