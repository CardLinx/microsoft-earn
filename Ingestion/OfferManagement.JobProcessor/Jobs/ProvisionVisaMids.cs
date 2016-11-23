//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lomo.Logging;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.VisaClient;

namespace OfferManagement.JobProcessor.Jobs
{
    public class ProvisionVisaMids : IScheduledJob
    {
        private const string ProviderId = "ProviderId";

        public async Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo)
        {
            IDictionary<string, string> payload = scheduledJobInfo.JobPayload;
            if (payload == null)
            {
                throw new ArgumentNullException($"VisaMerchantLookupJob {scheduledJobInfo.JobId} has an empty payload");
            }
            if (!payload.ContainsKey(ProviderId))
            {
                throw new ArgumentException(
                    $"Payload for VisaMerchantLookupJob {scheduledJobInfo.JobId} is missing the provider id");
            }
            
            if (string.IsNullOrWhiteSpace(payload[ProviderId]))
            {
                throw new ArgumentException($"ProviderId is empty in VisaMerchantLookupJob {scheduledJobInfo.JobId}");
            }
                     
            Provider provider = await EarnRepository.Instance.GetProviderAsync(payload[ProviderId]).ConfigureAwait(false);
            if (provider == null)
            {
                throw new Exception($"Provider id {payload[ProviderId]} not found");
            }

            Log.Info("Retrieving list of merchants for provider {0}", payload[ProviderId]);            
            var merchants = await EarnRepository.Instance.GetMerchantsForProviderAsync(payload[ProviderId]);
            if (merchants == null || !merchants.Any())
            {
                throw new Exception($"VisaMerchantLookupJob failed. Provider {payload[ProviderId]} does not have any merchants");
            }
        
            Log.Info("Calling Visa endpoint to look up mids for {0} merchants", merchants.Count());
            await CallVisaToGetMids(provider, merchants).ConfigureAwait(false);
        }

        private async Task CallVisaToGetMids(Provider provider, IEnumerable<Merchant> merchants)
        {
            var merchantsWithoutVisaIds = from merchant in merchants
                                          where (merchant.Payments == null) ||
                                                !(merchant.Payments.Any(payment => payment.Processor == PaymentProcessor.Visa))
                                          select merchant;

            int totalLookups = 0;
            int successfulLookups = 0;
            int failedLookups = 0;
            long totalTimeTaken = 0;
            foreach (var merchant in merchantsWithoutVisaIds)
            {
                if (ShoudDoVisaLookup(merchant))
                {
                    totalLookups++;
                    MerchantRequest merchantRequest = new MerchantRequest
                    {
                        Address = merchant.Location.Address,
                        City = merchant.Location.City,
                        Name = provider.IsNational ? provider.Name : merchant.Name,
                        PhoneNumber = merchant.PhoneNumber,
                        State = merchant.Location.State,
                        Zip = merchant.Location.Zip,
                        Payments = merchant.Payments
                    };

                    Log.Info("Call Visa to look up MID for {0}", merchantRequest.Name);
                    long timeTaken = 0;
                    Stopwatch sw = new Stopwatch();
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
                        sw.Start();
                        await VisaInvoker.Instance.SearchMerchantDetailsByAttributeAsync(merchant);
                        sw.Stop();
                        timeTaken = sw.ElapsedMilliseconds;
                        totalTimeTaken += timeTaken;
                        successfulLookups++;
                        merchant.Payments = merchantRequest.Payments;
                        Log.Info($"Successfully looked up Visa MID for Merchant : {merchant.Name} in {timeTaken} ms");

                        Log.Info("About to update the merchant : {0} in db ", merchantRequest.Name);
                        await EarnRepository.Instance.UpdateAsync(new List<Merchant> { merchant });
                        Log.Info("Successfully updated the merchant : {0} in db ", merchantRequest.Name);
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        timeTaken = sw.ElapsedMilliseconds;
                        totalTimeTaken += timeTaken;
                        Log.Error(ex, $"Error while looking up visa MID for {merchant.Name}, Time taken : {timeTaken} ms");
                        failedLookups++;
                    }                
                }
                else
                {
                    Log.Warn($"Skipping visa lookup for {merchant.Name} as zip code is not available");
                }              
            }

            long avgTimeForVisaCall = totalLookups > 0 ? (totalTimeTaken / totalLookups) : 0;
            Log.Info(
                $"Finished lookups. Total Merchants : {merchantsWithoutVisaIds.Count()}, Total Lookups : {totalLookups} successful lookups : {successfulLookups}, failed lookups : {failedLookups}, Avg lookup time : {avgTimeForVisaCall}");

        }
        
        public bool ShoudDoVisaLookup(Merchant merchant)
        {
            return merchant.IsAddressAvailable();
        }
    }
}