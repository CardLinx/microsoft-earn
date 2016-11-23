//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Utilities;
    using Users.Dal;

    /// <summary>
    /// Ping API controller for LoMo Commerce Web service.
    /// </summary>
    public class PingController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the PingController class.
        /// </summary>
        public PingController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PingController class.
        /// </summary>
        /// <param name="request">
        /// The request to process within this instance.
        /// </param>
        internal PingController(HttpRequestMessage request)
        {
            Request = request;
        }

        /// <summary>
        /// Pings the Commerce, and Users databases.
        /// </summary>
        /// <returns>
        /// The success or failure result of each ping operation.
        /// </returns>
        public HttpResponseMessage Get()
        {
            // Ping the databases.
            bool commerceSuccess = PingCommerce();
            bool usersSuccess = PingUsers();

            // Build and return the response.
            return BuildResponse(commerceSuccess, usersSuccess);
        }

        /// <summary>
        /// Builds the Ping response based on the results of the ping operations.
        /// </summary>
        /// <param name="commerceSuccess">
        /// Specifies whether or not the Commerce ping attempt was successful.
        /// </param>
        /// <param name="usersSuccess">
        /// Specifies whether or not the Users ping attempt was successful.
        /// </param>
        /// <returns>
        /// The response containing the results of the ping operations and the appropriate HTTP status code.
        /// </returns>
        internal HttpResponseMessage BuildResponse(bool commerceSuccess,
                                                   bool usersSuccess)
        {
            HttpResponseMessage result = null;

            string responseText = String.Format("Operations success report: Commerce {0}; Users {1}", commerceSuccess, usersSuccess);
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            if (commerceSuccess == false || usersSuccess == false)
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
            }

            // Build the response.
            if (Request != null)
            {
                result = Request.CreateResponse(httpStatusCode, responseText);
            }

            return result;
        }

        /// <summary>
        /// Pings the Commerce database.
        /// </summary>
        /// <returns>
        /// * True if the Commerce database could be pinged.
        /// * Else returns false.
        /// </returns>
        private static bool PingCommerce()
        {
            // Ping Commerce by attempting to retrieve cards for a user known not to exist.
            GetCardsResponse response = new GetCardsResponse();
            CommerceContext context = CommerceContext.BuildSynchronousRestContext(String.Empty, null, response, new Stopwatch());
            context[Key.GlobalUserId] = Guid.Empty;
            context[Key.RewardProgramType] = RewardPrograms.All;
            context.Log.OnlyLogIfVerbosityIsAll = true;
            V2GetCardsExecutor getCardsExecutor = new V2GetCardsExecutor(context);
            getCardsExecutor.Execute();
            return response.ResultSummary.GetResultCode() == ResultCode.UnregisteredUser;
        }

        /// <summary>
        /// Pings the Users database.
        /// </summary>
        /// <returns>
        /// * True if the Users database could be pinged.
        /// * Else returns false.
        /// </returns>
        private static bool PingUsers()
        {
            // Ping Users by attempting to retrieve a User known not to exist.
            IUsersDal usersDal = PartnerFactory.UsersDal(CommerceServiceConfig.Instance);
            return usersDal.GetUserByUserId(Guid.Empty) == null;
        }
    }
}