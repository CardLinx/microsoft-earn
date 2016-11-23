//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email controller.
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
    using System.Web.Http.Cors;

    using Lomo.Authorization;
    using Lomo.Logging;

    using LoMo.UserServices.DataContract;

    using Users.Dal;

    using UserServices.FrondEnd.Email;

    using DataModel = Users.Dal.DataModel;

    /// <summary>
    ///     The email controller.
    /// </summary>
    [HttpsOnly]
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class EmailController : ApiController
    {
        #region Fields

        /// <summary>
        /// The email client.
        /// </summary>
        private readonly IEmailClient emailClient;

        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="emailClient">
        /// The email client.
        /// </param>
        public EmailController(IUsersDal usersDal, IEmailClient emailClient)
        {
            this.usersDal = usersDal;
            this.emailClient = emailClient;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The ping.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        [ApiAuth]
        public HttpResponseMessage Ping()
        {
            Log.Verbose("Ping Request Started");
            Guid userId = Security.GetUserId();
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new StringContent(string.Format("User Id={0}", userId)) };
            Log.Verbose("Ping Request Completed");
            return response;
        }

        /// <summary>
        /// The ping anonymous.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage PingAnonymous()
        {
            Log.Verbose("PingAnonymous Request Started");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new StringContent(string.Format("User Id={0}", "Anonymous")) };
            Log.Verbose("PingAnonymous Request Started");
            return response;
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="correlationId"> the request correlation id</param>
        /// <param name="request"> The request. </param>
        /// <returns>
        /// The response message. </returns>
        [HttpPost]
        public HttpResponseMessage Send(Guid? correlationId, SendEmailRequest request)
        {
            // TODO - add application authentication
            string requestIdentifier = string.Format("Correletion Id={0}",correlationId);

            Log.Verbose("Start processing email send request. {0}", requestIdentifier);

            if (request == null || request.ToList == null || request.ToList.Count < 1 || request.Content == null || string.IsNullOrEmpty(request.Content.From))
            {
                Log.Verbose("Email Send Request, Bad Request. {0}", requestIdentifier);
                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                return httpResponse;
            }

            if (!request.ToList.TrueForAll(EmailValidator.IsValidEmailFormat))
            {
                Log.Verbose("Email Send Request, Bad Request. {0}", requestIdentifier);
                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                httpResponse.ReasonPhrase = "One or more email is in bad format";
                return httpResponse;
            }

            //Check the "To" list for any suppressed users
            var suppressedUsersList = FilterSuppressedUsers(request.ToList);

            if (suppressedUsersList != null && suppressedUsersList.Any())
            {
                Log.Error("Cannot send email...Suppressed users are in list");
                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                httpResponse.ReasonPhrase = string.Format("Cannot send email.The following are suppressed users : {0}",string.Join(",", suppressedUsersList));
                return httpResponse;
            }
            
            try
            {
                this.emailClient.Send(this.ConvertRequest(request.Content, request.ToList,request.IsTest), correlationId);
                HttpResponseMessage response = this.ControllerContext.Request.CreateResponse(HttpStatusCode.Accepted);
                Log.Verbose("Email Send Request Completed Succefully. {0}", requestIdentifier);
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Cannot send email. Request Correlation: {0}", correlationId);
                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.InternalServerError);
                return httpResponse;
            }
        }

        /// <summary>
        /// The send to users.
        /// </summary>
        /// <param name="correlationId"> the request correlation id</param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage SendToUsers(Guid? correlationId, SendEmailByUserIdRequest request)
        {
            // TODO - add application authentication
            string requestIdentifier = string.Format("Correletion Id={0}",correlationId);

            Log.Verbose("Start processing email send to users request. {0}", requestIdentifier);

            if (request == null || request.UserIds == null || request.UserIds.Count < 1 || request.Content == null || string.IsNullOrEmpty(request.Content.From))
            {
                Log.Verbose("Email Send Request, Bad Request. {0}", requestIdentifier);
                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest);
                return httpResponse;
            }

            try
            {
                var emails = new Dictionary<Guid, string>();
                foreach (Guid userId in request.UserIds)
                {
                    DataModel.User user = this.usersDal.GetUserByUserId(userId);
                    if (user != null && !string.IsNullOrEmpty(user.Email) && !user.IsSuppressed)
                    {
                        emails.Add(userId, user.Email);
                    }
                }

                if (emails.Count > 0)
                {
                    this.emailClient.Send(this.ConvertRequest(request.Content, emails.Values.ToList(),request.IsTest), correlationId);
                    IList<Guid> usersWithEmailAddress = emails.Keys.ToList();
                    HttpResponseMessage response = this.ControllerContext.Request.CreateResponse(HttpStatusCode.Accepted, usersWithEmailAddress);
                    Log.Verbose("Email Send Request Completed Succefully. {0}", requestIdentifier);
                    return response;
                }

                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);
                return httpResponse;
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't send emails by users. Request Corelletion={0}", correlationId);
                HttpResponseMessage httpResponse = this.ControllerContext.Request.CreateResponse(HttpStatusCode.InternalServerError);
                return httpResponse;
            }
        }
       
        #endregion

        #region Private Methods

        /// <summary>
        /// Checks and returns suppressed users from the list of email addresses
        /// </summary>
        /// <param name="toList">List of To Email addresses</param>
        /// <returns>List of suppressed users</returns>
        private List<string> FilterSuppressedUsers(IEnumerable<string> toList)
        {
            List<string> lstSuppressedUsers = null;

            foreach (var userId in toList)
            {
                var user = this.usersDal.GetUserByExternalId(userId, DataModel.UserExternalIdType.Email);
                if (user != null && user.IsSuppressed)
                {
                    if (lstSuppressedUsers == null)
                    {
                        lstSuppressedUsers = new List<string>();
                    }

                    lstSuppressedUsers.Add(userId);
                }
            }

            return lstSuppressedUsers;
        }

        /// <summary>
        /// The convert request.
        /// </summary>
        /// <param name="emailContent">
        /// The email content.
        /// </param>
        /// <param name="toList">
        /// The to list.
        /// </param>
        /// <param name="isTest">
        /// Indicates whether this email job should use sendgrid test account
        /// </param>
        /// <returns>
        /// The <see cref="EmailInformation"/>.
        /// </returns>
        private EmailInformation ConvertRequest(EmailContent emailContent, IList<string> toList, bool isTest)
        {
            return new EmailInformation
            {
                From = emailContent.From,
                FromDisplayName = emailContent.FromDisplay,
                ReplayTo = emailContent.ReplyTo,
                HtmlBody = emailContent.HtmlBody,
                Subject = emailContent.Subject,
                TextBody = emailContent.TextBody,
                Category = emailContent.Category,
                UniqueIdentifiers = emailContent.UniqueIdentifiers,
                To = toList,
                IsTest = isTest
            };
        }

        #endregion
    }
}