//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Earn.Dashboard.Web.Service;
using Earn.Dashboard.Web.Utils;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Earn.Dashboard.Web.Controllers
{
    public class AccountController : Controller
    {
        ////public void SignIn()
        ////{
        ////    // Send an OpenID Connect sign-in request.
        ////    if (!Request.IsAuthenticated)
        ////    {
        ////        HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
        ////            OpenIdConnectAuthenticationDefaults.AuthenticationType);
        ////    }
        ////}

        [AllowAnonymous]
        public void SignOut()
        {
            if (HttpContext.Session != null)
                HttpContext.Session.Clear();
            string callbackUrl = Url.Action("Index", "Home", routeValues: null, protocol: Request.Url.Scheme);
            HttpContext.GetOwinContext().Authentication.SignOut(new AuthenticationProperties { RedirectUri = callbackUrl }, OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        [AllowAnonymous]
        public void RequestAccess()
        {
            EmailService.Send(string.Format("{0} has requested 'Earn Dashboard' Access.", ClaimsPrincipal.Current.Identity.Name), ConfigurationManager.AppSettings["AdminEmailAddress"]);
        }

        public async Task<ActionResult> Index()
        {
            ActiveDirectoryClient activeDirectoryClient = GraphHelper.ActiveDirectoryClient();
            var users = await activeDirectoryClient.Users.Where(u => u.ObjectId.Equals(GraphHelper.UserObjectId)).ExecuteAsync();
            IUser user = users.CurrentPage.ToList().First();

            return View(user);
        }
    }
}