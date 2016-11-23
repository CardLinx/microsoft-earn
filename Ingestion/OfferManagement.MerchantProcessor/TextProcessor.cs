//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using OfferManagement.DataModel;
using Lomo.Logging;
using System.Threading.Tasks;
using System.Linq;
using OfferManagement.Dal;
using System.Text;

namespace OfferManagement.MerchantFileParser.MasterCard
{
    public class TextProcessor : MerchantFileProcessor
    {
        public override object GetMerchantsForExport(IList<Merchant> merchants, PaymentProcessor paymentProcessor, string providerId = null)
        {
            string exportData = null;
            switch (paymentProcessor)
            {
                case PaymentProcessor.MasterCard:
                    Log.Info("Starting export of merchants in the MasterCard schema specification");
                    exportData = GetMasterCardDataForExport(providerId, merchants);
                    break;
                case PaymentProcessor.Visa:
                case PaymentProcessor.Amex:
                    Log.Warn("Export of merchant list in text file is not implemented for Visa/Amex");
                    break;
                default:
                    Log.Error("Unknown payment processor");
                    break;
            }

            return exportData;
        }

        private string GetMasterCardDataForExport(string providerId, IList<Merchant> lstMerchants)
        {
            var provider = EarnRepository.Instance.GetProviderAsync(providerId).Result;
            if (provider == null)
            {
                Log.Error($"ProviderId {providerId} not found. Cannot generate the export data");
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HeaderRecord.Create(provider, MerchantConstants.MasterCardRecordDelimiter));
            Log.Info("Header Record crated");

            string provisioningDate = provider.ExtendedAttributes != null &&
                provider.ExtendedAttributes.ContainsKey(MerchantConstants.MCProvisioningDate)
                ? provider.ExtendedAttributes[MerchantConstants.MCProvisioningDate] : null;
            foreach (var merchant in lstMerchants)
            {
                sb.AppendLine(DetailRecord.Create(merchant, MerchantConstants.MasterCardRecordDelimiter, provisioningDate));
            }
            Log.Info("Merchant detail records created");

            sb.Append(TrailerRecord.Create(lstMerchants.Count(), MerchantConstants.MasterCardRecordDelimiter));
            Log.Info("Crated trailer record");

            return sb.ToString();
        }

        public override IList<Merchant> ImportMasterCardAuthFile(Stream fileStream)
        {
            var parsedAuthData = ParseMasterCardFile(fileStream, MerchantFileType.MasterCardAuth);
            if (parsedAuthData == null)
                return null;

            return parsedAuthData.Item2;
        }

        public override IList<Merchant> ImportMasterCardClearingFile(Stream fileStream)
        {
            var parsedClearingData = ParseMasterCardFile(fileStream, MerchantFileType.MasterCardClearing);
            if (parsedClearingData == null)
                return null;

            return parsedClearingData.Item2;
        }

        public override Tuple<string, IList<Merchant>> ImportMasterCardProvisioningFile(Stream fileStream)
        {
            return ParseMasterCardFile(fileStream, MerchantFileType.MasterCardProvisioning);
        }

        private Tuple<string, IList<Merchant>> ParseMasterCardFile(Stream fileStream, MerchantFileType merchantFileType)
        {
            bool validFile = true;
            string provisioningFileDate = null;

            List<Merchant> lstMerchants = new List<Merchant>();

            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                int lineNumber = 0;
                string currentLine = streamReader.ReadLine();
                while (!string.IsNullOrWhiteSpace(currentLine) && validFile)
                {
                    string recordType = currentLine.Substring(0, RecordLengthConstants.RecordType);

                    switch (recordType)
                    {
                        case HeaderRecord.RecordType:
                            Log.Info("Parsing the header record");
                            provisioningFileDate = HeaderRecord.Parse(currentLine, MerchantConstants.MasterCardRecordDelimiter);
                            validFile = !string.IsNullOrWhiteSpace(provisioningFileDate);
                            lineNumber++;
                            break;
                        case DetailRecord.ProvisioningRecordType:
                            Log.Info($"Parsing the {merchantFileType.ToString()} merchant detail record");
                            Merchant merchant = DetailRecord.Parse(currentLine, MerchantConstants.MasterCardRecordDelimiter, merchantFileType, lineNumber);
                            if (merchant != null)
                            {
                                lstMerchants.Add(merchant);
                                Log.Info($"Parsed the {merchantFileType.ToString()} in line number {lineNumber}");
                            }
                            lineNumber++;
                            break;
                        case DetailRecord.ResponseFileRecordType:
                            Log.Info($"Parsing the {merchantFileType.ToString()} merchant detail record");
                            merchant = DetailRecord.Parse(currentLine, MerchantConstants.MasterCardRecordDelimiter, merchantFileType, lineNumber);
                            if (merchant != null)
                            {
                                Merchant existingMerchant = null;

                                //Check if we have already seen this merchant in the current auth/clearing file based on MCID or MCSiteId
                                //If so, then update the payment info for the existing merchant. If not, add the 
                                //new merchant to the list.
                                if (merchant.ExtendedAttributes != null && merchant.ExtendedAttributes.ContainsKey(MerchantConstants.MCID))
                                {
                                    existingMerchant = lstMerchants.FirstOrDefault(m => (m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.MCID)
                                                                    && m.ExtendedAttributes[MerchantConstants.MCID] == merchant.ExtendedAttributes[MerchantConstants.MCID]));
                                }
                                else if (merchant.ExtendedAttributes != null && merchant.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId))
                                {
                                    existingMerchant = lstMerchants.FirstOrDefault(m => (m.ExtendedAttributes != null && m.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                                                                    && m.ExtendedAttributes[MerchantConstants.MCSiteId] == merchant.ExtendedAttributes[MerchantConstants.MCSiteId]));
                                }

                                if (existingMerchant != null)
                                {
                                    existingMerchant.Payments.Add(merchant.Payments[0]);
                                }
                                else
                                {
                                    lstMerchants.Add(merchant);
                                }
                                Log.Info($"Parsed the {merchantFileType.ToString()} in line number {lineNumber}");
                            }
                            lineNumber++;
                            break;
                        case TrailerRecord.RecordType:
                            Log.Info("Parsing the trailer record");
                            int detailRecordCount = TrailerRecord.Parse(currentLine, MerchantConstants.MasterCardRecordDelimiter);
                            //linenumber -2 = discarding the header record to get the total detailed records read
                            validFile = ((lineNumber - 1) == detailRecordCount);
                            break;
                        default:
                            Log.Error($"Unknown record type {recordType} in line number {lineNumber}");
                            lineNumber++;
                            break;
                    }

                    currentLine = streamReader.ReadLine();
                }
            }

            return validFile ? new Tuple<string, IList<Merchant>>(provisioningFileDate, lstMerchants) : null;
        }

        public override IList<Merchant> ImportVisaMidFile(Stream fileStream)
        {
            throw new NotImplementedException();
        }

        public override IList<Merchant> ImportAmexMidFile(Stream fileStream)
        {
            throw new NotImplementedException();
        }
    }
}