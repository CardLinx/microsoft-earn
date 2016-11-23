//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using OfferManagement.BingMapClient;
using OfferManagement.Dal;
using OfferManagement.DataModel;
using OfferManagement.MerchantFileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisaMidLookup
{
    class Program
    {
        static void Main(string[] args)
        {
            string providerId = "a29c8361-9ab7-40f3-a469-a51be549cea3";
            var merchantsFromDb = GetMerchantsFromDb(providerId);
            var merchantsFromFile = GetMerchantsFromVisaFile("WholeFoods.xlsx");

            var merchantsWithoutVisaIds = from merchant in merchantsFromDb
                                          where (merchant.Payments == null) ||
                                                !(merchant.Payments.Any(payment => payment.Processor == PaymentProcessor.Visa))
                                          select merchant;
            int matched = 0;
            List<Merchant> lstMatchedMerchants = new List<Merchant>();
            Console.WriteLine($"Merchants without vmid before match : {merchantsWithoutVisaIds.Count()}");
            foreach (var merchant in merchantsWithoutVisaIds)
            {
                var matchingMerchant = merchantsFromFile.FirstOrDefault(m => m.Location.Latitude == merchant.Location.Latitude && m.Location.Longitude == merchant.Location.Longitude);
                if (matchingMerchant == null && merchant.ExtendedAttributes!= null && 
                    merchant.ExtendedAttributes.ContainsKey(MerchantConstants.BingAddress))
                {
                    matchingMerchant = merchantsFromFile.FirstOrDefault(m => m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.BingAddress)
                    && m.ExtendedAttributes[MerchantConstants.BingAddress] == merchant.ExtendedAttributes[MerchantConstants.BingAddress]);
                }
                if (matchingMerchant != null)
                {
                    var midFromMatchingMerchant = matchingMerchant.Payments.Where(midInfo => midInfo.Processor == PaymentProcessor.Visa && midInfo.PaymentMids != null);
                    foreach (var matchingMid in midFromMatchingMerchant)
                    {
                        merchant.Payments.Add(new Payment
                        {
                            Id = Guid.NewGuid().ToString(),
                            Processor = PaymentProcessor.Visa,
                            LastUpdate = DateTime.UtcNow,
                            IsActive = true,
                            SyncedWithCommerce = false,
                            PaymentMids = matchingMid.PaymentMids
                        });
                    }
                    lstMatchedMerchants.Add(merchant);
                    matched++;
                }
            }

            Console.WriteLine($"Total matched : {matched}. Updating MID's of matched merchants to db");
            Task.WaitAny(EarnRepository.Instance.UpdateAsync(lstMatchedMerchants));
            Console.WriteLine("Merchants updated..");
            
            Console.ReadKey();
        }

        static IEnumerable<Merchant> GetMerchantsFromDb(string providerId)
        {
            Console.WriteLine($"Getting list of merchants for {providerId} from db");
            var merchants = EarnRepository.Instance.GetMerchantsForProviderAsync(providerId).Result;
            Console.WriteLine($"Retrieved {merchants.Count()} merchants for {providerId} from db");
            GeocodeMerchants(merchants);

            return merchants;
        }

        static IEnumerable<Merchant> GetMerchantsFromVisaFile(string fileName)
        {
            Console.WriteLine($"Getting list of merchants from the visa file {fileName}");
            FileStream fs = File.OpenRead(fileName);
            ExcelFileProcessor excelParser = new ExcelFileProcessor();
            var merchants = excelParser.ImportVisaMidFile(fs);
            Console.WriteLine($"Retrieved {merchants.Count()} merchants from the visa file");
            GeocodeMerchants(merchants);

            return merchants;
        }

        static void GeocodeMerchants(IEnumerable<Merchant> merchants)
        {
            Console.WriteLine("Geocoding merchants");
            foreach (var merchant in merchants)
            {
                if (!merchant.IsLocationGeocoded())
                {
                    var response = Geocoding.GetLocation(merchant.Location.Address, merchant.Location.State, merchant.Location.Zip, merchant.Location.City);
                    if (response != null)
                    {
                        if (merchant.ExtendedAttributes == null)
                            merchant.ExtendedAttributes = new Dictionary<string, string>();

                        merchant.ExtendedAttributes.Add(MerchantConstants.BingAddress, response.address.formattedAddress);
                        merchant.Location.Latitude = response.point.coordinates[0];
                        merchant.Location.Longitude = response.point.coordinates[1];
                    }
                    else
                    {
                        Console.WriteLine(merchant.ToString());
                    }
                }
            }
            Console.WriteLine("Finished Geocoding merchants");
        }
    }
}