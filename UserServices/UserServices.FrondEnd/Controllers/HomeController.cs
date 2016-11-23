//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace UserServices.FrondEnd.Controllers
{
    using System.Web.Mvc;
    using Lomo.Logging;
    using UserServices.FrondEnd.Email;
    using UserServices.FrondEnd.Models;
    using Users.Dal;
    using Users.Dal.DataModel;

    public class HomeController : Controller
    {
        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal _usersDal;

         #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        public HomeController(IUsersDal usersDal)
        {
            this._usersDal = usersDal;
        }

        #endregion

        [HttpPost]
        public ActionResult GetUserByEmail(string email)
        {
            string message;
            if (!EmailValidator.IsValidEmailFormat(email))
            {
                Log.Warn("Lookup Request with invalid email. Email is {0}", email);
                message = string.Format("{0} is an invalid email address", email);
            }
            else
            {
                User user = this._usersDal.GetUserByExternalId(email, UserExternalIdType.Email);
                message = user != null && !user.IsSuppressed ? "Found" : "Not Found";
            }


            return this.RedirectToAction("Index", "Home", new { message });
        }

        public ActionResult Index(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                EmailSubmitModel emailSubmitModel = new EmailSubmitModel { Message = message };

                return View(emailSubmitModel);
            }

            return View();
        }
    }
}