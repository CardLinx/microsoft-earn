//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The Merchant controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using LoMo.UserServices.DataContract;
    using Lomo.Authorization;
    using Users.Dal;
    using Users.Dal.DataModel;
    using Lomo.Logging;

    /// <summary>
    /// The Merchant Controller
    /// </summary>
    public class MerchantController : ApiController
    {
        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchantController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        public MerchantController(IUsersDal usersDal)
        {
            this.usersDal = usersDal;
        }

        /// <summary>
        /// Returns the merchant information
        /// </summary>
        /// <returns>
        /// Merchant Information
        /// </returns>
        [HttpsOnly]
        [ApiAuth]
        public MerchantInformation Get()
        {
            Guid userId = Security.GetUserId();
            Log.Verbose("Getting user information for merchant user Id: {0}", userId);

            try
            {
                MerchantInformation merchantInformation = null;
                IEnumerable<MerchantSubscriptionInfo> merchantSubscriptions = this.usersDal.GetMerchantSubscriptionsByUserId(userId);
                var subscription = merchantSubscriptions.FirstOrDefault(merchantSubscription => merchantSubscription.SubscriptionType == SubscriptionType.TransactionReport);
                if (subscription != null)
                {
                    merchantInformation = new MerchantInformation()
                        {
                            IsActive = subscription.IsActive,
                            Preferences =  subscription.IsActive ? new LoMo.UserServices.DataContract.MerchantPreferences
                                {
                                    EmailReportInterval = subscription.EmailReportInterval.ToString()
                                } : null,
                            SubscriptionType = subscription.SubscriptionType.ToString()
                        };
                }

                Log.Verbose("Get user preferences request completed succefully for merchant user Id: {0}", userId);

                return merchantInformation;
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't get preferences for merchant user Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }


        /// <summary>
        /// Updates the merchant information
        /// </summary>
        /// <param name="preferences">Email report settings</param>
        /// <returns>Http Status code</returns>
        [HttpsOnly]
        [ApiAuth]
        [HttpPost]
        public HttpResponseMessage Update(LoMo.UserServices.DataContract.MerchantPreferences preferences)
        {
            Guid userId = Security.GetUserId();
            Log.Verbose("Start updating preferences for merchant user id {0}", userId);

            HttpResponseMessage errorResponse;
            if (preferences == null)
            {
                Log.Info("Invalid request. Update preferences failed for merchant user Id: {0}..merchantpreferences is null", userId);
                errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "request is empty");
                throw new HttpResponseException(errorResponse);
            }

            ScheduleType scheduleType;
            if (Enum.TryParse(preferences.EmailReportInterval, true, out scheduleType))
            {
                usersDal.UpdateMerchantSubscription(userId, SubscriptionType.TransactionReport, scheduleType, null);

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }

            Log.Info("Invalid request. Update preferences failed for merchant user Id: {0}..scheduletype is invalid", userId);
            errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid schedule type");
            throw new HttpResponseException(errorResponse);
        }

        /// <summary>
        /// Unsubscribes the user from receiving future email
        /// </summary>
        /// <returns>Http Status code</returns>
        [HttpsOnly]
        [ApiAuth]
        [HttpPost]
        public HttpResponseMessage Unsubscribe()
        {
            Guid? userId = null;
            try
            {
                userId = Security.GetUserId();
                Log.Verbose("Received unsubscribe request for merchant user Id: {0}", userId);
                this.usersDal.DeleteMerchantSubscriptions(userId.Value, SubscriptionType.TransactionReport);
                Log.Verbose("Unsubscribe request completed succcefully for merchant user Id: {0}", userId);

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in executing unsubscribe request for merchant user Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}