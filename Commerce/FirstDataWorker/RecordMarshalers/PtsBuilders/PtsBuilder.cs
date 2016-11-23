//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security.Policy;
    using System.Text;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.FirstDataClient;
    
    /// <summary>
    /// Builds a First Data PTS file.
    /// </summary>
    public static class PtsBuilder
    {
        /// <summary>
        /// Builds the PTS file representation for the specified record.
        /// </summary>
        /// <param name="records">
        /// The records for which to build a PTS file.
        /// </param>
        /// <param name="submissionDate">
        /// The submission date to specify within the PTS representation of the record.
        /// </param>
        /// <param name="submissionSequenceNumber">
        /// The submission sequence number to place in the PTS file
        /// </param>
        /// <param name="forNonFirstDataPartners">
        /// Will be set to True if PTS being built is for Non-FDC (like Visa) partners
        /// </param>
        /// <returns>
        /// The PTS file representation for the specified record.
        /// </returns>
        /// <remarks>
        /// Since FDC is not on Windows platform, we do not use AppendLine on StringBuilder.
        /// We instead mark \n as new line character, FDC will break on \r\n
        /// </remarks>
        public static string Build(Collection<OutstandingRedeemedDealInfo> records,
                                     DateTime submissionDate,
                                     int submissionSequenceNumber,
                                     bool forNonFirstDataPartners = false)
        {
            StringBuilder result = new StringBuilder();
            int recordNumber = 1;
            int totalDiscountAmount = 0;

            Dictionary<PtsMerchantInfo, Collection<OutstandingRedeemedDealInfo>> recordsByMerchant =
                GroupByMerchant(records);
            foreach (KeyValuePair<PtsMerchantInfo, Collection<OutstandingRedeemedDealInfo>> keyValuePair in recordsByMerchant)
            {
                PtsMerchantInfo merchantInfo = keyValuePair.Key;
                result.Append(MerchantRecordPtsBuilder.Build(merchantInfo, submissionDate, submissionSequenceNumber,
                                                                recordNumber++));
                result.Append("\n");

                result.Append(DescriptorRecordPtsBuilder.Build(merchantInfo, recordNumber++));
                result.Append("\n");

                // Append records for each detail record in turn.
                foreach (OutstandingRedeemedDealInfo detailRecord in keyValuePair.Value)
                {
                    totalDiscountAmount += detailRecord.DiscountAmount;
          
                    result.Append(SpecialConditionPtsBuilder.Build(detailRecord, recordNumber++, forNonFirstDataPartners));
                    result.Append("\n");

                    result.Append(AcquirerReferenceNumberPtsBuilder.Build(detailRecord, recordNumber++, forNonFirstDataPartners));
                    result.Append("\n");

                    result.Append(TokenizationPtsBuilder.Build(detailRecord, recordNumber++));
                    result.Append("\n");

                    result.Append(TransactionDetailPtsBuilder.Build(detailRecord, recordNumber++));
                    result.Append("\n");
                }
            }

            // Append the total record.
            result.Append(TotalRecordPtsBuilder.Build(totalDiscountAmount, recordNumber));
            result.Append("\n");

            return result.ToString();
        }

        /// <summary>
        /// Group the redeemed deal records by merchant
        /// </summary>
        /// <param name="records">
        /// Collection of Outstanding Redeemed Deal Info Records
        /// </param>
        /// <returns>
        /// Dictionary of Merchant -> Redeemed Deals
        /// </returns>
        private static Dictionary<PtsMerchantInfo, Collection<OutstandingRedeemedDealInfo>> GroupByMerchant(Collection<OutstandingRedeemedDealInfo> records)
        {
            Dictionary<PtsMerchantInfo, Collection<OutstandingRedeemedDealInfo>> dictionary = new Dictionary<PtsMerchantInfo, Collection<OutstandingRedeemedDealInfo>>();
            foreach (OutstandingRedeemedDealInfo outstandingRedeemedDealInfo in records)
            {
                PtsMerchantInfo merchantInfo = new PtsMerchantInfo()
                {
                    PartnerMerchantId = outstandingRedeemedDealInfo.PartnerMerchantId,
                    MerchantName = outstandingRedeemedDealInfo.MerchantName,
                    ReimbursementTender = outstandingRedeemedDealInfo.ReimbursementTender
                };

                Collection<OutstandingRedeemedDealInfo> existing;
                if (dictionary.ContainsKey(merchantInfo))
                {
                    existing = dictionary[merchantInfo];
                    existing.Add(outstandingRedeemedDealInfo);
                }
                else
                {
                    existing = new Collection<OutstandingRedeemedDealInfo>();
                    existing.Add(outstandingRedeemedDealInfo);
                    dictionary.Add(merchantInfo,existing);
                }
            }

            return dictionary;
        }
        
    }
}