//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using OfferManagement.DataModel;
using OfferManagement.DataModel.Partners.Visa;
using services.visa.com.realtime.realtimeservice.datacontracts.v6;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Visa.Proxy;

namespace OfferManagement.VisaClient
{
    public class VisaInvoker : IVisaInvoker
    {

        internal class SearchInfo
        {
            internal IList<Payment> Payments;
            internal MerchantSearchType SearchType;
        }

        public static readonly IVisaInvoker Instance = new VisaInvoker();
        private readonly IRetryPolicy retryPolicy;

        private static readonly List<MerchantSearchType> MerchantSearchTypesForNational = new List<MerchantSearchType>
        {
            MerchantSearchType.SearchUsingParametersSupplied,
            MerchantSearchType.SearchUsingBingAddress,
            MerchantSearchType.SearchWithoutStreetAddress
        };

        private static readonly List<MerchantSearchType> MerchantSearchTypesForNonNational = new List<MerchantSearchType>
        {
            MerchantSearchType.SearchUsingParametersSupplied,
            MerchantSearchType.SearchUsingBingAddress,
            MerchantSearchType.SearchUsingWildCardInName,
            MerchantSearchType.SearchWithoutStreetAddress
        };

        public VisaInvoker()
        {
            retryPolicy = WaitAndRetryPolicy.Instance;
        }

        public VisaInvoker(IRetryPolicy retryPolicy)
        {
            this.retryPolicy = retryPolicy;
        }

        public string GetProxyInfo()
        {
            var proxy = VisaRtmClientManager.Instance.GetVisaRtmClient();
            return string.Format("Url:{0} ClientCertThumbprint:{1} UserName:{2} Pwd:{3}", proxy.Endpoint.Address.Uri, proxy.ClientCredentials?.ClientCertificate.Certificate.Thumbprint, proxy.ClientCredentials?.UserName.UserName, proxy.ClientCredentials?.UserName.Password);
        }

        public async Task<MerchantSearchType> SearchMerchantDetailsByAttributeAsync(Merchant merchant, bool isNational = true)
        {
            await merchant.ValidateMerchant();
            
            if (merchant.Payments == null)
            {
                merchant.Payments = new List<Payment>();
            }

            var searchInfo = await GetPaymentDetailsByAttributeAsync(merchant, isNational);
            var payments = searchInfo.Payments;

            if (payments != null && payments.Any())
            {
                foreach (var payment in payments)
                {
                    var newPaymentMids = payment.PaymentMids;
                    if (newPaymentMids != null && newPaymentMids.Any())
                    {
                        var newMidAlreadyPresent = false;
                        var existingPayments = merchant.Payments.Where(midInfo => midInfo.Processor == PaymentProcessor.Visa && midInfo.PaymentMids != null).ToList();
                        if (existingPayments.Any())
                        {
                            var newPaymentMidsHashSet = new HashSet<string>(newPaymentMids.Values);
                            foreach (var existingPayment in existingPayments)
                            {
                                var existingPaymentMids = existingPayment.PaymentMids;
                                var existingPaymentMidsHashSet = new HashSet<string>(existingPaymentMids.Values);
                                if (newPaymentMidsHashSet.SetEquals(existingPaymentMidsHashSet))
                                {
                                    newMidAlreadyPresent = true;
                                    break;
                                }
                            }
                        }

                        if (!newMidAlreadyPresent)
                        {
                            merchant.Payments.Add(payment);
                        }
                    }
                }
            }

            return searchInfo.SearchType;
        }
        
        public async Task<int> CreateOfferAsync(Merchant merchant, VisaOffer offer)
        {
            offer.UpdateDefaultValues(merchant);

            offer.ValidateOffer(merchant);

            await OnboardMerchantsAsync(merchant);

            var request = offer.GetCreateOfferRequest(merchant, VisaConstants.CommunityCodeClLevel);
            
            var policy = retryPolicy.Get();

            var response = await policy.ExecuteAsync(
                    async () =>
                    {
                        var client = VisaRtmClientManager.Instance.GetVisaRtmClient();
                        //uncomment to capture using Fiddler
                        //uncommnet line in Fiddler --  oSession["https-Client-Certificate"] = "...VisaProd.cer";
                        /*
                        var binding = client.Endpoint.Binding as BasicHttpBinding;
                        binding.UseDefaultWebProxy = false;
                        binding.ProxyAddress = new Uri("http://127.0.0.1:8888");
                        binding.BypassProxyOnLocal = false;
                        */
                        var response1 = await client.CreateOfferAsync(request);
                        return response1;
                    }
                );
           
            if (response.HasError() || response.OfferId == null || response.OfferId < 0)
            {
                //if (!response.ContainsError(VisaCallErrorConstants.DuplicateOfferName))
                if (!response.ContainsError(VisaCallErrorConstants.OfferAlreadyExisitForGivenMidAndSid))
                {
                    var errorInfoDetail = response.GetErrorInfoDetail();
                    var offerInfo = offer.GetOfferInfo(merchant);
                    var msg = string.Format("OfferId not returned by Visa. VisaErrorInfo:{0} OfferInfo:{1}", errorInfoDetail, offerInfo);
                    throw new Exception(msg);
                }
            }

            var visaOfferId = 0;

            if (response.OfferId != null)
            {
                visaOfferId = response.OfferId.Value;
                if (merchant.ExtendedAttributes == null)
                {
                    merchant.ExtendedAttributes = new Dictionary<string, string>();
                }
                merchant.ExtendedAttributes[MerchantConstants.VisaOfferId] = visaOfferId.ToString();
            }

            return visaOfferId;
        }


        //TODO: - Is offerId visa offerId or our ExternalOfferId
        public async Task<string> UpdateOfferAsync(string offerId, string action = "Reject")
        {

            var request = GetUpdateOfferRequest(offerId, action, VisaConstants.CommunityCodeClLevel);

            var policy = retryPolicy.Get();

            var response = await policy.ExecuteAsync(
                    async () =>
                    {
                        var client = VisaRtmClientManager.Instance.GetVisaRtmClient();
                        var response1 = await client.UpdateOfferAsync(request);
                        return response1;
                    }
                );

            if (response.HasError())
            {
                var errorInfoDetail = response.GetErrorInfoDetail();
                var msg = string.Format("Unable to update offerId:{0}. VisaErrorInfo:{1}", offerId, errorInfoDetail);
                throw new Exception(msg);
            }

            //Message returned is something like - Offer has been rejected successfully.
            return response.StatusMessage;
        }
        
        public async Task<bool> IsOfferEnabled(int offerId)
        {
            var offer = await GetGetOfferDetailsByOfferIdRequest(offerId);
            return offer.IsEnabled;
        }

        private async Task<GetOfferDetailsByOfferIdResponse> GetGetOfferDetailsByOfferIdRequest(int offerId)
        {
            var request = GetGetOfferDetailsByOfferIdRequest(offerId, VisaConstants.CommunityCodeClLevel);

            var policy = retryPolicy.Get();

            var response = await policy.ExecuteAsync(
                    async () =>
                    {
                        var client = VisaRtmClientManager.Instance.GetVisaRtmClient();
                        var response1 = await client.GetOfferDetailsByOfferIdAsync(request);
                        return response1;
                    }
                );

            if (response.HasError())
            {
                var errorInfoDetail = response.GetErrorInfoDetail();
                var msg = string.Format("Unable to get offer for offerId:{0}. VisaErrorInfo:{1}", offerId, errorInfoDetail);
                throw new Exception(msg);
            }

            return response;
        }


        private async Task<SearchInfo> GetPaymentDetailsByAttributeAsync(Merchant merchant, bool isNational = true)
        {
            
            List<Payment> payments = null;
            var errMsg = string.Empty;
            var merchantSearchTypes = isNational ? MerchantSearchTypesForNational : MerchantSearchTypesForNonNational;

            MerchantSearchType finalMerchantSearchType = MerchantSearchType.SearchUsingParametersSupplied;

            foreach (var merchantSearchType in merchantSearchTypes)
            {
                finalMerchantSearchType = merchantSearchType;
                var merchantSearch = await merchant.GetMerchantForSearch(merchantSearchType);

                if (merchantSearch == null)
                {
                    continue;
                }

                if (merchantSearchType == MerchantSearchType.SearchUsingBingAddress || merchantSearchType == MerchantSearchType.SearchWithoutStreetAddress)
                {
                    if(string.Equals(merchantSearch.Location.Address, merchant.Location.Address, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                var request = merchantSearch.GetSearchMerchantByAttributeRequest(VisaConstants.CommunityCodeGroupLevel);

                var policy = retryPolicy.Get();
                var response = await policy.ExecuteAsync(
                    async () =>
                    {
                        //if the city or zip is empty should we still call and let Visa API return the correct error
                        var client = VisaRtmClientManager.Instance.GetVisaRtmClient();
                        var response1 = await client.SearchMerchantDetailsByAttributeAsync(request);
                        return response1;
                    }
                    );

                //var singleMerchantReturnedForEmptyAddress = false;
                if (response?.Merchants != null && response.Merchants.Any())
                {
                    //var merchantAddressNullOrEmpty = string.IsNullOrEmpty(merchantSearch.Address);
                    //singleMerchantReturnedForEmptyAddress = merchantAddressNullOrEmpty && response.Merchants.Length == 1;
                    //if (singleMerchantReturnedForEmptyAddress || !merchantAddressNullOrEmpty)
                    {
                        payments = new List<Payment>();

                        foreach (var visaMerchant in response.Merchants)
                        {
                            //TODO: should we check visaMerchant.IsDeleted
                            //If VISA return multiple merchants do we have to do the matching on our end - this is only problem if our request contains only merchant name and zip code 
                            //like STARBUCKS, 98052. In this case VISA will return multiple locations. However we are assuming we will send VISA complete address. In this case VISA should
                            //not return multiple locations. If it returns multiple MIDS then we can assume they are for the same location. Need to be verified
                            if (visaMerchant.VisaMerchantId > 0 && visaMerchant.VisaStoreId > 0)
                            {
                                var paymentMids = new Dictionary<string, string>
                                {
                                    {MerchantConstants.VisaMid, visaMerchant.VisaMerchantId.ToString(CultureInfo.InvariantCulture)},
                                    {MerchantConstants.VisaSid, visaMerchant.VisaStoreId.ToString(CultureInfo.InvariantCulture)}
                                };

                                var visaPaymentInfo = new Payment
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Processor = PaymentProcessor.Visa,
                                    PaymentMids = paymentMids,
                                    IsActive = true,
                                    LastUpdate = DateTime.UtcNow
                                };

                                payments.Add(visaPaymentInfo);
                            }
                        }

                        //if not national chain and only a single merchant then update merchant mid name and merchant sid name
                        if (!isNational && response.Merchants.Length == 1)
                        {
                            if (merchant.ExtendedAttributes == null)
                            {
                                merchant.ExtendedAttributes = new Dictionary<string, string>();
                            }

                            var visaMerchant = response.Merchants[0];
                            merchant.ExtendedAttributes[MerchantConstants.VisaMidName] = visaMerchant.VisaMerchantName;
                            merchant.ExtendedAttributes[MerchantConstants.VisaSidName] = visaMerchant.VisaStoreName;
                            //await OnboardMerchantsAsync(visaMerchant);
                        }
                    }
                }

                if (payments != null && payments.Any())
                {
                    if (string.IsNullOrEmpty(merchantSearch.Location.Address))
                    {
                        finalMerchantSearchType = MerchantSearchType.SearchWithoutStreetAddress;
                    }

                    break;
                }

                if (response.HasError())
                {
                    var errorInfoDetail = response.GetErrorInfoDetail();
                    errMsg = errMsg + Environment.NewLine + $"SearchType:{merchantSearchType} VisaErrorInfo:{errorInfoDetail}";
                }
                //else if (!singleMerchantReturnedForEmptyAddress)
                //{
                //    errMsg = errMsg + Environment.NewLine + $"SearchType:{merchantSearchType}. Multiple merchants returned for empty address";
                //}
                else
                {
                    errMsg = errMsg + Environment.NewLine + $"SearchType:{merchantSearchType}. No MID information returned by Visa";
                }
            }


            if (payments == null || !payments.Any())
            {
                var merchantInfo = merchant.GetMerchantInfo();
                errMsg = errMsg + "MerchantInfo:" + merchantInfo;
                throw new Exception(errMsg);
            }

            var searchInfo = new SearchInfo
            {
                Payments = payments,
                SearchType = finalMerchantSearchType
            };

            return searchInfo;
        }

        private async Task OnboardMerchantsAsync(Merchant merchant)
        {
            const int statusCodeSuccess = 0;
            var request = merchant.GetOnboardMerchantsRequest();

            var policy = retryPolicy.Get();
            var response = await policy.ExecuteAsync(
                async () =>
                {
                    var client = VisaRtmClientManager.Instance.GetVisaRtmClient();
                    var response1 = await client.OnboardMerchantsAsync(request);
                    return response1;
                }
                );


            var isSuccess = true;
            string errorMsg = null;

            if (response.StatusCode != statusCodeSuccess)
            {
                isSuccess = false;

                if (response.OnboardMerchantsResults != null && response.OnboardMerchantsResults.Any())
                {
                    var onboardMerchantsResult = response.OnboardMerchantsResults.First();
                    if (onboardMerchantsResult.HasError())
                    {
                        if (onboardMerchantsResult.ContainsError(VisaCallErrorConstants.MerchantAlreadyOnboarded))
                        {
                            isSuccess = true;
                        }
                        else
                        {
                            errorMsg = onboardMerchantsResult.GetErrorInfoDetail();
                        }
                    }
                }
            }

            if (!isSuccess)
            {
                var msg = $"Unable to Onboard Merchant {merchant.GetMerchantInfo()} ResponseStatusCode={response.StatusCode} Error={errorMsg}";
                throw new Exception(msg);
            }
        }


        private static UpdateOfferRequest GetUpdateOfferRequest(string offerId, string action, string visaCommunityCode)
        {
            //var offerGuid = Guid.Parse(offerId);
            //var offerIdBase62Encoded = offerGuid.ToByteArray().ToBase62();

            var request = new UpdateOfferRequest
            {
                OfferId = offerId,
                CommunityCode = visaCommunityCode,
                Action = action
            };

            return request;
        }


        private static GetOfferDetailsByOfferIdRequest GetGetOfferDetailsByOfferIdRequest(int offerId, string visaCommunityCode)
        {
            var request = new GetOfferDetailsByOfferIdRequest
            {
                OfferId = offerId,
                CommunityCode = visaCommunityCode,

            };

            return request;
        }
    }
}