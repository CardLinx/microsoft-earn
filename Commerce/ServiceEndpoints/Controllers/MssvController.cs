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
    using System.Threading.Tasks;
    using System.Web.Http;
    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Microsoft Store Voucher controller for LoMo Commerce Web service.
    /// </summary>
    public class MssvController : ApiController
    {
        /// <summary>
        /// Distributes the specified amount of credits as Microsoft Store vouchers for the specified user.
        /// </summary>
        /// <param name="request">
        /// The object that contains the user, distribution amount, and other pertinent data.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a DistributeMssvResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        [ActionName("Distribute")]
        [HttpPost]
        public HttpResponseMessage Distribute(MssvDistributionDataContract request)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            if (request == null)
            {
                throw new ArgumentNullException("request", "Parameter request cannot be null.");
            }

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Distribute Microsoft Store vouchers", Request,
                                                                                   new DistributeMssvResponse(), callTimer);
            try
            {
                // Add request information to the call context.
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                context[Key.GlobalUserId] = CommerceContext.PopulateUserId(context);
                context[Key.DistributionAmount] = request.DistributionAmount;
                context[Key.VoucherExpirationUtc] = request.VoucherExpirationUtc;
                context[Key.Notes] = request.Notes;
                context.Log.Verbose("{0} request:\r\n{1}", context.ApiCallDescription, General.SerializeJson(request));

                // Create an executor object to execute the API invocation.
                DistributeMssvExecutor executor = new DistributeMssvExecutor(context);
                executor.Execute();

                result = RestResponder.BuildSynchronousResponse(context);
            }
            catch (Exception ex)
            {
                result = RestResponder.BuildSynchronousResponse(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the Microsoft Store Voucher history for the current user.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user for whom to retrieve history.
        /// </param>
        /// <returns>
        /// An HttpResponseMessage containing a GetMssVoucherDistributionHistoryResponse with detailed result information.
        /// </returns>
        [ApiAuth]
        [ActionName("GetHistory")]
        [HttpGet]
        public HttpResponseMessage RetrieveHistory()
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            HttpResponseMessage result;

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("Get Microsoft Store voucher distribution history", Request,
                                                                                   new GetMssVoucherDistributionHistoryResponse(), callTimer);
            try
            {
                // Add request information to the call context.
                context.Log.Information("Processing {0} call.", context.ApiCallDescription);
                Guid userId = CommerceContext.PopulateUserId(context);
                context[Key.GlobalUserId] = userId;
                context.Log.Verbose("{0} request for user {1}", context.ApiCallDescription, userId);

                // Create an executor object to execute the API invocation.
                MssvDistributionHistoryExecutor executor = new MssvDistributionHistoryExecutor(context);
                executor.Execute();

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