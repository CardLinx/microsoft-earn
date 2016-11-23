//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// V2 Fast Cards API controller for the LoMo Commerce web site.
    /// </summary>
    public class V2FastCardsController : ApiController
    {
        /// <summary>
        /// Creates a new, limited access account for the specified e-mail address, adds a new card for that user, and queues
        /// claiming already claimed deals for the card.
        /// </summary>
        /// <param name="payload">
        /// The payload that contians info on the card to add.
        /// </param>
        /// <returns>
        /// A Task that will, via an HttpResponseMessage, yield an AddCardResponse with detailed result information.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter payload cannot be null.
        /// </exception>
        [HttpPost]
        public Task<HttpResponseMessage> Add(AddCardPayload payload)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            if (payload == null)
            {
                throw new ArgumentNullException("payload", "Parameter payload cannot be null.");
            }

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildAsynchronousRestContext("Create account for unauthenticated user, add card, and queue claiming deals",
                                                                                   Request, new UnauthenticatedAddCardResponse(), callTimer);
            context.Log.Information("Processing {0} call.", context.ApiCallDescription);

            // Generate a legacy NewCardInfo object from the specified number.
            NewCardInfo newCardInfo = ControllerUtilities.GenerateLegacyCardInfo(new NewCardNumber { Number = payload.Number });
            ControllerUtilities.LogObfuscatedNewCardInfo(newCardInfo, context);

            // Add parameters of the call to the context.
            context[Key.CreateUnauthenticatedAccount] = true;
            context[Key.EmailAddress] = payload.Email;
            context[Key.ReferrerId] = payload.Referrer;
            context[Key.UserLocation] = payload.UserLocation;
            context[Key.ReferralEvent] = ReferralEvent.Signup;
            context[Key.QueueJob] = true;
            context[Key.NewCardInfo] = newCardInfo;
            context[Key.RewardProgramType] = ControllerUtilities.GetRewardProgramAssociatedWithRequest(this.Request);

            // Create an executor object to execute the API invocation.
            AddCardExecutor executor = new AddCardExecutor(context);
            Task.Factory.StartNew(async () => await executor.Execute());

            return ((TaskCompletionSource<HttpResponseMessage>)context[Key.TaskCompletionSource]).Task;
        }
    }
}