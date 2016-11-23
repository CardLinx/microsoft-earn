//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using OfferManagement.DataModel;
using OfferManagement.DataModel.Partners.Visa;
using OfferManagement.VisaClient;

namespace VisaMids.Console
{
    class Program
    {
        private const int MaxContinuousException = 20;
        private const string MerchantFileName = "Merchant.csv";
        private const string OfferFileName = "Offers.csv";
        //private const string ActualMerchantName = "Whole Foods Market";
        private static IVisaInvoker VisaInvoker;

        static void Main(string [] args)
        {
            try
            {
                var options = new Options();
                try
                {
                    if (!CommandLine.Parser.Default.ParseArguments(args, options))
                    {
                        // Display the default usage information
                        System.Console.WriteLine(options.GetUsage());
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Inccorect Parameters. " + ex);
                    System.Console.WriteLine(options.GetUsage());
                    return;
                }

                //TODO: We need to remove this statement after we are able to validate Visa ssl certificate during handshake
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
                {
                    return true;
                };


                VisaInvoker = new VisaInvoker(NoRetryPolicy.Instance);
                System.Console.WriteLine("Proxy Info: " + VisaInvoker.GetProxyInfo());
                Task task = null;
                
                if (options.VisaMethod == VisaMethod.SearchMerchantDetailsByAttribute)
                {
                    task = SearchMerchantDetailsByAttributeAsync();
                }
                //user cmd line parameter  "-m 2" to invoke createOffer
                else if (options.VisaMethod == VisaMethod.CreateOffer)
                {
                    task = CreateOfferAsync();
                    //task = CreateOfferAlreadyExistSameNameDifferenIdAsync();
                    //task = CreateOfferAlreadyExistDifferentNameSameIdAsync();
                    //fails when we use external offer id 
                    //task = RejectOfferAsync("a12b8356-3bda-418b-b845-dc15523c26fa");
                    //succedd when we use visa offer id
                    //task = RejectOfferAsync("4661273");
                }

                task?.Wait();
            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
            }
        }
        
        private static async Task SearchMerchantDetailsByAttributeAsync()
        {
            var merchants = MerchantFactory.LoadFeatures(MerchantFileName);
            await GetVisaMids(merchants);
            System.Console.Write("Processed " + merchants.Count + " merchants");
        }

        private static async Task CreateOfferAsync()
        {
            var merchants = MerchantFactory.LoadFeatures(MerchantFileName);
            await CreateOffersAsync(merchants);
            System.Console.Write("Processed " + merchants.Count + " offers");
        }

        private static async Task CreateOffersAsync(IList<Merchant> merchants)
        {
            var continuousException = 0;
            var visaOffers = new List<int>();

            var countSuccessful = 0;
            var countNotSuccessful = 0;
            var totalProcessed = 0;

            foreach (var merchant in merchants)
            {
                try
                {
                    await VisaInvoker.SearchMerchantDetailsByAttributeAsync(merchant);
                    var offerGuid = Guid.NewGuid().ToString();
                    var offer = new VisaOffer
                    {
                        Id = offerGuid,
                        StartDate = DateTime.UtcNow.AddDays(2),
                        EndDate = DateTime.UtcNow.AddDays(3),
                        Name = merchant.Name,
                        CampaignName = "Rewards Network"
                    };

                    System.Console.WriteLine("Calling CreateOfferAsync for " + merchant.GetMerchantInfo());
                    var offerId = await VisaInvoker.CreateOfferAsync(merchant, offer);
                    System.Console.WriteLine("Called CreateOfferAsync");

                    //change offer end date and see if we can update it
                    //it will throw exception "RTMCSCE0017 - Duplicate Offer Name"
                    //offer.EndDate = DateTime.UtcNow.AddDays(2);
                    //offerId = await VisaInvoker.CreateOfferAsync(merchant, offer);

                    //check if we can reject the offer
                    //it will throw exception "User does not have access to the operation UpdateOffer for the given community"
                    //await VisaInvoker.UpdateOfferAsync(offerGuid);

                    visaOffers.Add(offerId);
                    continuousException = 0;
                    ++countSuccessful;
                    ++totalProcessed;
                    if (totalProcessed % 20 == 0)
                    {
                        System.Console.WriteLine("Processed " + totalProcessed + " offers.");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error Calling CreateOffersAsync" + ex);
                    ++totalProcessed;
                    ++countNotSuccessful;
                    ++continuousException;

                    if (continuousException > MaxContinuousException)
                    {
                        System.Console.WriteLine(MaxContinuousException + " continuous exception occured.");
                        throw;
                    }
                }
            }

            var msg = $"Total Offers:{merchants.Count}, Visa offers created:{countSuccessful}, Visa offers Not Found:{countNotSuccessful}";
            System.Console.WriteLine(msg);

            var outputFile = Path.GetFileNameWithoutExtension(OfferFileName) + "Offers" + Path.GetExtension(MerchantFileName);

            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var csv = new CsvWriter(sw);
                    csv.WriteRecords(visaOffers);
                    sw.Flush();
                }
            }
        }



        private static async Task CreateOfferAlreadyExistSameNameDifferenIdAsync()
        {
                try
                {
                    var merchant = new Merchant
                    {
                        Name = "Pink Pony",
                        Payments = new List<Payment>
                        {
                            new Payment()
                            {
                                Processor = PaymentProcessor.Visa,
                                PaymentMids = new Dictionary<string, string>
                                {
                                    {MerchantConstants.VisaMid,"62511386"},
                                    {MerchantConstants.VisaSid,"82512901"},
                                }
                            }
                        }
                    };

                    var offerGuid = Guid.NewGuid().ToString();

                    var offer = new VisaOffer
                    {
                        Id = offerGuid,
                        StartDate = DateTime.UtcNow.AddDays(2),
                        EndDate = DateTime.UtcNow.AddDays(3),
                        Name = "Pink Pony",
                        CampaignName = "Rewards Network"
                    };

                //return error "RTMCSCE0017 - Duplicate Offer Name"
                //if we try to create the offer with a different name for same merchant then we get error "RTMMOBE0025 - Error Merchant Details VisaMerchantId 62511386 VisaStoreId 82512901 - already assigned to Merchant Group MSNCGMG."
                await VisaInvoker.CreateOfferAsync(merchant, offer);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error Calling CreateOffersAsync" + ex);
                }
        }


        private static async Task CreateOfferAlreadyExistDifferentNameSameIdAsync()
        {
            try
            {
                var merchant = new Merchant
                {
                    Name = "ROOT CAFE",
                    Payments = new List<Payment>
                        {
                            new Payment()
                            {
                                Processor = PaymentProcessor.Visa,
                                PaymentMids = new Dictionary<string, string>
                                {
                                    {MerchantConstants.VisaMid,"52003453"},
                                    {MerchantConstants.VisaSid,"72002843"},
                                }
                            }
                        }
                };
                
                var offer = new VisaOffer
                {
                    Id = "a12b8356-3bda-418b-b845-dc15523c26fa",
                    StartDate = DateTime.UtcNow.AddDays(2),
                    EndDate = DateTime.UtcNow.AddDays(3),
                    Name = "ROOT CAFE",
                    CampaignName = "Rewards Network"
                };

                //however if existing offer with same VisaMerchantId and VisaStoreId has expired then it will create offer
                //if the existing offer with same name has expired then also it will throw error "RTMCSCE0017 - Duplicate Offer Name" when creating offer with same name
                //return error "RTMMOBE0025 - Merchant Details VisaMerchantId 52003453 VisaStoreId 72002843 - already assigned to Merchant Group MSNCGMG."
                await VisaInvoker.CreateOfferAsync(merchant, offer);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error Calling CreateOffersAsync" + ex);
            }
        }


        private static async Task RejectOfferAsync(string offerId)
        {
            try
            {
                //use visa offer id to reject offer. We get offer id when we create a new offer
                await VisaInvoker.UpdateOfferAsync(offerId);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error Calling RejectOfferAsync" + ex);
            }
        }

        private static async Task GetVisaMids(IList<Merchant> merchants)
        {
            var continuousException = 0;
            var visaMerchants = new List<VisaMerchant>();

            var countMidsFound = 0;
            var countMidsNotFound = 0;
            var totalProcessed = 0;

            foreach (var merchant in merchants)
            {
                try
                {
                    //merchant.Name = ActualMerchantName;
                    System.Console.WriteLine("Calling SearchMerchantDetailsByAttributeAsync for " + merchant.GetMerchantInfo());
                    await VisaInvoker.SearchMerchantDetailsByAttributeAsync(merchant);
                    System.Console.WriteLine("Called SearchMerchantDetailsByAttributeAsync");
                    var visaMerchant = GetVisaMerchant(merchant, null);
                    visaMerchants.Add(visaMerchant);
                    continuousException = 0;
                    ++countMidsFound;
                    ++totalProcessed;
                    if (totalProcessed % 20 == 0)
                    {
                        System.Console.WriteLine("Processed " + totalProcessed + " merchants.");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error Calling SearchMerchantDetailsByAttributeAsync" + ex);
                    ++totalProcessed;
                    ++countMidsNotFound;
                    var visaMerchant = GetVisaMerchant(merchant, ex);
                    visaMerchants.Add(visaMerchant);

                    ++continuousException;

                    if (continuousException > MaxContinuousException)
                    {
                        System.Console.WriteLine(MaxContinuousException + " continuous exception occured.");
                        throw;
                    }
                }
            }

            var msg = $"Total Merchants:{merchants.Count}, Visa Mids Found:{countMidsFound}, Visa Mids Not Found:{countMidsNotFound}";
            System.Console.WriteLine(msg);

            var outputFile = Path.GetFileNameWithoutExtension(MerchantFileName) + "VisaMids" + Path.GetExtension(MerchantFileName);

            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var csv = new CsvWriter(sw);
                    csv.WriteRecords(visaMerchants);
                    sw.Flush();
                }
            }
            
        }

        private static VisaMerchant GetVisaMerchant(Merchant merchant, Exception ex)
        {
            var location = merchant.Location;
            var visaMerchant = new VisaMerchant
            {
                Name = merchant.Name,
                Address = location.Address,
                City = location.City,
                State = location.State,
                Zip = location.Zip
            };

            var mids = string.Join("|", merchant.Payments.Where(midInfo => midInfo.Processor == PaymentProcessor.Visa && midInfo.PaymentMids != null && midInfo.PaymentMids.Any())
                                .SelectMany(payment => payment.PaymentMids.Values).ToArray());

            visaMerchant.Mid = mids;

            if (ex != null)
            {
                visaMerchant.Error = ex.ToString();
            }

            return visaMerchant;
        }
    }
}