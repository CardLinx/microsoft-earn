//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user info controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Linq;

    using Lomo.Authorization;
    using Lomo.Logging;

    using LoMo.UserServices.DataContract;
    using Users.Dal;
    using Users.Dal.DataModel;

    using UserServices.FrondEnd.Email;

    using LocationType = Users.Dal.DataModel.LocationType;
    using User = LoMo.UserServices.DataContract.User;

    /// <summary>
    /// The user info controller.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    [HttpsOnly]
    public class UserInfoController : ApiController
    {
        #region Data Members

        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal _usersDal;

        /// <summary>
        /// The confirmation jobs queue.
        /// </summary>
        private readonly IPriorityEmailJobsQueue<PriorityEmailCargo> _confirmationJobsQueue;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="queue">Priority email queue</param>
        public UserInfoController(IUsersDal usersDal, IPriorityEmailJobsQueue<PriorityEmailCargo> queue = null)
        {
            this._usersDal = usersDal;
            this._confirmationJobsQueue = queue ?? new PriorityEmailJobsQueue<PriorityEmailCargo>();
        }

        #endregion

        #region API methods

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The user
        /// </returns>
        [ApiAuth]
        public User Get()
        {
            Guid userId = Security.GetUserId();
            try
            {
                Log.Verbose("Start Getting User Information. User Id {0}", userId);
                Users.Dal.DataModel.User user = this._usersDal.GetUserByUserId(userId);
                var res = this.ConvertUser(user);
                Log.Verbose("Get User Information request Completed Succefully. User Id {0}", userId);
                return res;
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't get user inforamtion user id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="userInformation">
        /// The user Information.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [ApiAuth]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] UserInformation userInformation)
        {
            Guid userId = Security.GetUserId();
            if (userInformation == null)
            {
                Log.Info("Update User Information Request with invalid input. User Id: {0}. Invalid Reason: {1}", userId, "Null");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (userInformation.Location != null)
            {
                string invalidReason;
                if (!LocationValidator.Validate(userInformation.Location, out invalidReason))
                {
                    Log.Info("Subscribe Request with invalid subscription. User Id: {0}. Invalid Reason: {1}", userId, invalidReason);
                    var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, invalidReason);
                    throw new HttpResponseException(errorResponse);
                }
            }

            try
            {
                Log.Verbose("Start updating user information. User Id {0}", userId);
                UserInfo userInfo = this.ConvertUserInformation(userInformation);
                this._usersDal.UpdateUserInfo(userId, userInfo);
                Log.Verbose("Update user information Completed Succefully. User Id {0}", userId);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't update user information. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// The update user phone number.
        /// </summary>
        /// <param name="phoneNumber">
        /// The new phone number
        /// </param>
        /// <param name="deletePhone">
        /// The delete Phone.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [ApiAuth]
        [HttpPost]
        public HttpResponseMessage UpdatePhoneNumber(string phoneNumber, bool deletePhone = false)
        {
            Guid userId = Security.GetUserId();
            try
            {
                // TODO - add confirmation code
                Log.Verbose("Start updating user's phone number. User Id {0}", userId);
                
                if (deletePhone)
                {
                    phoneNumber = string.Empty;
                }

                if (phoneNumber == null)
                {
                    Log.Verbose("Update phone number bad request. User Id {0}", userId);
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                
                this._usersDal.UpdateUserPhoneNumber(userId, phoneNumber);
                Log.Verbose("Update user's phone number Completed Succefully. User Id {0}", userId);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't update user's phone number. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Updates user's transaction notification preference
        /// </summary>
        /// <param name="notificationPreferences">Transaction notification preferences</param>
        /// <returns></returns>
        [ApiAuth]
        [HttpPost]
        public HttpResponseMessage UpdateNotificationPreference([FromUri]string[] notificationPreferences)
        {
            Guid userId = Security.GetUserId();
            try
            {
                string errorMessage = null;
                Log.Verbose("Start updating user's notification preference. User Id {0}", userId);
                Users.Dal.DataModel.TransactionNotificationPreference transactionNotificationPreference = Users.Dal.DataModel.TransactionNotificationPreference.None;

                foreach (var preference in notificationPreferences)
                {
                    Users.Dal.DataModel.TransactionNotificationPreference medium;
                    if (Enum.TryParse(preference, true, out medium))
                    {
                        transactionNotificationPreference |= medium;
                    }
                    else
                    {
                        errorMessage = string.Format("Notification Preference '{0}' is not valid.", preference);
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    throw new HttpResponseException(
                        this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
                }

                Users.Dal.DataModel.User user = this._usersDal.GetUserByUserId(userId);

                if (user.Info == null)
                {
                    user.Info = new UserInfo();
                }

                if (user.Info.Preferences == null)
                {
                    user.Info.Preferences = new Users.Dal.DataModel.UserPreferences();
                }

                user.Info.Preferences.TransactionNotificationMedium = transactionNotificationPreference;
                this._usersDal.UpdateUserInfo(userId, user.Info);

                Log.Verbose("Finished updating user's transaction notification preference succefully. User Id {0}", userId);

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in updating user's transaction notification preference. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// The update email.
        /// </summary>
        /// <param name="emailAddress">
        /// The email address.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [ApiAuth]
        [HttpPost]
        public HttpResponseMessage UpdateEmail(string emailAddress)
        {
            Log.Info("Start updating user's email address ");
            var identity = Thread.CurrentPrincipal.Identity as CustomIdentity;
            if (identity == null)
            {
                Log.Error("Update Email - User Identity is null while we are in authenticated context");
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            if (!EmailValidator.IsValidEmailFormat(emailAddress))
            {
                Log.Info("Update Email  Request with invalid email");
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format("email: {0} is invalid email address", emailAddress));
                throw new HttpResponseException(errorResponse);
            }

            var identityEmail = identity.EmailAddress;
            var userId = identity.UserId;
            
            var user = this._usersDal.GetUserByUserId(userId);
            if (user.Email == emailAddress)
            {
                Log.Verbose("No Operation of update email address. User already have the input email address. User Id={0}", userId);

                // Create confirmation code entity if confirmation is required or not required. 
                // The creation when confirmation isn't required help us to follow the sequence of update requests that the user done.
                this._usersDal.CreateConfirmationCode(emailAddress, EntityType.AuthenticatedEmailAddress, userId);
                return Request.CreateResponse(HttpStatusCode.Accepted, "SameEmail");
            }

            var emailUser = this._usersDal.GetUserByExternalId(emailAddress, UserExternalIdType.Email);
            if (user.IsSuppressed || (emailUser != null && emailUser.IsSuppressed))
            {
                Log.Warn("Can't update user email address. User is suppressed or target email is suppressed. User Id={0}; Target Email User Id={1}", user.Id, emailUser.Id);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (emailUser != null && !string.IsNullOrEmpty(emailUser.MsId))
            {
                Log.Warn("Can't update user email address for user. Email address is already in user by other authenticated user. User Id={0}; Target Email User Id={1}", user.Id, emailUser.Id);
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            // Create confirmation code entity if confirmation is required or not required. 
            // The creation when confirmation isn't required help us to follow the sequence of update requests that the user done.
            Tuple<string, int> confiramtionCodeResonse = this._usersDal.CreateConfirmationCode(emailAddress, EntityType.AuthenticatedEmailAddress, userId);
            string userIdHash = confiramtionCodeResonse.Item1;
            int confirmationCode = confiramtionCodeResonse.Item2;

            // Do not send a confirmation mail for email change if one of the following is true
            // 1. The email address is part of the user identity and already been validated
            // 2. Email change request is done from the Microsoft Earn site
            if ( (emailAddress == identityEmail) || (IsEarnUser()) )
            {
                try
                {
                    Log.Verbose("Start updating user email address. User Id={0}", userId);
                    this._usersDal.UpdateUserEmail(userId, emailAddress, true);
                    Log.Verbose("Completed updating user email address. User Id={0}", userId);
                    return Request.CreateResponse(HttpStatusCode.Accepted, "EmailChanged");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Couldn't update user email. User Id={0} ", userId);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            ConfirmationEmailCargo confirmationEmailCargo = new ConfirmationEmailCargo
                {
                    Id = Guid.NewGuid(),
                    EntityType = EntityType.AuthenticatedEmailAddress,
                    EmailAddress = emailAddress,
                    UserIdHash = userIdHash,
                    ConfirmationCode = confirmationCode
                };
            

            //queue the email confirmation job
            this._confirmationJobsQueue.Enqueue(confirmationEmailCargo);

            return Request.CreateResponse(HttpStatusCode.Accepted, "EmailConfirmationRequired");
        }
       
        /// <summary>
        /// The get update email status.
        /// </summary>
        /// <returns>
        /// the get update email stat
        /// </returns>
        /// <exception cref="HttpResponseException"> internal server error 
        /// </exception>
        [ApiAuth]
        [HttpGet]
        public GetEmailConfirmationStatusResponse EmailConfirmationStatus()
        {
            Guid userId = Security.GetUserId();
            try
            {
                Log.Verbose("Start get update email status. User Id {0}", userId);
                string userIdHash = SecureHashGenerator.Generate(userId.ToString());
                var user = this._usersDal.GetUserByUserId(userId);
                var confirmationEntity = this._usersDal.GetConfirmationEntity(userIdHash, EntityType.AuthenticatedEmailAddress);
                GetEmailConfirmationStatusResponse getEmailConfirmationStatusResponse;
                if ((!user.IsEmailConfirmed &&  !string.IsNullOrEmpty(user.Email)) || 
                    (confirmationEntity != null  && user.Email != confirmationEntity.Name && confirmationEntity.CreatedDate > (DateTime.UtcNow - TimeSpan.FromDays(3))))
                {
                    // Waiting for email confirmation
                    getEmailConfirmationStatusResponse = new GetEmailConfirmationStatusResponse
                                                             {
                                                                 EmailToConfirm = confirmationEntity != null ? confirmationEntity.Name : user.Email, 
                                                                 WaitingForConfirmation = true
                                                             };
                }
                else
                {
                    getEmailConfirmationStatusResponse = new GetEmailConfirmationStatusResponse { WaitingForConfirmation = false };
                }

                Log.Verbose(
                    "Get update email status completed. User Id {0}; Waiting for confirmation = {1};",
                    userId, 
                    getEmailConfirmationStatusResponse.WaitingForConfirmation);
                return getEmailConfirmationStatusResponse;
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't get update email status. User Id: {0}", userId);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The convert user information.
        /// </summary>
        /// <param name="userInformation">
        /// The user information.
        /// </param>
        /// <returns>
        /// The <see cref="UserInfo"/>.
        /// </returns>
        private UserInfo ConvertUserInformation(UserInformation userInformation)
        {
            UserInfo userInfo = new UserInfo();
            if (userInformation.Preferences != null && userInformation.Preferences.Categories != null)
            {
                userInfo.Preferences = new Users.Dal.DataModel.UserPreferences { Categories = new List<Guid>(userInformation.Preferences.Categories) };
            }

            if (userInformation.Location != null)
            {
                userInfo.Location = 
                    new Users.Dal.DataModel.Location(userInformation.Location.CountryCode, userInformation.Location.AdminDistrict, userInformation.Location.LocationName, (LocationType)userInformation.Location.LocationType);
            }
            
            return userInfo;
        }

        /// <summary>
        /// The convert user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        private User ConvertUser(Users.Dal.DataModel.User user)
        {
            User dataContractUser = new User { Email = user.Email, PhoneNumber = user.PhoneNumber };
            UserInformation userInformation = new UserInformation();
            dataContractUser.Information = userInformation;
            if (user.Info != null)
            {
                if (user.Info.Preferences != null)
                {
                    userInformation.Preferences = new LoMo.UserServices.DataContract.UserPreferences
                    {
                        TransactionNotificationMedium =  (LoMo.UserServices.DataContract.TransactionNotificationPreference) user.Info.Preferences.TransactionNotificationMedium
                    };
                    if (user.Info.Preferences.Categories != null)
                    {
                        userInformation.Preferences.Categories = new List<Guid>(user.Info.Preferences.Categories);
                    }
                    
                }

                if (user.Info.Location != null)
                {
                    var loc = user.Info.Location;
                    userInformation.Location = new LoMo.UserServices.DataContract.Location
                                                   {
                                                       AdminDistrict = loc.AdminDistrict,
                                                       CountryCode = loc.CountryCode,
                                                       LocationName = loc.Value,
                                                       LocationType = (LoMo.UserServices.DataContract.LocationType)loc.Type
                                                   };
                }
            }
            
            return dataContractUser;
        }

        /// <summary>
        /// Checks if the current request is made from user logged in via the Microsoft Earn site
        /// </summary>
        /// <returns>If the request is made from Microsoft Earn site or not</returns>
        private bool IsEarnUser()
        {
            const string flightIdHeaderName = "X-Flight-ID";
            const string flightIdHeaderValueForEarn = "Earn";

            bool isEarnUser = false;
            if (this.Request != null && this.Request.Headers.Contains(flightIdHeaderName))
            {
                IEnumerable<string> headerValues = this.Request.Headers.GetValues(flightIdHeaderName);
                isEarnUser = headerValues.Any(headerValue => string.Equals(headerValue, flightIdHeaderValueForEarn, StringComparison.OrdinalIgnoreCase));
            }

            return isEarnUser;
        }


        #endregion
    
    }
}