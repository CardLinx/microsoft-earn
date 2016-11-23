//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using System.Diagnostics;
    using System.Linq;
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
    /// V2 Cards API controller for the LoMo Commerce web site.
    /// </summary>
    public class V2CardsController : ApiController
    {
        /// <summary>
        /// Adds a new card to the authenticated user and queues claiming already claimed deals for the card.
        /// </summary>
        /// <param name="newCardNumber">
        /// The number of the new credit card.
        /// </param>
        /// <returns>
        /// A Task that will, via an HttpResponseMessage, yield an AddCardResponse with detailed result information.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter newCardNumber cannot be null.
        /// </exception>
        [ApiAuth(new string[] { "Resource:Cards", "Action:Create" }, null)]
        [HttpPost]
        public Task<HttpResponseMessage> Add(NewCardNumber newCardNumber)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            Task<HttpResponseMessage> result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildAsynchronousRestContext("Add card and queue claiming already claimed deals",
                                                                                   Request, new AddCardResponse(), callTimer);
            try
            {
                if (newCardNumber == null)
                {
                    throw new ArgumentNullException("newCardNumber", "Parameter newCardNumber cannot be null.");
                }

                context.Log.Information("Processing {0} call.", context.ApiCallDescription);

                // Generate a legacy NewCardInfo object from the specified number.
                NewCardInfo newCardInfo = ControllerUtilities.GenerateLegacyCardInfo(newCardNumber);
                ControllerUtilities.LogObfuscatedNewCardInfo(newCardInfo, context);

                // Add parameters of the call to the context.
                context[Key.QueueJob] = true;
                context[Key.NewCardInfo] = newCardInfo;
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.ReferrerId] = newCardNumber.Referrer;
                context[Key.RewardProgramType] = ControllerUtilities.GetRewardProgramAssociatedWithRequest(this.Request);

                // Create an executor object to execute the API invocation.
                AddCardExecutor executor = new AddCardExecutor(context);
                Task.Factory.StartNew(async () => await executor.Execute());

                result = ((TaskCompletionSource<HttpResponseMessage>)context[Key.TaskCompletionSource]).Task;
            }
            catch (Exception ex)
            {
                result = RestResponder.CreateExceptionTask(context, ex);
            }

            return result;
        }
    }
}