//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Lomo.Authentication.Tokens;
    using Lomo.Authorization;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute a get user token for card operation request.
    /// </summary>
    public class GetSecureCardOperationTokenExecutor
    {
        /// <summary>
        /// Initializes a new instance of the GetSecureCardOperationTokenExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public GetSecureCardOperationTokenExecutor(CommerceContext context)
        {
            Context = context;
            UserOperations = CommerceOperationsFactory.UserOperations(Context);
        }

        /// <summary>
        /// Executes the get user token for card operation invocation.
        /// </summary>
        /// <remarks>
        /// Authenticated user is automatically created within the system if necessary when obtaining a token for Create operations.
        /// </remarks>
        public void Execute()
        {
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];
            Crud crud;
            if (Enum.TryParse<Crud>((string)Context[Key.RequestedCrudOperation], true, out crud) == true)
            {
                // Validate the user ID in the context.
                ResultCode validateUserIdResult = ValidateUserId(crud);
                if (validateUserIdResult == ResultCode.Success || validateUserIdResult == ResultCode.Created)
                {
                    Guid userId = (Guid)Context[Key.GlobalUserId];
                    LomoUserIdSecurityToken token = new LomoUserIdSecurityToken(
                                          userId.ToString(),
                                          CommerceServiceConfig.Instance.Environment,
                                          Resource.Cards.ToString(),
                                          crud.ToString(),
                                          Convert.ToInt64(ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenLifetime]),
                                          ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenSigningKey],
                                          ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenPassword],
                                          ConfigurationManager.AppSettings[AppSettingsKeys.SecureTokenSalt]);
                    ((GetSecureCardOperationTokenResponse)Context[Key.Response]).Token = token.ToString();
                    resultSummary.SetResultCode(ResultCode.Success);
                }
                else
                {
                    resultSummary.SetResultCode(validateUserIdResult);
                }
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.InvalidParameter);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for User operations.
        /// </summary>
        internal IUserOperations UserOperations { get; set; }

        /// <summary>
        /// Validates the user ID in the context.
        /// </summary>
        /// <param name="crud">
        /// The CRUD operation for which a token is being created.
        /// </param>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        /// <remarks>
        /// Authenticated user is automatically created within the system if necessary when validating the user ID in the
        /// context while obtaining a token for Create operations.
        /// </remarks>
        private ResultCode ValidateUserId(Crud crud)
        {
            ResultCode result = ResultCode.Success;

            // Attempt to retrieve the user.
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));
            User user = sharedUserLogic.RetrieveUser();

            // If the user is null and the CRUD operation is Create, implicitly create the user.
            if (user == null)
            {
                if (crud == Crud.Create)
                {
                    if (Context.ContainsKey(Key.CorrelationId) == true)
                    {
                        Guid userId = (Guid)Context[Key.GlobalUserId];
                        user = new User(userId, Guid.NewGuid());
                        Context[Key.User] = user;
                        sharedUserLogic.AddUser();

                        // Update analytics.
                        Analytics.AddRegisterUserEvent(userId, user.AnalyticsEventId, (Guid)Context[Key.CorrelationId], Context[Key.ReferrerId] as string);

                        // Add referral, if any.
                        SharedReferralLogic sharedReferralLogic = new SharedReferralLogic(Context,
                                                                          CommerceOperationsFactory.ReferralOperations(Context));
                        sharedReferralLogic.AddReferral((string)Context[Key.ReferredUserId]);
                    }
                    else
                    {
                        Context.Log.Warning("No correlation ID could be found in the context.");
                        result = ResultCode.ParameterCannotBeNull;
                    }
                }
                else
                {
                    result = ResultCode.UnexpectedUnregisteredUser;
                }
            }

            return result;
        }
    }
}