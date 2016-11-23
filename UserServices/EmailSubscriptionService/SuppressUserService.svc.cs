//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The suppress user service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.EmailSubscription.Service
{
    using System;
    using System.ServiceModel;

    using Lomo.Logging;

    using LoMo.UserServices.Storage.HCP;

    /// <summary>
    /// The suppress user service.
    /// </summary>
    public class SuppressUserService : ISuppressUserService
    {
        /// <summary>
        /// The bad request.
        /// </summary>
        private const string BadRequest = "BadRequest";

        /// <summary>
        /// The server error.
        /// </summary>
        private const string ServerError = "InternalServerError";

        /// <summary>
        /// The suppress operation id.
        /// </summary>
        private const short SuppressOperationId = 1;

        /// <summary>
        /// The unsuppress operation id.
        /// </summary>
        private const short UnsuppressOperationId = 2;

        /// <summary>
        /// The hcp commands queue.
        /// </summary>
        private readonly IHcpCommandsQueue hcpCommandsQueue;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressUserService"/> class.
        /// </summary>
        /// <param name="hcpCommandsQueue"> The hcp commands queue. </param>
        /// <exception cref="ArgumentNullException">settings or hcp commands queue are null </exception>
        public SuppressUserService(IHcpCommandsQueue hcpCommandsQueue)
        {
            if (hcpCommandsQueue == null)
            {
                throw new ArgumentNullException("hcpCommandsQueue");
            }

            this.hcpCommandsQueue = hcpCommandsQueue;
        }

        #endregion

        /// <summary>
        /// The notify.
        /// </summary>
        /// <param name="obaEmailPermissionUpdateV2">
        /// The oba Email Permission Update V 2.
        /// </param>
        public void Notify(ObaEmailPermissionUpdateV2 obaEmailPermissionUpdateV2)
        {
            if (obaEmailPermissionUpdateV2 == null)
            {
                const string ErrorMsg = "Bad client request. Null request recieved";
                Log.Warn(ErrorMsg);
                throw new FaultException(ErrorMsg, new FaultCode(BadRequest));
            }

            Guid eventCorrelationId = obaEmailPermissionUpdateV2.CorrelationId;
            Guid eventInstanceId = obaEmailPermissionUpdateV2.EventInstanceId;
            string correlationTicket = string.Empty;
            string entityId = obaEmailPermissionUpdateV2.EntityId;
            var request = obaEmailPermissionUpdateV2.Body;
            var context = new RequestContext { CorrelationId = eventCorrelationId, EventInstanceId = eventInstanceId, CorrelationTicket = correlationTicket, EntityId = entityId };
            if (request == null || request.EmailPrivacyData == null)
            {
                const string ErrorMsg = "Bad client request. Invalid request recieved, request or request privacy data is empty";
                const string ErrorMsgTemplate = "Request Context={0}; Error={1}";
                Log.Warn(ErrorMsgTemplate, context, ErrorMsg);
                throw new FaultException(string.Format(ErrorMsgTemplate, context, ErrorMsg), new FaultCode(BadRequest));
            }

            if (!request.EmailPrivacyData.SuppressedForAllContactID.HasValue)
            {
                const string ErrorMsg = "Bad client request. Invalid request recieved, SuppressedForAllContactID property is null";
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
            
            var hcpCommand = new SuppressUserCommand { CorrelationTicket = correlationTicket, EventCorrelationId = eventCorrelationId, EventInstanceId = eventInstanceId, EmailAddress = entityId, RequestTimeUtc = DateTime.UtcNow };
            switch (request.EmailPrivacyData.SuppressedForAllContactID)
            {
                case SuppressOperationId:
                    hcpCommand.Operation = SuppressOperation.Suppress;
                    break;
                case UnsuppressOperationId:
                    hcpCommand.Operation = SuppressOperation.Unsuppress;
                    break;
                default:
                    string errorMsg = string.Format("Bad client request. Unknown value for SuppressedForAllContantID operation: {0}", request.EmailPrivacyData.SuppressedForAllContactID);
                    const string ErrorMsgTemplate = "Request Context={0}; Error={1}";
                    Log.Warn(ErrorMsgTemplate, context, errorMsg);
                    throw new FaultException(string.Format(ErrorMsgTemplate, context, errorMsg), new FaultCode(BadRequest));
            }

            try
            {
                Log.Verbose("Enqueuing Suppress User Request. Details: {0}", hcpCommand);
                this.hcpCommandsQueue.EnqueueCommand(hcpCommand);
                Log.Verbose("Suppress User Request Enqueued. Details: {0}", hcpCommand);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while calling hcp commands queue. Request Context={0}", context);
                throw new FaultException(string.Format("Internal Server Error. Request Context={0}", context), new FaultCode(ServerError));
            }
        }
    }
}