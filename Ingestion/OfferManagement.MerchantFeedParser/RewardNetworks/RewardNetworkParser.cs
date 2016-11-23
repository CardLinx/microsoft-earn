//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Logging;
using OfferManagement.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    public class RewardNetworkParser
    {
        DateTime fileCreationDateFromHeader = DateTime.MinValue;
        DateTime fileCreationDateFromTrailer = DateTime.MinValue;
        int totalRecords = 0;
        IList<Merchant> lstMerchants = new List<Merchant>();

        public async Task<IList<Merchant>> ImportMerchantsAsync(MemoryStream ms)
        {
            ms.Position = 0;     
            using (StreamReader streamReader = new StreamReader(ms))
            {             
                bool validFile = true;
                int lineNumber = 0;
                string currentLine;
                while ((currentLine = await streamReader.ReadLineAsync()) != null && validFile)
                {
                    string recordType = currentLine.Substring(0, RecordLengthConstants.RecordTypeLength);                    
                    switch (recordType)
                    {
                        case HeaderParser.RecordType:
                            Log.Info("Parsing the header record");
                            Tuple<bool,DateTime> headerResult = HeaderParser.Parse(currentLine);
                            validFile = headerResult.Item1;
                            fileCreationDateFromHeader = headerResult.Item2;
                            lineNumber++;
                            break;
                        case DetailParser.RecordType:
                            Merchant merchant = DetailParser.Parse(currentLine, lineNumber);
                            if (merchant != null)
                            {
                               lstMerchants.Add(merchant);
                                Log.Info($"Created merchant {merchant.Name} from {lineNumber}");
                            }
                            lineNumber++;
                            break;
                        case TrailerParser.RecordType:
                            Log.Info("Parsing the trailer record");
                            Tuple<bool, DateTime, string> trailerResult = TrailerParser.Parse(currentLine);
                            validFile = trailerResult.Item1;
                            fileCreationDateFromTrailer = trailerResult.Item2;
                            totalRecords = int.Parse(trailerResult.Item3);
                            break;
                    }
                }
            }

            return ValidateData() ? lstMerchants : null;
        }

        private bool ValidateData()
        {
            bool bValid = true;            
            bValid = (totalRecords == lstMerchants.Count);
            if (bValid)
            {
                bValid = fileCreationDateFromHeader == fileCreationDateFromTrailer;

                if (!bValid)
                    Log.Error("Invalid Reward networks file. File creation date specified in the header and the footer does not match");
            }
            else
            {
                Log.Error(string.Format("Invalid Reward networks File. The total number of records read is {0}. " +
                          "This does not match the number specified in the file {1} ", lstMerchants.Count, totalRecords));
            }

            return bValid;
        }        
    }
}