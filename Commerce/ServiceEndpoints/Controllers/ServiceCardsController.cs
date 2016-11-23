//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// V2 Cards API controller for LoMo Commerce Web service.
    /// </summary>
    public class ServiceCardsController : ApiController
    {
        /// <summary>
        /// Retrieves the cards registered to the authenticated user.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user whose cards to retrieve.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a ServiceGetCardsResponse with detailed result information.
        /// </returns>
        [SimpleWebTokenAuth("cards")]
        public HttpResponseMessage Get(Guid userId)
        {
            // get cards by user
            return Cards(userId, RewardPrograms.All, Partner.FirstData);
        }

        /// <summary>
        /// Retrieves all active cards in the system.
        /// </summary>
        /// <returns>
        /// An HttpResponseMessage containing a ServiceGetCardsResponse with detailed result information.
        /// </returns>
        [SimpleWebTokenAuth("cards")]
        public HttpResponseMessage Get()
        {
            // get all cards : user id is empty
            return Cards(Guid.Empty, RewardPrograms.All, Partner.FirstData);
        }

        /// <summary>
        /// Gets cards by user if specified else all active cards
        /// </summary>
        /// <param name="userId">
        /// if userid is non empty guid, get active cards by user, else all active cards
        /// </param>
        /// <param name="rewardPrograms">
        /// The reward programs by which to filter cards.
        /// </param>
        /// <param name="partner">
        /// The partner for which to retrieve cards.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a ServiceGetCardsResponse with detailed result information.
        /// </returns>
        /// <remarks>
        /// The partner parameter should properly default to Partner.All, but Partner enum was never intended for bitmasking, so there is no Partner.All.
        ///  But for now, None can be used to tell the sproc not to constrain on partner. Longer term it would be a good idea to convert this enum to Flags type
        ///  and update the database accordingly.
        /// </remarks>
        private HttpResponseMessage Cards(Guid userId,
                                          RewardPrograms rewardPrograms,
                                          Partner partner)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get cards", Request, new ServiceGetCardsResponse(),
                                                                                  callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented credentials are for role \"{0}\" and include token \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);

                context[Key.GlobalUserId] = userId;
                context[Key.RewardProgramType] = rewardPrograms;
                context[Key.Partner] = partner;

                // Create an executor object to execute the API invocation.
                ServiceGetCardsExecutor getCardsExecutor = new ServiceGetCardsExecutor(context);
                getCardsExecutor.Execute();

                // Build the response from the result of API invocation.
                result = RestResponder.BuildSynchronousResponse(context);
            }
            catch (Exception ex)
            {
                result = RestResponder.BuildSynchronousResponse(context, ex);
            }

            return result;
        }
    }
}