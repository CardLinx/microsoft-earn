//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The subscriptions controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Lomo.Authorization;
    using Lomo.Logging;

    using LoMo.UserServices.DataContract;

    using Users.Dal;
    using Users.Dal.DataModel;

    using UserServices.FrondEnd.Email;

    using EmailSubscription = Users.Dal.DataModel.EmailSubscription;
    using Location = Users.Dal.DataModel.Location;
    using LocationType = LoMo.UserServices.DataContract.LocationType;

    /// <summary>
    /// The subscriptions controller.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class SubscriptionsController : ApiController
    {
        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        public SubscriptionsController(IUsersDal usersDal)
        {
            this.usersDal = usersDal;
        }

        /// <summary>
        /// Get the user's subscriptions
        /// </summary>
        /// <returns>
        /// The active user's subscriptions
        /// </returns>
        [ApiAuth]
        [HttpGet]
        [HttpsOnly]
        public IEnumerable<LoMo.UserServices.DataContract.Location> Get()
        {
            Guid userId = Security.GetUserId();
            try
            {
                Log.Verbose("Start processing get subscriptions request. User Id {0}", userId);

                IEnumerable<EmailSubscription> dataModelSubscriptions = this.usersDal.GetEmailSubscriptionsByUserId(userId, true, SubscriptionType.WeeklyDeals.ToString());
                var res = dataModelSubscriptions.Select(this.Convert).ToList();
                Log.Verbose("Get subscription request Completed Succefully. User Id {0}", userId);
                return res;
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't get subscriptions for user with id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// UpdateSubscriptions user by email
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage SubscribeByEmail(SubscribeByEmailRequest request)
        {
            if (request == null)
            {
                Log.Info("UpdateSubscriptions By email with invalid requestr. request is null");
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "request is empty");
                throw new HttpResponseException(errorResponse);
            }

            var email = request.Email;
            var subscription = request.SubscriptionInfo;
            Log.Verbose("Start processing subscribe request");
            if (!EmailValidator.IsValidEmailFormat(email))
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("email: {0} is invalid email address", email));
                throw new HttpResponseException(errorResponse);
            }

            Users.Dal.DataModel.User user;
            try
            {
                user = this.usersDal.CreateOrGetUserByEmail(email, false, this.GetSourceInfo());
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't subscribe user by email.");
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            if (user.Info != null && user.MsId != null && user.Info.Location != null)
            {
                Log.Warn("Subscribe by email failure. User already exists in the system, need to go in the authenticated state in order to change settings. UserId is: {0}", user.Id);
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            this.UpdateSubscriptions(user.Id, new List<LoMo.UserServices.DataContract.Location> { subscription });
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// The update subscriptions. Overwrite the list of subscriptions with the input list
        /// </summary>
        /// <param name="emailSubscriptions">
        /// The email subscription.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [ApiAuth]
        [HttpPost]
        [HttpsOnly]
        public HttpResponseMessage Update(List<LoMo.UserServices.DataContract.Location> emailSubscriptions)
        {
            Guid userId = Security.GetUserId();
            if (emailSubscriptions == null)
            {
                Log.Info("Update Subscriptions with invalid requestr. request is null");
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "request is empty");
                throw new HttpResponseException(errorResponse);
            }

            Log.Verbose("Start processing update subscriptions request. User id: {0}", userId);
            this.UpdateSubscriptions(userId, emailSubscriptions);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// The un subscribe.
        /// </summary>
        /// <param name="emailSubscription">
        /// The email subscription.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [ApiAuth]
        [HttpPost]
        [HttpsOnly]
        public HttpResponseMessage UnSubscribe(LoMo.UserServices.DataContract.Location emailSubscription)
        {
            Guid userId = Security.GetUserId();
            Log.Verbose("Start unsubscribe request. User id: {0}", userId);
            string invalidReason;
            if (!this.ValidateApiUnsubscribe(emailSubscription, out invalidReason))
            {
                Log.Info("Unsubscribe request with invalid subscription. User Id: {0}. Invalid Reason: {1}", userId, invalidReason);
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, invalidReason);
                throw new HttpResponseException(errorResponse);
            }

            try
            {
                string locationId = this.GetDataModelLocationId(emailSubscription);
                Log.Verbose("unsubscribe request Completed Succefully. User Id {0}", userId);
                this.usersDal.DeleteEmailSubscriptions(userId, SubscriptionType.WeeklyDeals, locationId);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't unsubscribe. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// The unsubscribe all.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [ApiAuth]
        [HttpPost]
        [HttpsOnly]
        public HttpResponseMessage UnsubscribeAll()
        {
            Guid? userId = null;
            try
            {
                userId = Security.GetUserId();
                Log.Verbose("Start UnsubscribeAll. User Id {0}", userId);
                this.usersDal.DeleteEmailSubscriptions(userId.Value, SubscriptionType.WeeklyDeals);
                Log.Verbose("UnsubscribeAll completed succcefully. User Id {0}", userId);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't unsubscribe all. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #region Private Methods

        /// <summary>
        /// The subscribe. 
        /// </summary>
        /// <param name="userId">
        /// The user id. 
        /// </param>
        /// <param name="emailSubscriptions">
        /// The email Subscriptions.
        /// </param>
        /// <exception cref="HttpResponseException">
        /// error during subscribe operation 
        /// </exception>
        private void UpdateSubscriptions(Guid userId, List<LoMo.UserServices.DataContract.Location> emailSubscriptions)
        {
            foreach (LoMo.UserServices.DataContract.Location emailSubscription in emailSubscriptions)
            {
                string invalidReason;
                if (!LocationValidator.Validate(emailSubscription, out invalidReason))
                {
                    Log.Info("Update Subscriptions Request with invalid subscription. User Id: {0}. Invalid Reason: {1}", userId, invalidReason);
                    var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, invalidReason);
                    throw new HttpResponseException(errorResponse);
                }
            }

            IEnumerable<string> locationIds = emailSubscriptions.Select(this.GetDataModelLocationId);

            try
            {
                this.usersDal.UpdateEmailSubscriptions(userId, locationIds, SubscriptionType.WeeklyDeals);
                Log.Verbose("Update Subscriptions Completed Succefully. User Id {0}", userId);
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't update subscription for user. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// The validate subscription. </summary> <param name="emailSubscription">
        /// The email subscription. </param>
        /// <param name="reason"> in case of validation failure will include the reason</param>
        ///  <returns> is valid subscription argument value
        /// </returns>
        private bool ValidateApiUnsubscribe(LoMo.UserServices.DataContract.Location emailSubscription, out string reason)
        {
            if (emailSubscription == null)
            {
                reason = "Subscription information is emtpy";
                return false;
            }

            if (emailSubscription.LocationType == LocationType.None)
            {
                reason = string.Format("Location type not exist. The actual value is: {0}", emailSubscription.LocationType);
                return false;
            }

            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// The the location id in data model representation.
        /// </summary>
        /// <param name="emailSubscription"> The email subscription. </param>
        /// <returns> The <see cref="string"/>. </returns>
        private string GetDataModelLocationId(LoMo.UserServices.DataContract.Location emailSubscription)
        {
            Users.Dal.DataModel.LocationType type = (Users.Dal.DataModel.LocationType)Enum.Parse(typeof(Users.Dal.DataModel.LocationType), emailSubscription.LocationType.ToString());
            Location location = new Location(emailSubscription.CountryCode, emailSubscription.AdminDistrict, emailSubscription.LocationName, type);
            return location.ToString();
        }

        /// <summary>
        /// Convert subscription data model into data contract
        /// </summary>
        /// <param name="emailSubscription">
        /// The email subscription. </param>
        /// <returns>
        /// The <see cref="EmailSubscription"/> the data contract representation of the subscription  </returns>
        /// <exception cref="ApplicationException">the data model location id is in wrong format
        /// </exception>
        private LoMo.UserServices.DataContract.Location Convert(EmailSubscription emailSubscription)
        {
            var dalLocation  = Location.Parse(emailSubscription.LocationId);
            
            LocationType locationType;
            if (!Enum.TryParse(dalLocation.Type.ToString(), true, out locationType) || (locationType != LocationType.Postal && locationType != LocationType.National))
            {
                throw new ApplicationException(string.Format("Location Id wrong format. Only postal, national  location type are allowed in this phase. Location Id={0}", emailSubscription.LocationId));
            }

            return new LoMo.UserServices.DataContract.Location { CountryCode = dalLocation.CountryCode, AdminDistrict = dalLocation.AdminDistrict, LocationName = dalLocation.Value, LocationType = locationType };
        }

        /// <summary>
        /// The get referrer info.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetSourceInfo()
        {
            var sb = new StringBuilder();
            if (HttpContext.Current != null)
            {
                // The cookie "bo_referrer" has the publisher info that drove the user sign up to bing offers. 
                // Add this publisher info to the user's source column in the db for new user
                var cookie = HttpContext.Current.Request.Cookies["bor"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    sb.Append(cookie.Value);
                    Log.Info(string.Format("bor cookie is : {0}", cookie.Value));
                }
                else
                {
                    Log.Info("bor Cookie is null");
                }

                cookie = HttpContext.Current.Request.Cookies["bof"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    sb.Append("#");
                    sb.Append(cookie.Value);
                    Log.Info(string.Format("bof cookie is : {0}", cookie.Value));
                }
                else
                {
                    Log.Info("bof Cookie is null");
                }
            }
            else
            {
                Log.Info("HttpContext is null");
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }

        #endregion
    }
}