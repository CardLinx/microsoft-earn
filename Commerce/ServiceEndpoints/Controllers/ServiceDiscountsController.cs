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
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Discounts Controller for Lomo Commerce Web Service
    /// </summary>
    public class ServiceDiscountsController : ApiController
    {
        /// <summary>
        /// Get all active discount ids in the system
        /// </summary>
        /// <param name="reimbursementTender">
        /// The reimbursement tender the discount must include to be returned in the list. Defaults to ReimbursementTender.DealCurrency.
        /// </param>
        /// <param name="partner">
        /// The partner whose discounts to retrieve. Defaults to Partner.None.
        /// </param>
        /// <returns>
        /// HttpResponseMessage, yield a GetActiveDiscountIdsResponse with detailed result information.
        /// </returns>
        /// <remarks>
        /// Both the reimbursementTender and partner parameters should properly default to All, but neither enum was intended for bitmasking, so there is no All.
        ///  But for now, None can be used to tell the sproc not to constrain on partner. Longer term it would be a good idea to convert these enums to Flags type
        ///  and update the database accordingly.
        /// </remarks>
        [SimpleWebTokenAuth("discounts")]
        [HttpGet]
        public HttpResponseMessage ActiveDiscountIds(ReimbursementTender reimbursementTender = ReimbursementTender.DeprecatedEarn,
                                                     Partner partner = Partner.None)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get Active Discounts", Request, new GetActiveDiscountIdsResponse(),
                                                                                  callTimer);
            try
            {
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                CustomIdentity clientIdentity = (CustomIdentity)Thread.CurrentPrincipal.Identity;
                context.Log.Verbose("Presented credentials are for role \"{0}\" and include token \"{1}\".",
                                    clientIdentity.Name, clientIdentity.PresentedClientToken);

                context[Key.ReimbursementTender] = reimbursementTender;
                context[Key.Partner] = partner;

                // Create an executor object to execute the API invocation.
                GetActiveDiscountsExecutor discountsExecutor = new GetActiveDiscountsExecutor(context);
                discountsExecutor.Execute();

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