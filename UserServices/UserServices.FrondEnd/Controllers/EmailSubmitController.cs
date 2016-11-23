//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailSubmitController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UserServices.FrondEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Lomo.Logging;

    using LoMo.UserServices.DealsMailing;

    using Users.Dal;
    using Users.Dal.DataModel;

    using UserServices.FrondEnd.Models;

    /// <summary>
    ///     The email submit controller.
    /// </summary>
    public class EmailSubmitController : Controller
    {
        #region Constants

        /// <summary>
        /// The email type.
        /// </summary>
        private const string EmailType = "Email";

        /// <summary>
        /// The test type.
        /// </summary>
        private const string TestType = "Email Test";

        #endregion

        #region Fields

        /// <summary>
        /// The email jobs queue.
        /// </summary>
        private readonly IJobsQueue<EmailCargo> emailJobsQueue;

        /// <summary>
        /// The submit types.
        /// </summary>
        private readonly SelectList submitTypes = new SelectList(new List<string> { TestType, EmailType });

        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSubmitController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="emailJobsQueue">
        /// The email jobs queue.
        /// </param>
        public EmailSubmitController(IUsersDal usersDal, IJobsQueue<EmailCargo> emailJobsQueue)
        {
            this.usersDal = usersDal;
            this.emailJobsQueue = emailJobsQueue;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="submitType">
        /// The submit Type.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult Create(string userId, string submitType)
        {
            try
            {
                Guid trackingId = Guid.NewGuid();
                Guid userGuid;
                Guid.TryParse(userId, out userGuid);
                User user = this.GetUsersDal().GetUserByUserId(userGuid);
                EmailSubscription emailSubscription = this.GetUsersDal().GetEmailSubscriptionsByUserId(userGuid, true, SubscriptionType.WeeklyDeals.ToString()).FirstOrDefault();
                var unsubscribeUrl = this.GetUsersDal().GetUnsubscribeUrlInfo(user.Id).UnsubscribeUrl;
                var emailJob = new DealsEmailCargo { Id = trackingId, UserId = user.Id, EmailAddress = user.Email, LocationId  = emailSubscription.LocationId, UnsubscribeUrl = unsubscribeUrl, Hints = new EmailJobHints(), Categories = user.Info.Preferences.Categories};
                if (user.Info != null && user.Info.Preferences != null)
                {
                    emailJob.Categories = user.Info.Preferences.Categories;
                }

                bool isTest = submitType == TestType;
                emailJob.Hints.IsTest = isTest;

                this.GetEmailJobsQueue().Enqueue(emailJob);

                return this.RedirectToAction("Index", "EmailSubmit", new { trackingId, userGuid, isTest });
            }
            catch (Exception e)
            {
                Log.Warn("Submit Email Job Return Error. Details: {0}", e);
                return this.View("Error");
            }
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="trackingId">
        /// The tracking id.
        /// </param>
        /// <param name="userGuid"> The user Guid. </param>
        /// <param name="isTest"> The is Test. </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index(Guid? trackingId = null, Guid? userGuid = null, bool? isTest = null)
        {
            this.ViewBag.submitType = this.submitTypes;
            try
            {
                EmailSubmitModel model = null;
                if (trackingId.HasValue && userGuid.HasValue && isTest.HasValue)
                {
                    if (isTest.Value)
                    {
                        model = new EmailSubmitModel
                                    {
                                        TrackingId = trackingId.Value, 
                                        EmailSubmitJobCompleted = false, 
                                        DownloadLink = new Uri(string.Format("{0}/{1}-download.eml", WebApiApplication.EmailsBasePath, trackingId.Value)), 
                                        ViewLink = new Uri(string.Format("{0}/{1}-view.eml", WebApiApplication.EmailsBasePath, trackingId.Value)), 
                                    };

                        var client = new HttpClient();
                        Task getTask = client.GetAsync(model.ViewLink).ContinueWith(
                            task =>
                                {
                                    if (task.Exception != null)
                                    {
                                        throw task.Exception;
                                    }

                                    if (task.Result.IsSuccessStatusCode)
                                    {
                                        model.EmailSubmitJobCompleted = true;
                                    }
                                }, 
                            TaskContinuationOptions.ExecuteSynchronously);
                        getTask.Wait();
                        return this.View(model);
                    }

                    return this.View("UserMessage", userGuid);
                }

                return this.View(model);
            }
            catch (Exception e)
            {
                Log.Warn("Email Job View Return Error. Details: {0}", e);
                return this.View("Error");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get email jobs queue.
        /// </summary>
        /// <returns>
        ///     The IJobsQueue
        /// </returns>
        private IJobsQueue<EmailCargo> GetEmailJobsQueue()
        {
            return this.emailJobsQueue;
        }

        /// <summary>
        ///     The get users dal.
        /// </summary>
        /// <returns>
        ///     The <see cref="IUsersDal" />.
        /// </returns>
        private IUsersDal GetUsersDal()
        {
            return this.usersDal;
        }

        #endregion
    }
}