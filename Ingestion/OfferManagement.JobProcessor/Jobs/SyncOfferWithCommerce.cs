//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lomo.Commerce.DataContracts;
using Newtonsoft.Json;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using Lomo.Logging;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using Lomo.Commerce.AmexClient;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Azure.Utils;
using Microsoft.Azure;
using System.Text;
using Utilities;

namespace OfferManagement.JobProcessor.Jobs
{
    public enum MasterCardIdType
    {
        Authorization,
        Clearance
    }

    public class SyncOfferWithCommerce : IScheduledJob
    {
        private ICommerceService commerceService;
        private const string MCAcquiringMid = "AcquiringMid";
        private const string MCAcquiringICA = "AcquiringICA";
        private const string MCLocationId = "LocationID";
        private const string VisaMid = "VisaMid";
        private const string VisaSid = "VisaSid";
        private AzureBlob MerchantRegistrationAzureBlob;

        public SyncOfferWithCommerce(ICommerceService commerceService)
        {
            this.commerceService = commerceService;
        }

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            IDictionary<string, string> payload = scheduledJobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"SyncOfferWithCommerceJob {scheduledJobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(JobConstants.OfferId))
            {
                throw new ArgumentException(
                    $"Payload for SyncOfferWithCommerceJob {scheduledJobInfo.JobId} is missing the offer id");
            }
            string offerId = payload[JobConstants.OfferId];
            if (string.IsNullOrWhiteSpace(offerId))
            {
                throw new ArgumentException($"OfferId is empty in SyncOfferWithCommerceJob {scheduledJobInfo.JobId}");
            }
            Offer offerInDb = await EarnRepository.Instance.GetOfferAsync(offerId).ConfigureAwait(false);
            if (offerInDb == null)
            {
                throw new Exception($"SyncOfferWithCommerceJob failed. OfferId {offerId} does not exists");
            }
            var offerProvider = await EarnRepository.Instance.GetProviderAsync(offerInDb.ProviderId).ConfigureAwait(false);
            if (offerProvider == null)
            {
                throw new Exception($"SyncOfferWithCommerceJob failed. ProviderId {offerInDb.ProviderId} does not exists");
            }
            var offerMerchants = await EarnRepository.Instance.GetMerchantsForProviderAsync(offerProvider.Id).ConfigureAwait(false);
            if (offerMerchants == null || !offerMerchants.Any())
            {
                throw new Exception($"SyncOfferWithCommerceJob failed. There are no merchants attached the offer provider {offerProvider.Id}");
            }

            // commerce storage client
            IRetryPolicy retryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(6), 4);
            string connectionString = CloudConfigurationManager.GetSetting("CommerceStorageConnectionString");
            MerchantRegistrationAzureBlob = new AzureBlob(connectionString, retryPolicy);

            var merchantsNotFullyRegisteredInCommerce = offerMerchants.Where(offerMerchant => offerMerchant.Payments
            .Any(payment => payment.SyncedWithCommerce == false));
            if (merchantsNotFullyRegisteredInCommerce.Any())
            {
                Tuple<bool, List<Merchant>> registerResult = await RegisterOfferWithCommerceAsync(offerInDb, offerProvider, merchantsNotFullyRegisteredInCommerce).ConfigureAwait(false);
                if (registerResult.Item1 && registerResult.Item2.Any())
                {
                    //Updates to a merchant basically is updating the payment mid of the merchant to indicate
                    //whether the mid was successfully registered in commerce or not
                    Log.Info("Updating the payments inside merchants to mark the sync status with commerce");
                    await EarnRepository.Instance.UpdateAsync<Merchant>(registerResult.Item2);
                }
            }
            else
            {
                Log.Info($"All Mids of merchants associated with offer {offerId} are already synced with commerce");
            }

            scheduledJobInfo.JobCompletedTime = DateTime.UtcNow;
        }

        public async Task<Tuple<bool, List<Merchant>>> RegisterOfferWithCommerceAsync(Offer offer, Provider provider, IEnumerable<Merchant> merchants)
        {
            Log.Info($"Constructing {nameof(V3DealDataContract)} for offer {offer.Id}");
            bool bSuccess = true;
            int currentCount = 0;
            int maxMerchantsPerBatch = Math.Min(300, merchants.Count());
            List<Merchant> lstMerchantsToUpdate = new List<Merchant>();
            Log.Info($"Total merchants {merchants.Count()}. Total merchants to register in a batch to commerce is {maxMerchantsPerBatch}");

            while (currentCount < merchants.Count() && bSuccess)
            {
                var dealDataContract = BuildDealDataContractV3(offer, provider, merchants, currentCount, maxMerchantsPerBatch, lstMerchantsToUpdate);
                Log.Info($"Created {nameof(V3DealDataContract)} for offer {offer.Id}");
                bSuccess = await RegisterOfferWithCommerceAsync(dealDataContract);

                if (bSuccess)
                {
                    currentCount += maxMerchantsPerBatch;
                    foreach (var merchant in lstMerchantsToUpdate)
                    {
                        foreach (var payment in merchant.Payments)
                        {
                            payment.SyncedWithCommerce = true;
                        }
                    }
                }
                else
                {
                    Log.Warn("Clearing the list of merchants to update as the register call failed");
                    lstMerchantsToUpdate.Clear();
                }

            }

            return new Tuple<bool, List<Merchant>>(bSuccess, lstMerchantsToUpdate);
        }

        private async Task<bool> RegisterOfferWithCommerceAsync(V3DealDataContract dealDataContract)
        {
            bool bSuccess = false;
            string offerId = dealDataContract.Id.ToString();
            Log.Info($"Calling commerce to register offer {offerId}");
            string commercePayload = JsonConvert.SerializeObject(dealDataContract);

            int retryCount = 0;
            bool retry = true;

            while (retry)
            {
                try
                {
                    var commerceResponse = await Task.Run(() => this.commerceService.RegisterOffer(commercePayload)).ConfigureAwait(false);
                    if (commerceResponse != null)
                    {
                        retry = false;
                        if (commerceResponse.ResultSummary != null && (commerceResponse.ResultSummary.ResultCode == "Created" ||
                                                                commerceResponse.ResultSummary.ResultCode == "Success"))
                        {
                            bSuccess = true;
                            Log.Info(
                                $"Successfully registered offer {dealDataContract.Id.ToString()} for provider {dealDataContract.ProviderId}, {dealDataContract.ProviderName} with commerce");
                        }
                    }
                }
                catch (CryptographicException)
                {
                    Log.Error($"Error in registering offer with commerce.Unable to find commerce certificate.");
                    retry = false;
                }
                catch (WebException ex)
                {
                    Log.Error($"Unable to register the offer with commerce");
                    var response = (HttpWebResponse)ex.Response;
                    if (response != null)
                    {
                        CommerceResponse commerceResponse = ExtractCommerceResponse(response);
                        if (commerceResponse != null)
                        {
                            Log.Error($"Error is {commerceResponse.ResultSummary.ResultCode} ; {commerceResponse.ResultSummary.Explanation}");
                            retry = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Unable to register the offer with commerce; {ex.Message}");
                }

                if (retry)
                {
                    retryCount++;
                    if (retryCount < 3)
                    {
                        int delayInterval = retryCount * 50;
                        Log.Info($"Register offer with commerce failed...Call will be retried after a delay of {delayInterval} ms");
                        await Task.Delay(delayInterval);
                    }
                    else
                        retry = false;
                }
            }

            return bSuccess;
        }

        private CommerceResponse ExtractCommerceResponse(HttpWebResponse response)
        {
            CommerceResponse result = null;
            try
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    string responseText;
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        responseText = streamReader.ReadToEnd();
                    }
                    if (!string.IsNullOrWhiteSpace(responseText))
                    {
                        result = JsonConvert.DeserializeObject<V3RegisterDealResponse>(responseText);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

            return result;
        }


        public V3DealDataContract BuildDealDataContractV3(Offer offer, Provider provider, IEnumerable<Merchant> merchants, int currentIndex, int maxMerchantsPerBatch, List<Merchant> lstMerchantsToUpdate)
        {
            int index = 0;
            var dealDataContract = new V3DealDataContract
            {
                Id = Guid.Parse(offer.Id),
                ProviderId = provider.Id,
                ProviderName = provider.Name,
                IsNational = provider.IsNational
            };
            var discountList = new List<V3DiscountDataContract>();
            Log.Info($"Creating {nameof(V3DiscountDataContract)} for Offer : {dealDataContract.Id}, Provider : {provider.Name}");
            if (provider.IsNational)
            {
                Dictionary<string, List<string>> partnerMerchantIds = new Dictionary<string, List<string>>();
                var discount = GetDiscountDataContract(offer, provider, null);
                while (currentIndex < merchants.Count() && index < maxMerchantsPerBatch)
                {
                    Merchant merchant = merchants.ElementAt(currentIndex);
                    if (merchant.Payments != null && merchant.Payments.Any())
                    {
                        Log.Info($"Getting Partner Mids for Provider {provider.Name}, Merchant {merchant.Name}");
                        foreach (var partnerMerchantId in GetPartnermerchantIds(merchant))
                        {
                            if (!partnerMerchantIds.ContainsKey(partnerMerchantId.Key))
                                partnerMerchantIds.Add(partnerMerchantId.Key, partnerMerchantId.Value);
                            else
                            {
                                partnerMerchantIds[partnerMerchantId.Key].AddRange(partnerMerchantId.Value);
                            }
                        }
                        lstMerchantsToUpdate.Add(merchant);

                    }
                    else
                    {
                        Log.Warn($"Payments info is missing for Merchant Id {merchant.Id} in provider {provider.Name}");
                    }
                    currentIndex++;
                    index++;
                }

                discount.PartnerMerchantIds = partnerMerchantIds.Select(kvp => new DiscountPartnerMerchantIds
                {
                    Partner = kvp.Key,
                    MerchantIds = kvp.Value
                }).ToList();
                discountList.Add(discount);
            }
            else
            {
                while (currentIndex < merchants.Count() && index <= maxMerchantsPerBatch)
                {
                    Merchant merchant = merchants.ElementAt(currentIndex);
                    var discount = GetDiscountDataContract(offer, provider, merchant);
                    Log.Info($"Getting Partner Mids for Provider {provider.Name}, Merchant {merchant.Name}");
                    discount.PartnerMerchantIds = GetPartnermerchantIds(merchant).Select(kvp => new DiscountPartnerMerchantIds
                    {
                        Partner = kvp.Key,
                        MerchantIds = kvp.Value
                    }).ToList();
                    discountList.Add(discount);
                    currentIndex++;
                    lstMerchantsToUpdate.Add(merchant);
                }
            }

            dealDataContract.Discounts = discountList;

            return dealDataContract;
        }

        private V3DiscountDataContract GetDiscountDataContract(Offer offer, Provider provider, Merchant merchant)
        {
            var discount = new V3DiscountDataContract();
            discount.Id = Guid.Parse(offer.Id);
            discount.MerchantId = merchant != null ? merchant.Id : provider.Id;
            discount.MerchantName = merchant != null ? merchant.Name : provider.Name;
            discount.StartDate = offer.StartDate;
            discount.EndDate = offer.EndDate;
            discount.DiscountSummary = offer.Title;
            discount.Properties = new Dictionary<string, string>();
            discount.DiscountType = "PercentageStatementCredit";
            discount.Properties.Add("percent",
                (Math.Round(offer.Discount)).ToString(CultureInfo.InvariantCulture));
            discount.Properties.Add("currency", "USD");

            if (provider.ProviderType == ProviderType.Earn)
            {
                discount.Properties.Add("reimbursement_tender", "MicrosoftEarn");
            }
            else if (provider.ProviderType == ProviderType.Burn)
            {
                discount.DiscountType = "StaticStatementCredit";
                discount.Properties.Add("reimbursement_tender", "MicrosoftBurn");
                if (!discount.Properties.ContainsKey("amount"))
                {
                    discount.Properties.Add("amount", "0");
                }
            }
            return discount;
        }

        private Dictionary<string, List<string>> GetPartnermerchantIds(Merchant merchant)
        {
            Dictionary<string, List<string>> partnerIds = new Dictionary<string, List<string>>();
            Log.Info($"[{nameof(GetPartnermerchantIds)}] Processing merchant {merchant.Name}");

            foreach (var payment in merchant.Payments)
            {
                if (payment.Processor == PaymentProcessor.MasterCard)
                {
                    string masterCardMid = ProcessMasterCardMid(merchant.Name, payment);
                    if (masterCardMid != null)
                    {
                        if (!partnerIds.ContainsKey(PaymentProcessor.MasterCard.ToString()))
                            partnerIds[PaymentProcessor.MasterCard.ToString()] =
                                new List<string> { masterCardMid };
                        else
                        {
                            partnerIds[PaymentProcessor.MasterCard.ToString()].Add(masterCardMid);
                        }
                        Log.Info($"Added MasterCardMID {masterCardMid} of PaymentId {payment.Id} to PartnerMids collection for {merchant.Name}");
                    }
                }
                else if (payment.Processor == PaymentProcessor.Visa)
                {
                    string visaMid = ProcessVisaMid(merchant.Name, payment);
                    if (!string.IsNullOrWhiteSpace(visaMid))
                    {
                        if (!partnerIds.ContainsKey(PaymentProcessor.Visa.ToString()))
                            partnerIds[PaymentProcessor.Visa.ToString()] =
                                new List<string> { visaMid };
                        else
                        {
                            partnerIds[PaymentProcessor.Visa.ToString()].Add(visaMid);
                        }
                        Log.Info($"Added VisaMID {visaMid} of PaymentId {payment.Id} to PartnerMids collection for {merchant.Name}");
                    }
                }
                else if (payment.Processor == PaymentProcessor.Amex)
                {
                    string amexSeNumber = payment.PaymentMids.ContainsKey(MerchantConstants.AmexSENumber) ? payment.PaymentMids[MerchantConstants.AmexSENumber] : null;
                    if (!string.IsNullOrWhiteSpace(amexSeNumber))
                    {
                        // Upload it to commerce for further processing                        
                        string detailRecord = new OfferRegistrationDetail()
                        {
                            ActionCode = OfferRegistrationActionCodeType.Add, // Need to update if already added Merchant
                            MerchantName = merchant.Name,
                            MerchantNumber = amexSeNumber,
                            MerchantEndDate = DateTime.UtcNow.AddYears(10),
                            MerchantId = merchant.Id,
                            OfferName = "Earn Offer",
                            MerchantStartDate = DateTime.UtcNow
                        }.BuildFileDetailRecord();

                        string blobName = "ToBeProcessed/" + amexSeNumber + "-" + GuidUtility.GenerateShortGuid() + ".txt";
                        byte[] contentBytes = Encoding.ASCII.GetBytes(detailRecord);
                        MemoryStream ms = new MemoryStream(contentBytes);
                        ms.Position = 0;
                        Task task = MerchantRegistrationAzureBlob.UploadBlobFromStreamAsync("amex-offer-registrationrecords", blobName, ms);

                        // commerce registration
                        string formattedSeNumber = $"{amexSeNumber};1";
                        if (!partnerIds.ContainsKey(PaymentProcessor.Amex.ToString()))
                            partnerIds[PaymentProcessor.Amex.ToString()] = new List<string> { formattedSeNumber };
                        else
                        {
                            partnerIds[PaymentProcessor.Amex.ToString()].Add(formattedSeNumber);
                        }

                        Task.WaitAny(task);
                        Log.Info($"Added Amex SEnumber {formattedSeNumber} of PaymentId {payment.Id} to PartnerMids collection for {merchant.Name}");
                    }
                }
            }

            return partnerIds;
        }

        private string ProcessMasterCardMid(string merchantName, Payment payment)
        {
            string midInCommerceFormat = null;

            //This is a clearing payment info
            if (payment.PaymentMids.ContainsKey(MCLocationId))
            {
                string clearanceId = payment.PaymentMids[MCLocationId];
                midInCommerceFormat = payment.IsActive ? $";;{clearanceId};1" : $";;{clearanceId};0";
                Log.Info($"Created MasterCard ClearanceId {midInCommerceFormat} for {merchantName}");
            }
            else
            {
                //This is a authorization payment info
                string acquiringIca = payment.PaymentMids.ContainsKey(MCAcquiringICA)
                    ? payment.PaymentMids[MCAcquiringICA]
                    : null;
                string acquiringMid = payment.PaymentMids.ContainsKey(MCAcquiringMid)
                    ? payment.PaymentMids[MCAcquiringMid]
                    : null;
                bool bValid = true;
                if (string.IsNullOrWhiteSpace(acquiringMid))
                {
                    Log.Warn($"Missing Acquirer MID for {merchantName}. MID's for {merchantName} will not be registered with commerce");
                    bValid = false;
                }
                if (bValid && string.IsNullOrWhiteSpace(acquiringIca))
                {
                    Log.Warn($"Missing Acquirer ICA for {merchantName}. MID's for {merchantName} will not be registered with commerce");
                    bValid = false;
                }
                //The above is a square MID which should not be provisioned. Don't have a better solution for now other than hard coding the MID
                if (bValid && acquiringIca.Trim() == "003286" && acquiringMid.Trim() == "242661000053360")
                {
                    Log.Warn(
                        $"Found the Square MID {acquiringIca};{acquiringMid}. MID's for {merchantName} will not be registered with commerce");
                    bValid = false;
                }

                if (bValid)
                {
                    midInCommerceFormat = payment.IsActive ? $"{acquiringIca};{acquiringMid};;1" : $"{acquiringIca};{acquiringMid};;0";
                    Log.Info($"Created MasterCard AuthenticationId {midInCommerceFormat} for {merchantName}");
                }
            }


            return midInCommerceFormat;
        }

        private string ProcessVisaMid(string merchantName, Payment payment)
        {
            string midInCommerceFormat = null;
            string vmid = payment.PaymentMids.ContainsKey(VisaMid) ? payment.PaymentMids[VisaMid] : null;
            string vsid = payment.PaymentMids.ContainsKey(VisaSid) ? payment.PaymentMids[VisaSid] : null;
            bool bValid = true;

            if (string.IsNullOrWhiteSpace(vmid))
            {
                Log.Warn($"Missing VMid for {merchantName}. MID's for {merchantName} will not be registered with commerce");
                bValid = false;
            }
            if (bValid && string.IsNullOrWhiteSpace(vsid))
            {
                Log.Warn($"Missing VSid for {merchantName}. MID's for {merchantName} will not be registered with commerce");
                bValid = false;
            }

            if (bValid)
            {
                midInCommerceFormat = payment.IsActive ? $"{vmid};{vsid};1" : $"{vmid};{vsid};0";
            }

            return midInCommerceFormat;
        }
    }
}