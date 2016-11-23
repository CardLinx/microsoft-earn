//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deals mailing manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using UserServices.Worker.Common;

namespace LoMo.UserServices.DealsMailing
{
    using System.Collections.Generic;
    using Lomo.Logging;
    using System;
    using Users.Dal;
    using Microsoft.WindowsAzure;
    using Microsoft.Azure;

    /// <summary>
    /// The deals mailing manager.
    /// </summary>
    public class MailingManager : IJobManager
    {
        #region Consts

        /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        /// The message queue name.
        /// </summary>
        private const string EmailJobsQueueNameSetting = "LoMo.EmailJobs.Queue";

        /// <summary>
        /// The agents counter setting.
        /// </summary>
        private const string JobsProcessorCountSetting = "LoMo.EmailJobs.JobsProcessorCount";

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
        public IEnumerable<IJobProcessor> Bootstrap(string idPrefix)
        {
            int jobProcessorCount = int.Parse(CloudConfigurationManager.GetSetting(JobsProcessorCountSetting));
            Log.Verbose("Mailing Manager. Bootstrap Start - agents number = {0}", jobProcessorCount);

            string storageSetting = CloudConfigurationManager.GetSetting(StorageSetting);
            string emailJobsQueueName = CloudConfigurationManager.GetSetting(EmailJobsQueueNameSetting);
            
            //Initialize the jobs queue
            IJobsQueue<EmailCargo> emailJobsQueue = new JobsQueue<EmailCargo>(storageSetting, emailJobsQueueName);
            IPriorityEmailJobsQueue<PriorityEmailCargo> priorityEmailJobsQueue = new PriorityEmailJobsQueue<PriorityEmailCargo>();

            //Initialize the Merchant Email handler
            IEmailJobHandler merchantEmailJobHandler = new MerchantEmailJobHandler();
            merchantEmailJobHandler.Initialize();

            //Initialize the Deals Email handler
            IEmailJobHandler dealsEmailJobHandler = new DealsEmailJobHandler();
            dealsEmailJobHandler.Initialize();

            //Initialize the Remainder Email handler
            IEmailJobHandler remainderMailHandler = new RemainderEmailJobHandler();
            remainderMailHandler.Initialize();

            //Initialize the Campaign Email handler
            IEmailJobHandler campaignEmailJobHandler = new CampaignEmailJobHandler();
            campaignEmailJobHandler.Initialize();

            //Initialize the Priority Email handler
            IPriorityEmailJobHandler confirmationEmailHandler = new ConfirmationEmailJobHandler();
            confirmationEmailHandler.Initialize();
            
            //TODO: Having a different emailhandler and emailcargo for priority email is not right.
            //      The priority email handler and priority email cargo should be of type IEmailJobHandler and EmailCargo respectively
            //      This change has to be done once the users dal is decoupled out of commerce and merchant. We could then have all the jobs and their cargo inside
            //      userservicesstorage.
            //Register the handlers
            Dictionary<Type,object> jobHandlers = new Dictionary<Type, object>
                {
                    {typeof (MerchantReportEmailCargo), merchantEmailJobHandler},
                    {typeof (DealsEmailCargo), dealsEmailJobHandler},
                    {typeof (ConfirmationEmailCargo), confirmationEmailHandler},
                    {typeof (PromotionalEmailCargo), remainderMailHandler},
                    {typeof (CampaignEmailCargo), campaignEmailJobHandler}
                };

            List<EmailJobProcessor> agents = new List<EmailJobProcessor>();
            for (int i = 0; i < jobProcessorCount; i++)
            {
                EmailJobProcessor agent = new EmailJobProcessor(idPrefix + "_" + i, emailJobsQueue, priorityEmailJobsQueue, jobHandlers);
                agents.Add(agent);
            }

            Log.Verbose("Mailing Manager. Bootstrap Completed - agents number = {0}", jobProcessorCount);

            return agents;
        }
    }
}