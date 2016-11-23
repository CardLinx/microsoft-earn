//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Commerce.RewardsNetworkWorker.RecordMarshalers.TransactionReportBuilder;
    using Deals.DataAccess;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.SftpClient;
    using Lomo.Commerce.Utilities;
    using Microsoft.Azure;

    /// <summary>
    /// Generates daily transaction report for Reward network and uploads it to RN ftp
    /// </summary>
    public class RewardNetworkReportProcessor
    {
        public RewardNetworkReportProcessor(CommerceContext commerceContext)
        {
            Context = commerceContext;
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            FtpUserName = CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpUserName);
            FtpPassword = CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpPassword);
            FtpUri = CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpAddress);
            FtpFolder = CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpTransactionReportDirectory);
        }

        /// <summary>
        /// Generates transaction report for Rewardnetwork between the given dates
        /// </summary>
        /// <param name="startDate">Start date for the transaction report</param>
        /// <param name="endDate">End date for the transaction report exclusive</param>
        public void GenerateReportForDays(DateTime startDate, DateTime endDate)
        {
            DateTime reportDate = startDate.AddDays(1);

            //Get all the transactions till the end date
            while (reportDate <= endDate)
            {
                //Get all transactions from db
                var result = RedeemedDealOperations.GetRedeemedDealTransactionInfosByDate(startDate, endDate);

                if (result == ResultCode.Success)
                {
                    List<RedeemedDealTransactionInfo> lstTransactionInfos =
                  (List<RedeemedDealTransactionInfo>)RedeemedDealOperations.Context[Key.RedeemedDealInfo];

                    List<string> lstDealGuids =
                        lstTransactionInfos.Select(redeemDealTransactionInfo => redeemDealTransactionInfo.DealId.ToString())
                            .ToList();

                    //Filter out only RN transactions
                    Dictionary<string, DotM.DataContracts.Deal> dealIdsToProviderId = GetRewardNetworkDeals(lstDealGuids);

                    Collection<TransactionRecord> lstTransactionRecords = GetTransactionRecords(lstTransactionInfos,
                        dealIdsToProviderId);

                    //Generate the report as per RN specification
                    Tuple<string, string> reportTuple = TransactionReportBuilder.Build(lstTransactionRecords, startDate);

                    //Upload the report to RN FTP
                    UploadReport(reportTuple);

                    //Update the last successful run date in the context. This will eventually be persisted in the scheduler job details, so that we know the start date for the next run
                    Context[Key.RewardNetworkReportLastRunDate] = startDate;

                    startDate = startDate.AddDays(1);
                    reportDate = reportDate.AddDays(1);
                }   
            }
        }

        /// <summary>
        /// Marshalls the redeem deal trasnsaction into TransactionRecord 
        /// </summary>
        /// <param name="lstTransactionInfos">List of redeemed deal transactions</param>
        /// <param name="dealIdsToProviderId">Deal Id to deal object dictionary</param>
        /// <returns>List of transaction record</returns>
        private Collection<TransactionRecord> GetTransactionRecords(List<RedeemedDealTransactionInfo> lstTransactionInfos, Dictionary<string, DotM.DataContracts.Deal> dealIdsToProviderId)
        {
            Collection<TransactionRecord> lstTransactionRecords = new Collection<TransactionRecord>();
            foreach (var redeemDealTransactionInfo in lstTransactionInfos)
            {
                if (dealIdsToProviderId.ContainsKey(redeemDealTransactionInfo.DealId.ToString()))
                {
                    DotM.DataContracts.Deal deal = dealIdsToProviderId[redeemDealTransactionInfo.DealId.ToString()];
                    if (deal != null && !string.IsNullOrWhiteSpace(deal.ProviderDealId))
                    {
                        string zipCode = null;
                        string[] providerDealIdParts = deal.ProviderDealId.Split(':');
                        if (deal.Business != null && deal.Business.Locations != null &&
                            deal.Business.Locations.Count > 0)
                        {
                            zipCode = deal.Business.Locations.First().Zip;
                        }
                        if (providerDealIdParts.Length == 3)
                        {
                            TransactionRecord transactionRecord = new TransactionRecord
                            {
                                MerchantId = providerDealIdParts[2],
                                CardLastFourDigits = redeemDealTransactionInfo.CardLastFourDigits,
                                CardType = redeemDealTransactionInfo.CardType,
                                MerchantName = redeemDealTransactionInfo.MerchantName,
                                TransactionAmount = redeemDealTransactionInfo.TransactionAmount,
                                TransactionDate = redeemDealTransactionInfo.TransactionDate,
                                TransactionIdentifier = redeemDealTransactionInfo.RedemptionId.ToString(),
                                CardMemberZip = zipCode
                            };
                            lstTransactionRecords.Add(transactionRecord);
                        }
                    }
                }
            }

            return lstTransactionRecords;
        }

        /// <summary>
        /// Returns RN transactions from the list of all redeemed deals
        /// </summary>
        /// <param name="lstDealGuids">Deal Guids</param>
        /// <returns>Dictionary of Deal id to Deal object for RN transactions</returns>
        private Dictionary<string, DotM.DataContracts.Deal> GetRewardNetworkDeals(List<string> lstDealGuids)
        {
            Dictionary<string, DotM.DataContracts.Deal> dealIdsToProviderId = new Dictionary<string, DotM.DataContracts.Deal>();
            var deals = DealsRepository.GetDealsById(lstDealGuids.Distinct());

            foreach (var deal in deals)
            {
                if (deal.Categories != null &&
                      deal.Categories.Find(item => (item.CategoryName.Trim().ToLower() == "rewards network") ||
                      item.CategoryName.Trim().ToLower() == "microsoft/91000") != null)
                {
                    dealIdsToProviderId.Add(deal.Id, deal);
                }
            }

            return dealIdsToProviderId;
        }

        /// <summary>
        /// Uploads the RN transaction report to RN ftp
        /// </summary>
        /// <param name="reportTuple">Tuple of transaction file name and transaction data</param>
        private void UploadReport(Tuple<string, string> reportTuple)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(reportTuple.Item2));
         
            DefaultSftpClient defaultSftpClient = new DefaultSftpClient(FtpUserName, FtpPassword, FtpUri);
            defaultSftpClient.UploadFileAsync(reportTuple.Item1, memoryStream, FtpFolder).Wait();
        }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }

        /// <summary>
        /// Reward network ftp usersname
        /// </summary>
        private string FtpUserName { get; set; }

        /// <summary>
        /// Reward network ftp password
        /// </summary>
        private string FtpPassword { get; set; }

        /// <summary>
        /// Reward network ftp uri
        /// </summary>
        private string FtpUri { get; set; }

        /// <summary>
        /// Reward network ftp folder for uploading transaction report
        /// </summary>
        private string FtpFolder { get; set; }
    }
}