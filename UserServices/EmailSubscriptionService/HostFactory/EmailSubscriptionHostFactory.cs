//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The lookup service host factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.EmailSubscription.Service.HostFactory
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using Lomo.Logging;

    using LoMo.UserServices.Storage.HCP;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure;

    using Users.Dal;

    /// <summary>
    ///     The lookup service host factory.
    /// </summary>
    public class EmailSubscriptionHostFactory : ServiceHostFactory
    {
        #region Fields

        /// <summary>
        /// The mdm application id.
        /// </summary>
        private readonly int mdmApplicationId;

        /// <summary>
        /// The publication description.
        /// </summary>
        private readonly string publicationDescription;

        /// <summary>
        ///     Gets or sets the publication id.
        /// </summary>
        private readonly string publicationId;

        /// <summary>
        ///     Gets or sets the publication name.
        /// </summary>
        private readonly string publicationName;

        /// <summary>
        ///     Gets or sets the publication opt-in link.
        /// </summary>
        private readonly string publicationOptinLink;

        /// <summary>
        ///     The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        /// <summary>
        /// The hcp commands queue
        /// </summary>
        private readonly IHcpCommandsQueue hcpCommandsQueue;

        #endregion
        #region Constructors and Destructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmailSubscriptionHostFactory" /> class.
        /// </summary>
        public EmailSubscriptionHostFactory()
        {
            try
            {
                string storageAccountConnectionString = this.GetMandatorySetting("StorageAccountConnectionString");
                
                // User Dal
                string usersConnectionString = this.GetMandatorySetting("UsersDalConnectionString");
                this.usersDal = new UsersDal(usersConnectionString, null, false, new PriorityEmailJobsQueue<PriorityEmailCargo>(storageAccountConnectionString));
                
                // HCP commands queue
                int queueMaxRetries = int.Parse(this.GetMandatorySetting("QueueMaxRetriesNumber"));
                TimeSpan queueRetriesDeltaBackoff = TimeSpan.Parse(this.GetMandatorySetting("QueueRetriesDeltaBackoff"));
                this.hcpCommandsQueue = new HcpCommandsAzureQueue(storageAccountConnectionString, queueMaxRetries, queueRetriesDeltaBackoff);

                // Other settings
                this.mdmApplicationId = int.Parse(this.GetMandatorySetting("MdMApplicationId"));
                this.publicationDescription = this.GetMandatorySetting("PublicationDescription");
                this.publicationId = this.GetMandatorySetting("PublicationId");
                this.publicationName = this.GetMandatorySetting("PublicationName");
                this.publicationOptinLink = this.GetMandatorySetting("PublicationOptinLink");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't initialize email subscriptions host factory");
                throw;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create service host.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="baseAddresses">
        /// The base addresses.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceHost"/>.
        /// </returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            if (serviceType == typeof(SubscriptionLookupService))
            {
                var serviceSettings = new LookupServiceSettings
                                          {
                                              MdmApplicationId = this.mdmApplicationId, 
                                              PublicationId = this.publicationId, 
                                              PublicationDescription = this.publicationDescription, 
                                              PublicationName = this.publicationName, 
                                              PublicationOptinLink = this.publicationOptinLink
                                          };

                return new LookupServiceHost(this.usersDal, serviceSettings, serviceType, baseAddresses);
            }

            if (serviceType == typeof(UnsubscribeService))
            {
                var serviceSettings = new UnsubscribeServiceSettings { MdmApplicationId = this.mdmApplicationId };
                return new UnsubscribeServiceHost(this.hcpCommandsQueue, serviceSettings, serviceType, baseAddresses);
            }

            if (serviceType == typeof(SuppressUserService))
            {
                return new SuppressUserServiceHost(this.hcpCommandsQueue, serviceType, baseAddresses);
            }

            string errorMsg = string.Format("service type: {0} is unknown service type", serviceType.FullName);
            Log.Error(errorMsg);
            throw new ArgumentException(errorMsg, "serviceType");
        }

        /// <summary>
        /// Get mandatory field from configuration 
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns> The match value
        /// </returns>
        /// <exception cref="ConfigurationErrorsException"> Key doesn't exist or the value is null or empty </exception>
        private string GetMandatorySetting(string key)
        {
            string value = CloudConfigurationManager.GetSetting(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new ConfigurationErrorsException(string.Format("Key {0} is missing error value is null or emtpy", key));
            }

            return value;
        }

        #endregion
    }
}