//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.Dal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataModel;

    /// <summary>
    /// Repository
    /// </summary>
    public interface IEarnRepository
    {
        Task<IEnumerable<Provider>> GetAllProvidersAsync();

        Task<Provider> GetProviderAsync(string providerId);

        Task<IEnumerable<Merchant>> GetMerchantsForProviderAsync(string providerId);

        Task<Merchant> GetMerchantByIdAsync(string merchantId);

        Task<IEnumerable<Merchant>> GetMerchantByNameAsync(string merchantName);

        Task<DataModel.Offer> GetOfferAsync(string offerId);

        Task<IEnumerable<DataModel.Offer>> GetOffersForProviderAsync(string providerId);

        Task<int> AddMerchantsInBatchAsync(IList<Merchant> merchants);
    }
}