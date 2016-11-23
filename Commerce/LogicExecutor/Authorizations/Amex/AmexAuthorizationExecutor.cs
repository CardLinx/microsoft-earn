//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Notifications;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to process the Amex Auth Request
    /// </summary>
    public class AmexAuthorizationExecutor
    {
        /// <summary>
        /// Initializes a new instance of the AmexAuthorizationExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context for the API being invoked.
        /// </param>
        public AmexAuthorizationExecutor(CommerceContext context)
        {
            Context = context;
            Context[Key.Partner] = Partner.Amex;
        }

        /// <summary>
        /// Executes processing of the amex auth request.
        /// </summary>
        public ResultCode Execute()
        {
            ResultCode result = ResultCode.None;
            SharedAuthorizationLogic sharedAuthorizationLogic = new SharedAuthorizationLogic(Context, CommerceOperationsFactory.AuthorizationOperations(Context));
            Authorization authorization = new Authorization();
            Context[Key.Authorization] = authorization;
            Amex amex = new Amex(Context);
            amex.MarshalAuthorization();
            result = sharedAuthorizationLogic.AddAuthorization();

            // Build the response to send back to Amex based on the result of adding the Authorization.
            Context[Key.ResultCode] = result;
            amex.BuildAuthorizationResponse();
            if (result == ResultCode.Created)
            {
                Context.Log.Verbose("Authorization successfully persisted, now send notification");
                
                // Send notification.
                NotifyAuthorization notifyAuthorization = new NotifyAuthorization(Context);
                Context[Key.CardBrand] = CardBrand.AmericanExpress;
                notifyAuthorization.SendNotification();
                Context.Log.Verbose("Authorization notification sent");
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}