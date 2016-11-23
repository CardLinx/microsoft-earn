//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Threading.Tasks;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Notifications;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to process MasterCard authorization requests.
    /// </summary>
    public class MasterCardAuthorizationExecutor
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardAuthorizationExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context for the API being invoked.
        /// </param>
        public MasterCardAuthorizationExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.Partner] = Partner.MasterCard;
        }

        /// <summary>
        /// Executes processing of the authorization event info.
        /// </summary>
        public ResultCode Execute()
        {
            ResultCode result = ResultCode.None;

            SharedAuthorizationLogic sharedAuthorizationLogic = new SharedAuthorizationLogic(Context,
                                                                               CommerceOperationsFactory.AuthorizationOperations(Context));

            // Marshal MasterCard Authorization request object into partner-agnostic model.
            Authorization authorization = new Authorization();
            Context[Key.Authorization] = authorization;
            MasterCard masterCard = new MasterCard(Context);
            masterCard.MarshalAuthorization();

            // Add the authorization even to the data store.
            result = sharedAuthorizationLogic.AddAuthorization();

            // Build the response to send back to MasterCard based on the result of adding the Authorization.
            Context[Key.ResultCode] = result;
            masterCard.BuildAuthorizationResponse();

            // If the authorization event was successfully committed to the data store, send user notifications.
            if (result == ResultCode.Created)
            {
                Context.Log.Verbose("MasterCard Authorization successfully persisted. Sending notifications asynchronously.");
                NotifyAuthorization notifyAuthorization = new NotifyAuthorization(Context);
                Context[Key.CardBrand] = CardBrand.MasterCard;
                Task.Run(new Action(notifyAuthorization.SendNotification));
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}