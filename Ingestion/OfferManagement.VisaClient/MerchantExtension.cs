//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lomo.Core.Extensions;
using OfferManagement.BingMapClient;
using OfferManagement.DataModel;
using services.visa.com.realtime.realtimeservice.datacontracts.mob.v6;

namespace OfferManagement.VisaClient
{
    public static class MerchantExtension
    {
        private const string CountryCodeUS = "840";

        public static SearchMerchantByAttribute_Request GetSearchMerchantByAttributeRequest(this Merchant merchant, string visaCommunityCode)
        {
            var request = new SearchMerchantByAttribute_Request();
            request.CommunityCode = visaCommunityCode;
            request.MerchantName = merchant.Name;
            //country code hard coded to US - should change if we go international
            request.MerchantCountryCode = CountryCodeUS;

            var location = merchant.Location;

            request.MerchantAddress = location.Address;
            request.MerchantCity = location.City;
            request.MerchantPostalCode = location.Zip;
            request.MerchantState = location.State;

            return request;
        }
        
        public static async Task ValidateMerchant(this Merchant merchant)
        {
            if (string.IsNullOrEmpty(merchant?.Name))
            {
                throw new Exception("Merchant Name is required field");
            }

            var location = merchant.Location;

            if (location == null)
            {
                throw new Exception("Merchant location is required field");
            }
            
            if (string.IsNullOrEmpty(location.Zip))
            {
                if (string.IsNullOrEmpty(location.Address) || string.IsNullOrEmpty(location.City) || string.IsNullOrEmpty(location.State))
                {
                    throw new Exception("Merchant Zip is required field");
                }

                var response = await Geocoding.GetLocationAsync(location.Address, location.State, location.Zip, location.City);
                if (response?.address != null)
                {
                    if (!string.IsNullOrEmpty(response.address.postalCode))
                    {
                        location.Zip = response.address.postalCode;
                    }
                    else
                    {
                        throw new Exception("Merchant Zip is required field");
                    }

                }
            }
        }
       

        public static string GetMerchantInfo(this Merchant merchant)
        {
            var location = merchant.Location;
            return string.Format("Merchant Name:{0} Address:{1} City:{2} State:{3} Zip:{4}", merchant.Name, location.Address, location.City, location.State, location.Zip);
        }

        public static async Task<Merchant> GetMerchantForSearch(this Merchant merchant, MerchantSearchType searchType)
        {
            if (searchType == MerchantSearchType.SearchUsingParametersSupplied)
            {
                return merchant;
            }

            if (searchType == MerchantSearchType.SearchUsingWildCardInName)
            {
                return merchant.GetMerchantForSearchUsingWildCardInName();
            }

            if (searchType == MerchantSearchType.SearchWithoutStreetAddress)
            {
                return merchant.GetMerchantForSearchWithoutStreetAddress();
            }

            if (searchType == MerchantSearchType.SearchUsingBingAddress)
            {
                return await merchant.GetMerchantForSearchUsingBingAddress();
            }

            return null;
        }

        public static Merchant Clone(this Merchant merchant)
        {
            var location = merchant.Location;
            var clone = new Merchant
            {
                Name = merchant.Name,
                Location = new Location
                {
                    Address = location.Address,
                    City = location.City,
                    State = location.State,
                    Zip = location.Zip
                }
            };

            return clone;
        }

        public static IList<Payment> GetVisaPayments(this Merchant merchant)
        {
            var visaPayments = merchant.Payments.Where(p => p.Processor == PaymentProcessor.Visa && p.PaymentMids.ContainsKey(MerchantConstants.VisaMid) && p.PaymentMids.ContainsKey(MerchantConstants.VisaSid)).ToList();
            return visaPayments;
        }
        

        public static OnboardMerchants_Request GetOnboardMerchantsRequest(this Merchant merchant)
        {
            var request = new OnboardMerchants_Request { CommunityCode = VisaConstants.CommunityCodeClLevel};

            var midName = merchant.GetExtendedAttributeValue(MerchantConstants.VisaMidName);
            var sidName = merchant.GetExtendedAttributeValue(MerchantConstants.VisaSidName);

            if (string.IsNullOrEmpty(midName) && !string.IsNullOrEmpty(sidName))
            {
                midName = sidName;
            }
            else if (string.IsNullOrEmpty(sidName) && !string.IsNullOrEmpty(midName))
            {
                sidName = midName;
            }

            if (string.IsNullOrEmpty(midName))
            {
                midName = merchant.Name;
            }

            if (string.IsNullOrEmpty(sidName))
            {
                sidName = merchant.Name;
            }

            if (string.IsNullOrEmpty(midName) || string.IsNullOrEmpty(sidName))
            {
                throw new Exception("Unable to find VisaMidName or VisaSidName for merchant");
            }

            var visaPayments = merchant.GetVisaPayments();
            
            var list = new List<OnboardMerchantDetails>();
            foreach (var payment in visaPayments)
            {
                var mid = payment.PaymentMids[MerchantConstants.VisaMid];
                var sid = payment.PaymentMids[MerchantConstants.VisaSid];
                
                var onboardMerchantDetails = new OnboardMerchantDetails
                {
                    VisaMerchantId = Convert.ToDecimal(mid),
                    VisaMerchantName = midName,
                    VisaStoreId = Convert.ToDecimal(sid),
                    VisaStoreName = sidName
                };

                list.Add(onboardMerchantDetails);
            }
            
            OnboardMerchantDetails[] onboardMerchantDetailsList = list.ToArray();
            request.OnboardMerchants = onboardMerchantDetailsList;
            return request;
        }


        private static string GetExtendedAttributeValue(this Merchant merchant, string attributeKey)
        {
            if (merchant.ExtendedAttributes != null && merchant.ExtendedAttributes.ContainsKey(attributeKey))
            {
                return merchant.ExtendedAttributes[attributeKey];
            }

            return null;
        }

        private static Merchant GetMerchantForSearchUsingWildCardInName(this Merchant merchant)
        {
            var tokens = merchant.Name.TokenizeUsingWhiteSpace();
            var tokenIndex = 0;
            var token = tokens[tokenIndex];
            const int minTokenChar = 7;

            while (token.Length < minTokenChar && tokenIndex < tokens.Count)
            {
                ++tokenIndex;
                token = token + " " + tokens[tokenIndex];
            }

            token = token + "*";

            var cloneMerchant = merchant.Clone();
            cloneMerchant.Name = token;
            return cloneMerchant;
        }

        private static Merchant GetMerchantForSearchWithoutStreetAddress(this Merchant merchant)
        {
            var cloneMerchant = merchant.Clone();
            cloneMerchant.Location.Address = string.Empty;
            return cloneMerchant;
        }

        private static async Task<Merchant> GetMerchantForSearchUsingBingAddress(this Merchant merchant)
        {
            var location = merchant.Location;
            var response = await Geocoding.GetLocationAsync(location.Address, location.State, location.Zip, location.City);
            if (response != null && response.address != null)
            {
                var address = response.address;
                var cloneMerchant = merchant.Clone();
                cloneMerchant.Location.Address = address.addressLine;
                return cloneMerchant;
            }

            return null;
        }
    }
}