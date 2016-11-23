//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The subscription lookup service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace LoMo.EmailSubscription.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Logging;

    using Microsoft.IT.RelationshipManagement.MDM.LookupSubscription;
    using Microsoft.IT.RelationshipManagement.MDM.Platform.NewsletterUnsubscribe;

    using BackgroundWorker;
    using Users.Dal;
    using Users.Dal.DataModel;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SubscriptionLookupService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SubscriptionLookupService.svc or SubscriptionLookupService.svc.cs at the Solution Explorer and start debugging.
    
    /// <summary>
    /// The subscription lookup service.
    /// </summary>
    public class SubscriptionLookupService : ILookupService
    {
        private class RefreshWorkItem : IWorkItem
        {
            private readonly SubscriptionLookupService subscriptionLookupService;
            public RefreshWorkItem(SubscriptionLookupService lookupService)
            {
                this.subscriptionLookupService = lookupService;

            }
            public void ExecuteWorkItem()
            {
               subscriptionLookupService.RefreshEmailSubscribers();
            }
        }

        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        /// <summary>
        /// The lookup service settings
        /// </summary>
        private readonly LookupServiceSettings serviceSettings;

        private static List<string> _emailSubscribers;

        /// <summary>
        /// Interval in minutes to refresh the in memory list of email subscribers from the db
        /// </summary>
        private const int RefreshIntervalInMinutes = 5;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionLookupService"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="serviceSettings">the lookup service settings</param>
        public SubscriptionLookupService(IUsersDal usersDal, LookupServiceSettings serviceSettings)
        {
            Log.Info("Instantiating the Subscription Lookup Service");
            if (serviceSettings == null)
            {
                throw new ArgumentNullException("serviceSettings");
            }

            if (string.IsNullOrEmpty(serviceSettings.PublicationDescription) || 
                string.IsNullOrEmpty(serviceSettings.PublicationId) || 
                string.IsNullOrEmpty(serviceSettings.PublicationName) || 
                string.IsNullOrEmpty(serviceSettings.PublicationOptinLink))
            {
                throw new ArgumentException("one or more of the parameter properties have invalid value", "serviceSettings");
            }

            this.usersDal = usersDal;
            this.serviceSettings = serviceSettings;
            _emailSubscribers = new List<string>();

            Log.Info("Creating the Background worker to refresh the in memory email subscribers list every {0} minute(s) from db", RefreshIntervalInMinutes);
            Worker worker = new Worker(new RefreshWorkItem(this), 1000 * 60 * RefreshIntervalInMinutes);
            worker.StartWorker();
            
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The lookup subscription.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="entityTypeId">
        /// The entity type id.
        /// </param>
        /// <param name="includeSensitiveSubscriptions">
        /// The include sensitive subscriptions.
        /// </param>
        /// <returns>
        /// The <see cref="SubscribedPublications"/>.
        /// </returns>
        public SubscribedPublications LookupSubscription(string entityId, int entityTypeId, bool includeSensitiveSubscriptions)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Publisher> publishers = this.LookupSubscriptions(entityId);
            sw.Stop();
            Log.Info("Time to lookup subscription (in ms) : {0}", sw.ElapsedMilliseconds);

            return new SubscribedPublications { LstPublisher = publishers };

        }

        /// <summary>
        /// The lookup subscription v 2.
        /// </summary>
        /// <param name="lookupSubscriptionRequestData">
        /// The lookup subscription request data.
        /// </param>
        /// <returns>
        /// The <see cref="SubscribedPublicationsV2"/>.
        /// </returns>
        public SubscribedPublicationsV2 LookupSubscriptionV2(LookupSubscriptionRequestData lookupSubscriptionRequestData)
        {
            string emailAddress = lookupSubscriptionRequestData != null ? lookupSubscriptionRequestData.EntityId : null;
            List<Publisher> publishers = this.LookupSubscriptions(emailAddress);
            return new SubscribedPublicationsV2 { LstPublisher = publishers };
        }

        /// <summary>
        /// The lookup subscriptions.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address.
        /// </param>
        /// <returns>
        /// The list of publishers
        /// </returns>
        private List<Publisher> LookupSubscriptions(string emailAddress)
        {
            try
            {
                if (emailAddress == null)
                {
                    throw new ArgumentNullException("emailAddress");
                }

                Log.Verbose("Starting Subscriptions Lookup. Email={0}", emailAddress);

                var publisher = 
                    new Publisher 
                    { 
                        MdmApplicationId = this.serviceSettings.MdmApplicationId, 
                        LstPublicationSubscribed = new List<PublicationSubscribed>(), 
                        LookupStatus = LookupStatus.Success 
                    };
                
                if (_emailSubscribers.Any())
                {
                    Log.Info("In memory list has {0} items...Will check this instead of db", _emailSubscribers.Count);
                    if (_emailSubscribers.Contains(emailAddress, StringComparer.CurrentCultureIgnoreCase))
                    {
                        Log.Info("Found the email address {0} in the in memory list ", emailAddress);
                        var publicationSubscribed = new PublicationSubscribed
                        {
                            Description = this.serviceSettings.PublicationDescription,
                            Name = this.serviceSettings.PublicationName,
                            Id = this.serviceSettings.PublicationId,
                            OptInLnk = this.serviceSettings.PublicationOptinLink,
                        };
                        publisher.LstPublicationSubscribed.Add(publicationSubscribed);
                    }
                }
                else
                {
                    Log.Info("In memory list is empty...will check the db for email address lookup");
                    var emailSubscriptions = this.usersDal.GetEmailSubscriptionsByEmail(emailAddress, true);
                    if (emailSubscriptions.Any())
                    {
                        var publicationSubscribed = new PublicationSubscribed
                                                                          {
                                                                              Description = this.serviceSettings.PublicationDescription,
                                                                              Name = this.serviceSettings.PublicationName,
                                                                              Id = this.serviceSettings.PublicationId,
                                                                              OptInLnk = this.serviceSettings.PublicationOptinLink,
                                                                          };
                        publisher.LstPublicationSubscribed.Add(publicationSubscribed);
                    }
                }

                Log.Verbose("Subscriptions Lookup Completed. Email={0}", emailAddress);

                return new List<Publisher> { publisher };
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while lookup user subscriptions. Email={0}", emailAddress);

                return new List<Publisher>
                           {
                               new Publisher
                                   {
                                       MdmApplicationId = this.serviceSettings.MdmApplicationId, 
                                       LookupStatus = LookupStatus.Failure,
                                       LstPublicationSubscribed = new List<PublicationSubscribed>()
                                   }
                           };
            }
        }

        public void RefreshEmailSubscribers()
        {
            Log.Info("Beginning to refresh the list of email subscribers");
            List<string> lstEmailSubscribers = new List<string>();
            object continuationContext = null;

            try
            {
                bool hasMore = true;
                while (hasMore)
                {
                    EmailsSubscriptionsBatchResponse response = usersDal.GetNextEmailSubscriptionsBatch(10000, true, continuationContext, SubscriptionType.WeeklyDeals);
                    if (response.EmailSubscriptions != null)
                    {
                        foreach (Users.Dal.DataModel.EmailSubscription emailSubscription in response.EmailSubscriptions)
                        {
                            if (!string.IsNullOrWhiteSpace(emailSubscription.Email))
                            {
                                lstEmailSubscribers.Add(emailSubscription.Email);
                            }
                        }
                    }

                    hasMore = response.HasMore;
                    continuationContext = response.ContinuationContext;
                }

                if (lstEmailSubscribers.Any())
                {
                    _emailSubscribers = lstEmailSubscribers;
                }

                Log.Info("Finished refreshing the list of email subscribers");
                Log.Info("After refresh, In memory list has " + lstEmailSubscribers.Count + " items");
            }
            catch (Exception exception)
            {
                Log.Error("Error in refreshing list of email subscribers : {0}", exception.Message);
            }

        }

        #endregion
    }
}