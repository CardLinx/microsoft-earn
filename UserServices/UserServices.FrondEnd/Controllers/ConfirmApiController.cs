//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The connect controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Lomo.Authorization;
    using Lomo.Logging;

    using LoMo.UserServices.DataContract;

    using Users.Dal;
    using Users.Dal.DataModel;

    using User = Users.Dal.DataModel.User;

    /// <summary>
    /// The connect controller.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class ConfirmApiController : ApiController
    {
        #region Consts & Static

        /// <summary>
        /// The max send requests per user in window.
        /// </summary>
        private const int MaxSendRequestsPerUserInWindow = 5;

        /// <summary>
        /// The send requests validation window.
        /// </summary>
        private static readonly TimeSpan SendRequestsValidationWindow = TimeSpan.FromDays(1);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The user dal.
        /// </summary>
        private readonly IUsersDal userDal;

        /// <summary>
        /// The confirmation jobs queue.
        /// </summary>
        private readonly IPriorityEmailJobsQueue<PriorityEmailCargo> confirmationJobsQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmApiController"/> class.
        /// </summary>
        /// <param name="userDal">
        /// The user dal.
        /// </param>
        /// <param name="confirmationJobsQueue">the confirmation jobs confirmationJobsQueue</param>
        public ConfirmApiController(IUsersDal userDal, IPriorityEmailJobsQueue<PriorityEmailCargo> confirmationJobsQueue)
        {
            this.userDal = userDal;
            this.confirmationJobsQueue = confirmationJobsQueue;
        }

        /// <summary>
        /// The link account result code.
        /// </summary>
        public enum LinkAccountResultCode
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0,

            /// <summary>
            /// The unknown error.
            /// </summary>
            UnknownError = 1,

            /// <summary>
            /// The invalid.
            /// </summary>
            Invalid = 2,

            /// <summary>
            /// The linked.
            /// </summary>
            Linked = 3,

            /// <summary>
            /// The external identity in use.
            /// </summary>
            ExternalIdentityInUse = 4,

            /// <summary>
            /// The already linked.
            /// </summary>
            AlreadyLinked = 5
        }

        /// <summary>
        /// The send confirmation result code.
        /// </summary>
        public enum SendConfirmationResultCode
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0,

            /// <summary>
            /// The unknown error.
            /// </summary>
            UnknownError = 1,

            /// <summary>
            /// The email sent.
            /// </summary>
            EmailSent = 2,

            /// <summary>
            /// The invalid.
            /// </summary>
            Invalid = 3,

            /// <summary>
            /// The already confirmed.
            /// </summary>
            AlreadyConfirmed = 4,

            /// <summary>
            /// The already linked.
            /// </summary>
            AlreadyLinked = 5
        }

        /// <summary>
        /// The connect method.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpsOnly]
        [HttpPost]
        [ApiAuth(new string[] { }, new[] { "UseExternalIdentity" })]
        public HttpResponseMessage Link(LinkAccountRequest request)
        {
            if (request == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string uh = request.UserIdHash;
            int code = request.Code;
            Guid activityId = Guid.NewGuid();
            var linkAccountResponse = new ConfirmApiResponse { ActivityId = activityId };
            Log.Info(activityId, "Start Processing link account request. uh={0}", uh);
            var identity = Thread.CurrentPrincipal.Identity as CustomIdentity;
            if (identity == null)
            {
                Log.Error(activityId, "call passed authentication however identity context is empty. uh: {0}", uh);
                linkAccountResponse.Code = LinkAccountResultCode.UnknownError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, linkAccountResponse);
            }

            try
            {
                var result = this.userDal.ConfirmEntity(uh, EntityType.AccountLink, code);

                // couldn't confirm code
                if (result == null || result.Status != ConfirmStatus.CodeConfirmed)
                {
                    Log.Error(activityId, "Couldn't link account for user: {0}. Response from confirm code call: {1}", uh, result != null ? (ConfirmStatus?)result.Status : null);
                    linkAccountResponse.Code = LinkAccountResultCode.Invalid.ToString();
                    return Request.CreateResponse(HttpStatusCode.BadRequest, linkAccountResponse);
                }

                // code confirmed
                Guid userId = result.UserId.Value;
                User user = this.userDal.GetUserByUserId(userId);
                if (user == null)
                {
                    Log.Error(activityId, "Link account error. User is confirmed however the user couldn't be found in DB. user id: {0}", userId);
                    linkAccountResponse.Code = LinkAccountResultCode.UnknownError.ToString();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, linkAccountResponse);
                }

                User authenticateUser = this.userDal.GetUserByExternalId(identity.ExternalUserId, UserExternalIdType.MsId);

                // there is a user in the db with the given external id
                if (authenticateUser != null)
                {
                    // the users are actualy the same user
                    if (authenticateUser.Id == userId)
                    {
                        Log.Verbose(activityId, "Link account succeeded. User already linked to this external id. user id: {0}", userId);
                        linkAccountResponse.Code = LinkAccountResultCode.Linked.ToString();
                        return Request.CreateResponse(HttpStatusCode.Accepted, linkAccountResponse);
                    }

                    // this external identity already in use. 
                    Log.Warn(activityId, "Can't link account to external identity. this identity is in use by other user. user id:{0}, other user id:{1}", userId, authenticateUser.Id);
                    linkAccountResponse.Code = LinkAccountResultCode.ExternalIdentityInUse.ToString();
                    return Request.CreateResponse(HttpStatusCode.Forbidden, linkAccountResponse);
                }

                if (user.MsId != null)
                {
                    // user already linked to different msid
                    Log.Warn(activityId, "User linked to other external id. user id: {0}", userId);
                    linkAccountResponse.Code = LinkAccountResultCode.AlreadyLinked.ToString();
                    return Request.CreateResponse(HttpStatusCode.Conflict, linkAccountResponse);
                }

                // Link exterenal id to the account
                this.userDal.CreateOrGetUserByMsId(identity.ExternalUserId, null, null, userId);
                Log.Verbose(activityId, "Account linking succeeded. user id: {0}", userId);
                linkAccountResponse.Code = LinkAccountResultCode.Linked.ToString();
                return Request.CreateResponse(HttpStatusCode.Accepted, linkAccountResponse);
            }
            catch (Exception e)
            {
                Log.Error(activityId, e, "Unexpected error in link accounts call. user id hash: {0}", uh);
                linkAccountResponse.Code = LinkAccountResultCode.UnknownError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, linkAccountResponse);
            }
        }

        /// <summary> The send. </summary>
        /// <param name="request"> The request. </param>
        /// <returns> The <see cref="HttpResponseMessage"/>. </returns>
        /// <exception cref="HttpResponseException"> error output
        /// </exception>
        [HttpsOnly]
        [HttpPost]
        public HttpResponseMessage Send(SendConfirmationEmailRequest request)
        {
            Guid activityId = Guid.NewGuid();
            var response = new ConfirmApiResponse { ActivityId = activityId };
            this.ValidateSendInput(request, activityId, response);
            EntityType entityType;
            if (!this.TryConvertEntityType(request.ConfirmationType, out entityType))
            {
                Log.Verbose(activityId, "Bad Request: unknown confirmation type: {0}", request.ConfirmationType);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }

            try
            {
                Log.Info(activityId, "Start Processing link account request. uh={0}; confirmation type={1}", request.UserIdHash, entityType);
                var confirmationEntity = this.userDal.GetConfirmationEntity(request.UserIdHash, entityType);
                User user = null;
                if (confirmationEntity != null)
                {
                    user = this.userDal.GetUserByUserId(confirmationEntity.UserId);
                }

                if (confirmationEntity == null || user == null)
                {
                    response.Code = SendConfirmationResultCode.Invalid.ToString();
                    Log.Info(activityId, "confirmation entity or user not found to the given user hash and entity type. uh={0}; confirmation type={1}", request.UserIdHash, entityType);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
                }

                this.ValidateResendJobsInTimeWindow(user.Id, entityType, activityId, response);
                ConfirmationEmailCargo job = null;
                if (entityType == EntityType.AccountLink)
                {
                    job = this.CreateSendLinkAccountEmailJob(user, entityType, activityId, response);
                }
                else if (entityType == EntityType.UnAuthenticatedEmailAddress)
                {
                    job = this.CreateSendConfirmEmailAddressEmailJob(user, confirmationEntity, entityType, activityId, response);
                }

                this.userDal.LogUserConfirmEmailResend(user.Id, entityType);
                this.confirmationJobsQueue.Enqueue(job);
                response.Code = SendConfirmationResultCode.EmailSent.ToString();
                return Request.CreateResponse(HttpStatusCode.Accepted, response);
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception e)
            {
                Log.Error(activityId, e, "Unexpected error in send confirmation email call. user id hash: {0}; entity type: {1}", request.UserIdHash, entityType);
                response.Code = SendConfirmationResultCode.UnknownError.ToString();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary> The validate send input. </summary>
        /// <param name="request"> The request. </param>
        /// <param name="activityId"> The activity id. </param>
        /// <param name="response"> The response. </param>
        /// <exception cref="HttpResponseException"> Validation failed
        /// </exception>
        private void ValidateSendInput(SendConfirmationEmailRequest request, Guid activityId, ConfirmApiResponse response)
        {
            if (request == null)
            {
                Log.Verbose(activityId, "Bad Request: input is null");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }

            if (string.IsNullOrEmpty(request.UserIdHash))
            {
                Log.Verbose(activityId, "Bad Request: user id is null or empty");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }
        }

        /// <summary> The validate resend jobs in time window. </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="activityId"> The activity id. </param>
        /// <param name="response"> The response. </param>
        /// <exception cref="HttpResponseException"> validation failed
        /// </exception>
        private void ValidateResendJobsInTimeWindow(Guid userId, EntityType entityType, Guid activityId, ConfirmApiResponse response)
        {
            int sendRequestInWindow = this.userDal.GetUserConfirmEmailResendCount(userId, entityType, DateTime.UtcNow.Add(-SendRequestsValidationWindow));
            if (sendRequestInWindow >= MaxSendRequestsPerUserInWindow)
            {
                response.Code = SendConfirmationResultCode.Invalid.ToString();
                Log.Error(activityId, "number of resend confirmation email request passed the allowed number of requests. user id={0}; num of requests={1}", sendRequestInWindow, userId);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }
        }

        /// <summary> The create send confirm email address email job. </summary>
        /// <param name="user"> The user. </param>
        /// <param name="confirmEntity"> The confirm entity. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="activityId"> The activity id. </param>
        /// <param name="response"> The response. </param>
        /// <returns> The <see cref="ConfirmationEmailCargo"/>. the created job to queue</returns>
        /// <exception cref="HttpResponseException">the email address is already confirmed </exception>
        private ConfirmationEmailCargo CreateSendConfirmEmailAddressEmailJob(User user, ConfirmEntity confirmEntity, EntityType entityType, Guid activityId, ConfirmApiResponse response)
        {
            string emailToConfirm = confirmEntity.Name;
            if (user.Email == emailToConfirm && user.IsEmailConfirmed)
            {
                response.Code = SendConfirmationResultCode.AlreadyConfirmed.ToString();
                Log.Info(activityId, "Email Address already confirmed, won't resend confirm email. user id={0}; ", user.Id);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }

            Tuple<string, int> confirmationCode = this.userDal.CreateConfirmationCode(emailToConfirm, entityType, user.Id);
            return new ConfirmationEmailCargo
                       {
                           Id = Guid.NewGuid(),
                           EntityType = EntityType.UnAuthenticatedEmailAddress,
                           EmailAddress = user.Email,
                           UserIdHash = confirmationCode.Item1,
                           ConfirmationCode = confirmationCode.Item2
                       };
        }

        /// <summary> The create send link account email job. </summary>
        /// <param name="user"> The user. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="activityId"> The activity id. </param>
        /// <param name="response"> The response. </param>
        /// <returns> The <see cref="ConfirmationEmailCargo"/>. </returns>
        /// <exception cref="HttpResponseException">the account is already linked </exception>
        private ConfirmationEmailCargo CreateSendLinkAccountEmailJob(User user, EntityType entityType, Guid activityId, ConfirmApiResponse response)
        {
            if (!string.IsNullOrEmpty(user.MsId))
            {
                response.Code = SendConfirmationResultCode.AlreadyLinked.ToString();
                Log.Info(activityId, "Account is already linked to ms-id/ fb-id, won't resend confirm email. user id={0}; ", user.Id);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, response));
            }

            Tuple<string, int> confirmationCode = this.userDal.CreateConfirmationCode(user.Id.ToString(), entityType, user.Id);
            return new ConfirmationEmailCargo
                       {
                           Id = Guid.NewGuid(),
                           EntityType = EntityType.AccountLink,
                           EmailAddress = user.Email,
                           UserIdHash = confirmationCode.Item1,
                           ConfirmationCode = confirmationCode.Item2,
                       };
        }

        #endregion

        /// <summary>
        /// The try convert entity type.
        /// </summary>
        /// <param name="confirmationType">
        /// The confirmation type.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryConvertEntityType(string confirmationType, out EntityType entityType)
        {
            entityType = EntityType.None;
            if (confirmationType == null)
            {
                return false;
            }

            if (confirmationType.ToLowerInvariant() == "emailaddress")
            {
                entityType = EntityType.UnAuthenticatedEmailAddress;
                return true;
            }

            if (confirmationType.ToLowerInvariant() == "accountlink")
            {
                entityType = EntityType.AccountLink;
                return true;
            }

            return false;
        }

        

    }
}