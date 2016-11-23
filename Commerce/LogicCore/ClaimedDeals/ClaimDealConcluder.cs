//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Threading.Tasks;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Notifications;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to conclude the execution of a claim deal request.
    /// </summary>
    public class ClaimDealConcluder
    {
        /// <summary>
        /// Initializes a new instance of the ClaimDealConcluder class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        public ClaimDealConcluder(CommerceContext context)
        {
            Context = context;
            ClaimedDealOperations = CommerceOperationsFactory.ClaimedDealOperations(Context);
        }

        /// <summary>
        /// Concludes execution of the Add card request after previous work has been completed.
        /// </summary>
        /// <param name="resultCode">
        /// The ResultCode to set within the call response.
        /// </param>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public void Conclude(ResultCode resultCode)
        {
            try
            {
                // Set the Response ResultCode as needed.
                Context.Log.Verbose("ResultCode when Conclude process begins: {0}.", resultCode);

                // If process succeeded, update internal data storage.
                if (resultCode == ResultCode.Success)
                {
                    // Add record of the claimed deal.
                    resultCode = AddClaimedDeal();
                }

                ((ClaimDealResponse)Context[Key.Response]).ResultSummary.SetResultCode(resultCode);
                RestResponder.BuildAsynchronousResponse(Context);
            }
            catch (Exception ex)
            {
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for ClaimedDeal operations.
        /// </summary>
        internal IClaimedDealOperations ClaimedDealOperations { get; set; }

        /// <summary>
        /// Adds record of the claimed deal to the data store and logs accordingly.
        /// </summary>
        private ResultCode AddClaimedDeal()
        {
            ResultCode result;

            Context.Log.Verbose("Attempting to add the claimed deal to the data store.");
            result = ClaimedDealOperations.AddClaimedDeal();
            Context.Log.Verbose("ResultCode after adding the claimed deal to the data store: {0}", result);

            return result;
        }
    }
}