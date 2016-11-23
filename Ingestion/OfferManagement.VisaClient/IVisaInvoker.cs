//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Threading.Tasks;
using OfferManagement.DataModel;
using OfferManagement.DataModel.Partners.Visa;

namespace OfferManagement.VisaClient
{
    public interface IVisaInvoker
    {
        string GetProxyInfo();

        Task<MerchantSearchType> SearchMerchantDetailsByAttributeAsync(Merchant merchant, bool isNational = true);
        Task<int> CreateOfferAsync(Merchant merchant, VisaOffer offer);

        Task<string> UpdateOfferAsync(string offerId, string action = "Reject");

        Task<bool> IsOfferEnabled(int offerId);
    }
}