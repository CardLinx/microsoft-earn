//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the template creator for merchant transaction report
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DealsServerClient;
    using DotM.DataContracts;
    using Microsoft.WindowsAzure;
    using System.Collections.Generic;
    using Analytics.API.Contract;
    using OffersEmail.DataContracts;
    using Microsoft.Azure;

    /// <summary>
    ///  Defines the template creator for merchant transaction report
    /// </summary>
    public class MerchantReportTemplateCreator : ITemplateModelCreator<MerchantReportContract>
    {
        #region Const

        /// <summary>
        /// The client name.
        /// </summary>
        private const string ClientName = "BO_MERCHANTREPORT_EMAIL";

        /// <summary>
        /// The deals server address setting.
        /// </summary>
        private const string DealsServerAddressSetting = "LoMo.DealsServer.Address";

        #endregion

        #region Members

        /// <summary>
        /// The deals selector.
        /// </summary>
        private readonly DealsClient _dealsClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MerchantReportTemplateCreator()
        {
            Uri dealsServerBaseAddress = new Uri(CloudConfigurationManager.GetSetting(DealsServerAddressSetting));
            _dealsClient = new DealsClient(dealsServerBaseAddress, ClientName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the template contract for merchant transaction report email
        /// </summary>
        /// <param name="modelData">Merchant Email template data</param>
        /// <returns>The MerchantReportContract</returns>
        public MerchantReportContract GenerateModel(EmailTemplateData modelData)
        {
            MerchantReportContract merchantReportContract = null;
            if (modelData is MerchantTemplateData)
            {
                MerchantTemplateData merchantTemplateData = modelData as MerchantTemplateData;
                merchantReportContract = new MerchantReportContract
                    {
                        FromDate = merchantTemplateData.FromDate.ToString("MM/dd/yyyy"),
                        ToDate = merchantTemplateData.ToDate.ToString("MM/dd/yyyy"),
                        ScheduleType = merchantTemplateData.ScheduleType,
                        MerchantPortalUrl = merchantTemplateData.MerchantPortalUrl,
                        MerchantStoreTransactions = new MerchantStoreTransactionContract[merchantTemplateData.RedemptionsByMerchant.Count]
                    };

                Dictionary<string, List<RedemptionContract>> redemptionsByMerhantId = merchantTemplateData.RedemptionsByMerchant;

                List<MerchantStoreTransactionContract> storeTransactions = new List<MerchantStoreTransactionContract>();
                foreach (KeyValuePair<string, List<RedemptionContract>>  kvp in redemptionsByMerhantId)
                {
                    MerchantStoreTransactionContract storeTransactionContract = new MerchantStoreTransactionContract();
                    List<MerchantTransactionContract> transactionContracts = new List<MerchantTransactionContract>();

                    foreach (var merchantStore in kvp.Value)
                    {
                        if (string.IsNullOrEmpty(storeTransactionContract.MerchantName))
                        {
                            storeTransactionContract.MerchantName = merchantStore.MerchantName;
                        }

                        if (storeTransactionContract.StoreLocation == null)
                        {
                            storeTransactionContract.StoreLocation = new MerchantLocationContract
                                {
                                    Address = string.Format("{0} {1}", merchantStore.MerchantLocation.Address1, merchantStore.MerchantLocation.Address2),
                                    City = merchantStore.MerchantLocation.City,
                                    State = merchantStore.MerchantLocation.State,
                                    Postal = merchantStore.MerchantLocation.Postal
                                };
                        }
                        List<Guid> dealsGuid = new List<Guid> {merchantStore.DealId};
                        Task<IEnumerable<Deal>> dealsByGuidTask = _dealsClient.GetDealsById(dealsGuid);
                        IEnumerable<Deal> deals = dealsByGuidTask.Result.ToList();
                        
                        MerchantTransactionContract transactionContract = new MerchantTransactionContract
                            {
                                RedemptionTime = merchantStore.AuthorizationDateTimeLocal != null
                                                     ? merchantStore.AuthorizationDateTimeLocal.ToString()
                                                     : string.Empty,
                                DealTitle = !deals.Any() ? string.Empty : deals.First().Title,
                                SettlementDate = merchantStore.EventDateTimeUtc.ToString("MM/dd/yyyy"),
                                SettlementAmount = merchantStore.Amount.ToString(),
                                Discount = merchantStore.DiscountAmount.ToString(),
                                CardLastFourDigits = merchantStore.CardLastFourDigits
                            };

                        int dollarValue = merchantStore.Amount/100;
                        int cents = merchantStore.Amount%100;
                        transactionContract.SettlementAmount = string.Format("${0}.{1}", dollarValue, cents.ToString("00"));

                        dollarValue = merchantStore.DiscountAmount / 100;
                        cents = merchantStore.DiscountAmount % 100;
                        transactionContract.Discount = string.Format("${0}.{1}", dollarValue, cents.ToString("00"));

                        transactionContracts.Add(transactionContract);
                    }
                    storeTransactionContract.Transactions = transactionContracts.ToArray();
                    storeTransactions.Add(storeTransactionContract);
                }

                merchantReportContract.MerchantStoreTransactions = storeTransactions.ToArray();
            }

            return merchantReportContract;
        }

        #endregion
    }
}
    