//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Extensions
{
    public static class MerchantRequestExtension
    {
        private SearchMerchantByAttribute_Request GetRequest(MerchantRequest merchant)
        {
            var request = new SearchMerchantByAttribute_Request();
            request.CommunityCode = config.VisaCommunityCode;
            request.MerchantName = merchant.Name;
            //country code hard coded to US - should change if we go international
            request.MerchantCountryCode = CountryCodeUS;

            request.MerchantAddress = merchant.Address;
            request.MerchantCity = merchant.City;
            request.MerchantPostalCode = merchant.Zip;
            request.MerchantState = merchant.State;

            return request;
        }

        private string GetMerchantInfo(MerchantRequest merchant)
        {
            return string.Format("Merchant Name:{0} Address:{1} City:{2} State:{3} Zip:{4}", merchant.Name, merchant.Address, merchant.City, merchant.State, merchant.Zip);
        }

    }
}