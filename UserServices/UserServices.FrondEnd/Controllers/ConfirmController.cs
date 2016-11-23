//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The confirm controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Http.Cors;

    using Lomo.Logging;
    using Users.Dal;
    using Users.Dal.DataModel;

    /// <summary>
    /// The confirm controller.
    /// </summary>
    [EnableCors(origins: "https://www.earnbymicrosoft.com, https://earn.microsoft.com, https://int.earnbymicrosoft.com", headers: "*", methods: "*")]
    public class ConfirmController : Controller
    {
        #region Constants

        /// <summary>
        /// Relative url for the page to redirect if the authenticated user's email is confirmed
        /// </summary>
        private const string AuthUserConfirmedPage = @"email\update?emailaddress={0}";

        /// <summary>
        /// Relative url for the page to redirect if the authenticated user's email is not confirmed due to an invalid or an expired code
        /// </summary>
        private const string AuthUserInvalidPage = @"email\error";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's email is confirmed, but the user's account is not linked with MS/FB id
        /// </summary>
        private const string UnauthEmailConfirmAccountNotLinkedPage = @"emailconfirm?state=link&uh={0}&c={1}";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's email is confirmed and the user's account is already linked with MS/FB id
        /// </summary>
        private const string UnauthEmailConfirmAccountLinkedPage = @"emailconfirm?state=success&a={0}";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's email is not confirmed due to an expired code
        /// </summary>
        private const string UnauthEmailConfirmExpiredCodePage = @"emailconfirm?state=resend&uh={0}";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's email is not confirmed due to an invalid code or other exception
        /// </summary>
        private const string UnauthEmailConfirmUserErrorPage = @"emailconfirm?state=error";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's account link code is confirmed, but the user's account is not linked with MS/FB id
        /// </summary>
        private const string UnauthAlinkConfirmAccountNotLinkedPage = @"linkaccount?state=link&uh={0}&c={1}";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's account link code is confirmed and the user's account is already linked with MS/FB id
        /// </summary>
        private const string UnauthAlinkConfirmAccountLinkedPage = @"linkaccount?state=success&a={0}";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's account link code is not confirmed due to an expired code
        /// </summary>
        private const string UnauthAlinkConfirmExpiredCodePage = @"linkaccount?state=resend&uh={0}";

        /// <summary>
        /// Relative url for the page to redirect if the unauthenticated user's account link code is not confirmed due to an invalid code or other exception
        /// </summary>
        private const string UnauthAlinkConfirmUserErrorPage = @"linkaccount?state=error";

        /// <summary>
        /// Facebook auth provider
        /// </summary>
        const string Facebook = "f";

        /// <summary>
        /// Microsoft auth provider
        /// </summary>
        const string Microsoft = "m";

        /// <summary>
        /// Facebook Id Pattern
        /// </summary>
        const string FbIdPattern = "fb-";

        #endregion

        #region Members

        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal _usersDal;

        /// <summary>
        /// The bing offers base uri.
        /// </summary>
        private readonly Uri _bingOffersBaseUri;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmController"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="entityConfirmationSettings">
        /// The entity confirmation settings.
        /// </param>
        /// <exception cref="ArgumentNullException"> entityConfirmationSettings is null
        /// </exception>
        public ConfirmController(IUsersDal usersDal, EntityConfirmationSettings entityConfirmationSettings)
        {
            if (entityConfirmationSettings == null)
            {
                throw new ArgumentNullException("entityConfirmationSettings");
            }

            this._bingOffersBaseUri = entityConfirmationSettings.BingOffersBaseUri;
            this._usersDal = usersDal;
        }

        /// <summary>
        /// Confirm Email.
        /// </summary>
        /// <param name="uh">
        /// The user id hash.
        /// </param>
        /// <param name="c">
        /// The confirmation code.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult Email(string uh, int c)
        {
            Log.Verbose("Starting Confirming email. User Id Hash = {0}", uh);
            Uri redirectUri = new Uri(_bingOffersBaseUri, AuthUserInvalidPage);
            if (string.IsNullOrEmpty(uh))
            {
                Log.Info("Confirm Email Request with invalid paramter. User Hash is null or empty", uh);
            }
            else
            {
                try
                {
                    ConfirmEntityResult result = _usersDal.ConfirmEntity(uh, EntityType.AuthenticatedEmailAddress, c);
                    switch (result.Status)
                    {
                        case ConfirmStatus.CodeConfirmed:
                            _usersDal.UpdateUserEmail(result.UserId.Value, result.EntityId, true);
                            Log.Verbose("Email confirmed and updated for user. User Id={0} ", result.UserId);
                            redirectUri = new Uri(_bingOffersBaseUri, string.Format(AuthUserConfirmedPage, result.EntityId));
                            break;
                        case ConfirmStatus.CodeWrong:
                            redirectUri = new Uri(_bingOffersBaseUri, AuthUserInvalidPage);
                            Log.Verbose("Can't confirm email. The user provided wrong code. User Id Hash = {0}", uh);
                            break;
                        case ConfirmStatus.Invalid:
                            redirectUri = new Uri(_bingOffersBaseUri, AuthUserInvalidPage);
                            Log.Verbose("Can't confirm email. The confirmation code is expired. User Id Hash = {0}", uh);
                            break;
                        case ConfirmStatus.CodeNotFound:
                            Log.Verbose("Can't confirm email. Cannot locate the confirmation request for this user in the system. User Id Hash = {0}", uh);
                            break;
                    }
                }
                catch (Exception exp)
                {
                    Log.Error(exp, "Can't Confirm email. User Id Hash = {0}", uh);
                    redirectUri = new Uri(_bingOffersBaseUri, AuthUserInvalidPage);
                }
            }

            return new RedirectResult(redirectUri.ToString());
        }

        /// <summary>
        /// Controller method for confirming unauthenticated user email address
        /// </summary>
        /// <param name="uh">Hash of user id</param>
        /// <param name="c">Confirmation code</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult CoEmail(string uh, int c)
        {
            Log.Verbose("Start confirming email for unauthenticated user. User Id Hash = {0}", uh);
            Uri redirectUri = new Uri(_bingOffersBaseUri, UnauthEmailConfirmUserErrorPage); 
            if (string.IsNullOrEmpty(uh))
            {
                Log.Info("Unauthenticated user email confirm request failed. Request with invalid parameter. User Hash is null or empty", uh);
            }
            else
            {
                try
                {
                    ConfirmEntityResult result = _usersDal.ConfirmEntity(uh, EntityType.UnAuthenticatedEmailAddress, c);
                    switch (result.Status)
                    {
                        case ConfirmStatus.CodeConfirmed:
                            redirectUri = HandleUnauthEmailConfirmed(uh, result);
                            break;
                        case ConfirmStatus.CodeWrong:
                            redirectUri = new Uri(_bingOffersBaseUri, string.Format(UnauthEmailConfirmExpiredCodePage, uh));
                            Log.Verbose("Unauthenticated user email confirm request failed. The user provided wrong code. User Id Hash = {0}", uh);
                            break;
                        case ConfirmStatus.Invalid:
                            redirectUri = new Uri(_bingOffersBaseUri, string.Format(UnauthEmailConfirmExpiredCodePage, uh));
                            Log.Verbose("Unauthenticated user email confirm request failed. The confirmation code is expired. User Id Hash = {0}", uh);
                            break;
                        case ConfirmStatus.CodeNotFound:
                            Log.Verbose("Unauthenticated user email confirm request failed. The confirmation code is not found in the system. User Id Hash = {0}", uh);
                            break;
                    }
                }
                //Ideally this should not happen. If it happens, just means something is terribly screwed up with this user record
                catch (UserNotExistsException invalidUserException)
                {
                    Log.Error(invalidUserException, "Email confirmed for the User Id Hash = {0}, but the user could not be located in the system", uh);
                }
                catch (Exception exp)
                {
                    Log.Error(exp, "Unexpected error in confirming unauthenticated user email. User Id Hash = {0}", uh);
                }
            }

            return new RedirectResult(redirectUri.ToString());
        }

        /// <summary>
        /// Controller method for confirming unauthenticated user account link code
        /// </summary>
        /// <param name="uh">Hash of user id</param>
        /// <param name="c">Confirmation code</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Alink(string uh, int c)
        {
            Log.Verbose("Start confirming account link code for unauthenticated user. User Id Hash = {0}", uh);
            Uri redirectUri = new Uri(_bingOffersBaseUri, UnauthAlinkConfirmUserErrorPage);
            if (string.IsNullOrEmpty(uh))
            {
                Log.Info("Unauthenticated user account link code confirm request failed. Request with invalid parameter. User Hash is null or empty", uh);
            }
            else
            {
                try
                {
                    ConfirmEntityResult result = _usersDal.ConfirmEntity(uh, EntityType.AccountLink, c);
                    switch (result.Status)
                    {
                        case ConfirmStatus.CodeConfirmed:
                            redirectUri = HandleUnauthAlinkConfirmed(uh, c, result);
                            break;
                        case ConfirmStatus.CodeWrong:
                            redirectUri = new Uri(_bingOffersBaseUri, string.Format(UnauthAlinkConfirmExpiredCodePage, uh));
                            Log.Verbose("Unauthenticated user account link code  confirm request failed. The user provided wrong code. User Id Hash = {0}", uh);
                            break;
                        case ConfirmStatus.Invalid:
                            redirectUri = new Uri(_bingOffersBaseUri, string.Format(UnauthAlinkConfirmExpiredCodePage, uh));
                            Log.Verbose("Unauthenticated user email account link code request failed. The confirmation code is expired. User Id Hash = {0}", uh);
                            break;
                        case ConfirmStatus.CodeNotFound:
                            Log.Verbose("Unauthenticated user email account link code request failed. The confirmation code is not found in the system. User Id Hash = {0}", uh);
                            break;
                    }
                }
                //Ideally this should not happen. If it happens, just means something is terribly screwed up with this user record
                catch (UserNotExistsException invalidUserException)
                {
                    Log.Error(invalidUserException, "Account link code  confirmed for the User Id Hash = {0}, but the user could not be located in the system", uh);
                }
                catch (Exception exp)
                {
                    Log.Error(exp, "Unexpected error in confirming unauthenticated user account link code. User Id Hash = {0}", uh);
                }
            }

            return new RedirectResult(redirectUri.ToString());
        }


        #region Private Methods

        /// <summary>
        /// Create an appropriate redirect uri for unauthenticated user email confirm request
        /// </summary>
        /// <param name="uh">Hash of unauthenticated user id</param>
        /// <param name="result">The confirm entity result</param>
        /// <returns>Redirect Uri</returns>
        private Uri HandleUnauthEmailConfirmed(string uh, ConfirmEntityResult result)
        {
            Uri redirectUri = null;

            //Mark the unauthenticated user email as confirmed
            _usersDal.UpdateUserEmail(result.UserId.Value, result.EntityId, true);
            User user = _usersDal.GetUserByUserId(result.UserId.Value);
            if (user != null)
            {
                //Check if the user's account is already linked and return the appropriate authentication provider info.
                if (!string.IsNullOrEmpty(user.MsId))
                {
                    redirectUri = user.MsId.IndexOf(FbIdPattern, StringComparison.OrdinalIgnoreCase) != -1
                                      ? new Uri(_bingOffersBaseUri, string.Format(UnauthEmailConfirmAccountLinkedPage, Facebook))
                                      : new Uri(_bingOffersBaseUri, string.Format(UnauthEmailConfirmAccountLinkedPage, Microsoft));
                }
                else
                {
                    Tuple<string, int> confirmationResponse = _usersDal.CreateConfirmationCode(user.Id.ToString(), EntityType.AccountLink, user.Id);
                    
                    redirectUri = new Uri(_bingOffersBaseUri, string.Format(UnauthEmailConfirmAccountNotLinkedPage, uh, confirmationResponse.Item2));
                }
            }
            Log.Verbose("Email Confirmed and updated for user. User Id={0} ", result.UserId);

            return redirectUri;
        }

        /// <summary>
        /// Create an appropriate redirect uri for unauthenticated user account link confirm request
        /// </summary>
        /// <param name="uh">Hash of User Id</param>
        /// <param name="activationToken">User's Account activation token</param>
        /// <param name="result">The confirm entity result</param>
        /// <returns>Redirect uri</returns>
        private Uri HandleUnauthAlinkConfirmed(string uh, int activationToken, ConfirmEntityResult result)
        {
            Uri redirectUri = null;
            
            User user = _usersDal.GetUserByUserId(result.UserId.Value);
            if (user != null)
            {
                //Check if the user's account is already linked and return the appropriate authentication provider info.
                if (!string.IsNullOrEmpty(user.MsId))
                {
                    redirectUri = user.MsId.IndexOf(FbIdPattern, StringComparison.OrdinalIgnoreCase) != -1
                                      ? new Uri(_bingOffersBaseUri, string.Format(UnauthAlinkConfirmAccountLinkedPage, Facebook))
                                      : new Uri(_bingOffersBaseUri, string.Format(UnauthAlinkConfirmAccountLinkedPage, Microsoft));
                }
                else
                {
                    redirectUri = new Uri(_bingOffersBaseUri, string.Format(UnauthAlinkConfirmAccountNotLinkedPage, uh, activationToken));
                }
            }

            return redirectUri;
        }

        #endregion
    }
}