//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Contains utility methods for partner implementations.
    /// </summary>
    internal static class PartnerUtilities
    {
        /// <summary>
        /// Invokes a call to a partner API method, retrying if needed.
        /// </summary>
        /// <param name="context">
        /// The context of the operation.
        /// </param>
        /// <param name="invoker">
        /// The function to call to invoke the partner API method.
        /// </param>
        /// <param name="terminalCodes">
        /// Set of terminal result codes on which we should not retry. 
        /// For example, if result is invalid card, one should not need to try again.
        /// </param>
        /// <param name="partner">
        /// Optionally, callers can specify their partner.
        /// </param>
        /// <param name="retryOnTransientErrorOnly">
        /// Optionally, settting this as true will retry on transientErrorOnly which in this case mean that if there is an exception
        /// </param>
        internal static async Task<ResultCode> InvokePartner(CommerceContext context,
                                                             Func<Task<ResultCode>> invoker,
                                                             HashSet<ResultCode> terminalCodes = null,
                                                             Partner partner = Partner.None, bool retryOnTransientErrorOnly = false)
        {
            ResultCode result = ResultCode.None;

            // initialize terminal codes if not provided
            if (terminalCodes == null)
            {
                terminalCodes = new HashSet<ResultCode>();
                terminalCodes.Add(ResultCode.Success);
                terminalCodes.Add(ResultCode.Created);
            }

            // Invoke the partner, retrying if needed.
            CommerceConfig config = context.Config ?? CommerceServiceConfig.Instance;

            int tryCount = 0;
            int maxRetries = config.MaxPartnerRetries;
            int retryLatency = config.InitialPartnerRetryLatency;

            do
            {
                try
                {
                    tryCount++;
                    result = await invoker().ConfigureAwait(false);

                    //we consider transient error as a runtime exception while calling external API. Since there is no exception and retryOnTransientErrorOnly is set 
                    //to true we will break
                    if (retryOnTransientErrorOnly)
                    {
                        break;
                    }
                }
                catch(Exception ex)
                {
                    bool firstDataBadRequest = partner == Partner.FirstData && ex.Message.Contains("(400) Bad Request");
                    string message = String.Format("{0} call encountered an error.", context.ApiCallDescription);
                    if (tryCount <= maxRetries)
                    {
                        if (firstDataBadRequest == false)
                        {
                            context.Log.Critical(message, ex);
                        }
                        else
                        {
                            context.Log.Warning(String.Format("First Data {0}: (400) Bad Request detected. This is common because of clock skew and should resolve itself when the job is retried.", message));
                        }
                    }
                    else
                    {
                        throw;
                    }
                }

                // If a rety is needed, wait a short but increasingly lengthy time before proceeding.
                if (tryCount <= maxRetries && !terminalCodes.Contains(result))
                {
                    context.Log.Verbose("Waiting {0} milliseconds before retrying partner invocation.", retryLatency);
                    Thread.Sleep(retryLatency);
                    retryLatency *= 2;
                }
            }
            while (tryCount <= maxRetries && result != ResultCode.Success && result != ResultCode.Created);

            return result;
        }
    }
}