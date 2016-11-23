//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The reporting manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionReporting
{
    using System.Collections.Generic;
    using LoMo.UserServices.DealsMailing;
    using Lomo.Logging;
    using Microsoft.Azure;
    /// <summary>
    /// The reporting manager
    /// </summary>
    public class ReportingManager
    {
        #region Consts

        /// <summary>
        ///  User services storage setting
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        /// The transaction jobs queue name.
        /// </summary>
        private const string TransactionJobsQueueNameSetting = "LoMo.TransactionJobs.Queue";

        /// <summary>
        /// The job processor counter setting.
        /// </summary>
        private const string JobsProcessorCountSetting = "LoMo.TransactionJobs.JobsProcessorCount";

        #endregion

        /// <summary>
        /// Bootstraps the job agents
        /// </summary>
        /// <param name="idPrefix">
        /// The agent id Prefix.
        /// </param>
        /// <returns>
        /// List of instantiaged job processors
        /// </returns>
        public IEnumerable<TransactionJobProcessor> Bootstrap(string idPrefix)
        {
            int jobProcessorCount = int.Parse(CloudConfigurationManager.GetSetting(JobsProcessorCountSetting));
            Log.Verbose("Reporting Manager. Bootstrap Start - agents number = {0}", jobProcessorCount);

            string storageSetting = CloudConfigurationManager.GetSetting(StorageSetting);
            string emailJobsQueueName = CloudConfigurationManager.GetSetting(TransactionJobsQueueNameSetting);

            //Initialize the transaction jobs queue
            PartnerTransactionsQueue partnerTransactionsQueue = new PartnerTransactionsQueue(storageSetting, emailJobsQueueName);
            List<TransactionJobProcessor> agents = new List<TransactionJobProcessor>();
            for (int i = 0; i < jobProcessorCount; i++)
            {
                TransactionJobProcessor transactionJobProcessor = new TransactionJobProcessor(idPrefix + "_" + i, partnerTransactionsQueue);
                agents.Add(transactionJobProcessor);
            }

            Log.Verbose("Reporting Manager. Bootstrap Completed - agents number = {0}", jobProcessorCount);

            return agents;
        }
    }
}