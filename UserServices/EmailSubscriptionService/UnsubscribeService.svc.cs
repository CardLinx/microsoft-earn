//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The unsubscribe service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    using Lomo.Logging;

    using LoMo.UserServices.Storage.HCP;

    using Microsoft.IT.RelationshipManagement.MDM.Platform.NewsletterUnsubscribe;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UnsubscribeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UnsubscribeService.svc or UnsubscribeService.svc.cs at the Solution Explorer and start debugging.
    
    /// <summary>
    /// The unsubscribe service.
    /// </summary>
    public class UnsubscribeService : IUnsubscribeService
    {
        #region Constants

        /// <summary>
        /// The bad request.
        /// </summary>
        private const string BadRequest = "BadRequest";

        /// <summary>
        /// The server error.
        /// </summary>
        private const string ServerError = "InternalServerError";

        #endregion

        #region Fields

        /// <summary>
        /// The hcp commands queue.
        /// </summary>
        private readonly IHcpCommandsQueue hcpCommandsQueue;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly UnsubscribeServiceSettings settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeService"/> class.
        /// </summary>
        /// <param name="hcpCommandsQueue"> The hcp commands queue. </param>
        /// <param name="settings"> The service settings. </param>
        /// <exception cref="ArgumentNullException">settings or hcp commands queue are null
        /// </exception>
        public UnsubscribeService(IHcpCommandsQueue hcpCommandsQueue, UnsubscribeServiceSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (hcpCommandsQueue == null)
            {
                throw new ArgumentNullException("hcpCommandsQueue");
            }

            this.hcpCommandsQueue = hcpCommandsQueue;
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Notify unsubscribe request
        /// </summary>
        /// <param name="obaUnsubscribe">
        /// The oba Unsubscribe.
        /// </param>
        /// <exception cref="FaultException">
        /// Error during executing the request 
        /// </exception>
        public void Notify(ObaUnsubscribe obaUnsubscribe)
        {
            if (obaUnsubscribe == null)
            {
                const string ErrorMsg = "Bad client request. Null request recieved";
                Log.Warn(ErrorMsg);
                throw new FaultException(ErrorMsg, new FaultCode(BadRequest));
            }

            Guid eventCorrelationId = obaUnsubscribe.CorrelationId;
            Guid eventInstanceId = obaUnsubscribe.EventInstanceId;
            string correlationTicket = obaUnsubscribe.CorrelationTicket;
            string entityId = obaUnsubscribe.EntityId;
            
            var request = obaUnsubscribe.Body;

            var context = new RequestContext { CorrelationId = eventCorrelationId, EventInstanceId = eventInstanceId, CorrelationTicket = correlationTicket, EntityId = entityId };
            if (request == null)
            {
                const string ErrorMsg = "Bad client request. Null request recieved";
                const string ErrorMsgTemplate = "Request Context={0}; Error={1}";
                Log.Warn(ErrorMsgTemplate, context, ErrorMsg);
                throw new FaultException(string.Format(ErrorMsgTemplate, context, ErrorMsg), new FaultCode(BadRequest));
            }

            if (string.IsNullOrEmpty(entityId))
            {
                const string ErrorMsg = "Bad client request. entity id is null or empty";
                const string ErrorMsgTemplate = "Request Context={0}; Error={1}";
                Log.Warn(ErrorMsgTemplate, context, ErrorMsg);
                throw new FaultException(string.Format(ErrorMsgTemplate, context, ErrorMsg), new FaultCode(BadRequest));
            }

            var hcpCommand = new UnsubscribeCommand { CorrelationTicket = correlationTicket, EventCorrelationId = eventCorrelationId, EventInstanceId = eventInstanceId, EmailAddress = entityId, RequestTimeUtc = request.DateTimeUtcUnsubscribed };
            switch (request.NotificationType)
            {
                // Unsubscribe All
                case NotificationType.UA:
                    hcpCommand.IsUnsubscribeAll = true;
                    break;

                // Targeted unsubscribe
                case NotificationType.TU:
                    hcpCommand.IsUnsubscribeAll = false;
                    hcpCommand.PublicationIds = this.GetPublicationIds(context, request);
                    break;

                    // Unsupported command
                default:
                    string errorMsg = string.Format("Bad client request. Unknown notification type: {0}", request.NotificationType);
                    const string ErrorMsgTemplate = "Request Context={0}; Error={1}";
                    Log.Warn(ErrorMsgTemplate, context, errorMsg);
                    throw new FaultException(string.Format(ErrorMsgTemplate, context, errorMsg), new FaultCode(BadRequest));
            }

            try
            {
                Log.Verbose("Enqueuing Unsubscribe Request. Details: {0}", hcpCommand);
                this.hcpCommandsQueue.EnqueueCommand(hcpCommand);
                Log.Verbose("Unsubscribe Request Enqueued. Details: {0}", hcpCommand);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while calling hcp commands queue. Request Context={0}", context);
                throw new FaultException(string.Format("Internal Server Error. Request Context={0}", context), new FaultCode(ServerError));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The publication ids to unsubscribe. </summary>
        /// <param name="requestContext"> The request context. </param>
        /// <param name="request"> The request. </param>
        /// <returns>
        /// The publication ids list
        /// </returns>
        /// <exception cref="FaultException"> Error while processing the request </exception>
        private List<string> GetPublicationIds(RequestContext requestContext, UnsubscribeRequestData request)
        {
            if (request.SubscribedPublications == null || request.SubscribedPublications.LstPublisher == null)
            {
                const string ErrorMsg = "Bad client request. Subscribed Publication List is empty for targeted unsubscribe";
                const string ErrorMsgTemplate = "Request Context={0}; Error={1}";
                Log.Warn(ErrorMsgTemplate, requestContext, ErrorMsg);
                throw new FaultException(string.Format(ErrorMsgTemplate, requestContext, ErrorMsg), new FaultCode(BadRequest));
            }

            var publications = new HashSet<string>();
            foreach (Publisher publisher in request.SubscribedPublications.LstPublisher)
            {
                if (publisher.MdmApplicationId == this.settings.MdmApplicationId)
                {
                    if (publisher.LstPublicationSubscribed != null)
                    {
                        foreach (PublicationSubscribed publication in publisher.LstPublicationSubscribed)
                        {
                            if (publication != null && !string.IsNullOrEmpty(publication.Id))
                            {
                                publications.Add(publication.Id);
                            }
                            else
                            {
                                // This is invalid state. Log error and continue
                                Log.Error("Null or empty publication id received for unsubscribe request. Request Context={0}", requestContext);
                            }
                        }
                    }
                }
            }

            return publications.ToList();
        }

        #endregion
    }
}