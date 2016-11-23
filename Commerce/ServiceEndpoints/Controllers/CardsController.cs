//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Cards API controller for LoMo Commerce Web service.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class CardsController : ApiController
    {
        /// <summary>
        /// Removes the card with the specified ID if it belongs to the authenticated user.
        /// </summary>
        /// <param name="cardId">
        /// The ID of the card to remove.
        /// </param>
        /// <returns>
        /// A Task that will, via an HttpResponseMessage, yield a RemoveCardResponse with detailed result information.
        /// </returns>
        /// <remarks>
        /// **** This method performs a DELETE operation-- Bing FrontDoor does not support the DELETE verb! ****
        /// **** Hackity hackity hack-hack-hack! ****
        /// </remarks>
        [ApiAuth]
        [HttpPost]
        public Task<HttpResponseMessage> Delete(Guid cardId)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            Task<HttpResponseMessage> result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildAsynchronousRestContext("Remove card", Request,
                                                                                   new RemoveCardResponse(), callTimer);
            try
            {
                context[Key.CardId] = General.IntegerFromGuid(cardId);
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                context.Log.Verbose("{0} request:\r\ncardId={1}", context.ApiCallDescription, cardId);

                // Add ID for the user making this call to the context.
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.RewardProgramType] = ControllerHelper.GetRewardProgramAssociatedWithRequest(this.Request);

                // Create an executor object to execute the API invocation.
                RemoveCardExecutor removeCardExecutor = new RemoveCardExecutor(context);
                Task.Factory.StartNew(async () => await removeCardExecutor.Execute());

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