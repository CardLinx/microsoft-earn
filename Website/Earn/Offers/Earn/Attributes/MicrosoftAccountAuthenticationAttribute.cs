//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Helper;
using Lomo.Authentication;
using Lomo.Authentication.Tokens;
using Microsoft.Passport.RPS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Earn.Offers.Earn.Attributes
{
    public class MicrosoftAccountAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            HandleMicrosoftPassportAuthentication(filterContext);
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }

        private bool IsRequestForOffice(AuthenticationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            return context.Request.Url.Authority.ToLower().Contains("offers.office.com") ? true : false;
        }

        private void HandleMicrosoftPassportAuthentication(AuthenticationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;

            Microsoft.Passport.RPS.RPS myRps = (Microsoft.Passport.RPS.RPS)context.Application["globalRPS"];
            string siteName = ConfigurationManager.AppSettings["SiteName"];
            Microsoft.Passport.RPS.RPSHttpAuth httpAuth = new Microsoft.Passport.RPS.RPSHttpAuth(myRps);
            RPSPropBag authPropBag = new RPSPropBag(myRps);
            RPSDomainMap domainMap = new RPSDomainMap(myRps);
            RPSServerConfig mainConfig = new RPSServerConfig(myRps);
            RPSPropBag siteConfig = (RPSPropBag)mainConfig["Sites"];
            RPSPropBag mySiteConfig = (RPSPropBag)siteConfig[siteName];
            int siteID = Convert.ToInt32(mySiteConfig["SiteID"]);
            var returnUrlBuilder = new UriBuilder(context.Request.Url);
            returnUrlBuilder.Scheme = Uri.UriSchemeHttps;
            returnUrlBuilder.Port = 443;
            authPropBag["ReturnURL"] = returnUrlBuilder.ToString();
            authPropBag["SiteID"] = siteID;
            RPSTicket ticket = null;

            try
            {
                ticket = httpAuth.Authenticate(siteName, HttpContext.Current.Request, authPropBag);
            }
            catch (Exception e)
            {
                ticket = null;
            }

            // Get the user's 'authState' to determine whether user is currently signed in.
            uint authState = (uint)authPropBag["RPSAuthState"];

            if (ticket == null)
            {
                //No RPSTicket found.  Check for AuthState=2 (Maybe) state.
                if (authState == 2)
                {
                    // RPS Maybe state detected. Indicates that a ticket is present
                    // but cannot be read. Redirect to SilentAuth URL to obtain
                    // a fresh RPS Ticket. Write RPS response headers to write
                    // the Maybe state cookie to prevent looping.
                    string rpsHeaders = (string)authPropBag["RPSRespHeaders"];
                    if (rpsHeaders != "")
                    {
                        httpAuth.WriteHeaders(HttpContext.Current.Response, rpsHeaders);
                    }

                    string silentAuthUrl = domainMap.ConstructURL("SilentAuth", siteName, null, authPropBag);
                    HttpContext.Current.Response.Redirect(silentAuthUrl);
                }
                else
                {
                    bool showSignIn = true;
                    bool isSecure = HttpContext.Current.Request.IsSecureConnection;
                    string signInHtml = httpAuth.LogoTag(showSignIn, isSecure, null, null, siteName, authPropBag);
                    LiveIdAuthResult result = new LiveIdAuthResult();
                    result.IsUserAuthenticated = false;
                    result.SignInHtmlLink = signInHtml;
                    context.Items["liveauthstate"] = result;
                }
            }
            else
            {
                // RPS ticket found. Ensure ticket is valid (signature is valid
                // and policy criteria such as time window and use of SSL are met).
                bool isValid = ticket.Validate(authPropBag);
                if (!isValid)
                {
                    // RPS ticket exists, but is not valid. Refresh ticket by
                    // redirecting user to "Auth" URL. If appropriate Login Server
                    // cookies exist, ticket refresh will be transparent to the user.
                    HttpContext.Current.Response.Redirect(domainMap.ConstructURL("Auth", siteName, null, authPropBag));
                }
                else
                {
                    //// RPS ticket exists and is valid. Show page with sign-out
                    //// button and user's NetID.


                    bool showSignIn = false;
                    bool isSecure = HttpContext.Current.Request.IsSecureConnection;
                    string signOutHtml = httpAuth.LogoTag(showSignIn, isSecure, null, null, siteName, authPropBag);

                    LiveIdAuthResult result = new LiveIdAuthResult();
                    result.IsUserAuthenticated = true;
                    result.SignOutHtmlLink = signOutHtml;

                    string puid = (string)ticket.Property["HexPUID"];
                    uint ticketType = ticket.TicketType;
                    string clearTicket = (string)ticket.Property["ClearTicket"];
                    System.UInt32 issueInstant = (System.UInt32)ticket.Property["IssueInstant"];

                    if (ticket != null & ticket.ProfileProperty["memberName"] != null)
                    {
                        string profileName = (string)ticket.ProfileProperty["memberName"];
                        string firstName = (string)ticket.ProfileProperty["firstname"];
                        string lastName = (string)ticket.ProfileProperty["lastname"];
                        string gender = (string)ticket.ProfileProperty["gender"];
                        DateTime? birthdate = (DateTime?)ticket.ProfileProperty["birthdate"];
                        string city = (string)ticket.ProfileProperty["city"];
                        int region = (int)ticket.ProfileProperty["region"];
                        string country = (string)ticket.ProfileProperty["country"];
                        string postalcode = (string)ticket.ProfileProperty["postalcode"];
                        string revipcountry = (string)ticket.ProfileProperty["revipcountry"];

                        Int32 myflags = myflags = (Int32)ticket.ProfileProperty["flags"];
                        bool f2bit = (myflags & 0x2) == 0x2; //Hotmail enabled
                        bool f4bit = (myflags & 0x4) == 0x4;  // Mobile Subscriber
                        bool f5bit = (myflags & 0x10) == 0x10;  // User is blocked
                        bool f10bit = (myflags & 0x200) == 0x200; //Mobile Subscriber
                        bool f11bit = (myflags & 0x400) == 0x400;  //E-mail Address Verified
                        bool f12bit = (myflags & 0x800) == 0x800;  //Primary MSNIA subscription
                        bool f13bit = (myflags & 0x1000) == 0x1000; //Mobile subscription
                        bool f14bit = (myflags & 0x2000) == 0x2000; //Hotmail Premium subscription
                        bool f16bit = (myflags & 0x8000) == 0x8000;  //Managed domain
                        bool f17bit = (myflags & 0x10000) == 0x10000; //MSN subscription-no age-out
                        bool f18bit = (myflags & 0x20000) == 0x20000; //Secondary MSNIA subscription
                        bool f19bit = (myflags & 0x40000) == 0x40000; //Verizon user
                        bool f20bit = (myflags & 0x80000) == 0x80000; //MSN Direct subscription 
                        bool f21bit = (myflags & 0x100000) == 0x100000; //User does not age out
                        bool f22bit = (myflags & 0x200000) == 0x200000; //HIP challenge not required
                        bool f30bit = (myflags & 0x20000000) == 0x20000000; //WLTOU Not accepted

                        // Get RPS response headers from authPropBag and write to response stream
                        string rpsHeaders = (string)authPropBag["RPSRespHeaders"];
                        if (rpsHeaders != "")
                        {
                            httpAuth.WriteHeaders(HttpContext.Current.Response, rpsHeaders);
                            result.RpsHeaders = rpsHeaders;
                        }

                        result.ProfileName = firstName + " " + lastName;
                    }

                    result.Anid = TokenHelper.ConvertPuidToAnid(puid);
                    context.Items["liveauthstate"] = result;
                    filterContext.Principal = TokenHelper.GetClaimsPrincipal(result.Anid, result.ProfileName, "custompassportauth", puid);

                    string data = WriteAuthDataToAuthCookie(puid, result.ProfileName, null);
                    context.Items["backendtoken"] = data;

                    // Convert Post to Get
                    var query = new NameValueCollection(context.Request.QueryString);
                    var wa = query["wa"];
                    if (string.Compare(wa, "wsignin1.0", true) == 0)
                    {
                        query.Remove("wa");
                        UriBuilder builder = new UriBuilder(context.Request.Url);
                        builder.Query = ConvertToQueryString(query);
                        filterContext.HttpContext.Response.Redirect(builder.ToString());
                    }
                }
            }
        }

        private string WriteAuthDataToAuthCookie(string puid, string profileName, string email)
        {
            try
            {
                LomoUserIdSecurityToken token = new LomoUserIdSecurityToken(puid.ToLower(), Constants.TokenIssuer, Constants.TokenResource, Constants.TokenAction, 86400, Constants.TokenSigningKey, Constants.TokenEncryptionKey, Constants.TokenEcryptionSalt);
                if (!string.IsNullOrWhiteSpace(profileName))
                {
                    token.AddClaim(LomoClaimTypes.NameClaimType, profileName);
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    token.AddClaim(LomoClaimTypes.EmailClaimType, email);
                }

                string data = Constants.CustomMSAPrefix + token.ToString();

                string authCookieName = ConfigurationManager.AppSettings["AuthCookieName"];
                HttpCookie authCookie = new HttpCookie(authCookieName, HttpUtility.UrlEncode(data));
                authCookie.Domain = ConfigurationManager.AppSettings["RootDomain"];
                authCookie.Expires = DateTime.UtcNow.Add(TimeSpan.FromDays(1));
                HttpContext.Current.Response.Cookies.Add(authCookie);
                return data;
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        private string ConvertToQueryString(NameValueCollection coll)
        {
            StringBuilder str = new StringBuilder();
            foreach (string key in coll.Keys)
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(coll[key]))
                {
                    str.Append(string.Format("{0}={1}&", key, HttpUtility.UrlEncode(coll[key])));
                }
            }

            if (str.Length > 0 && str[str.Length - 1] == '&')
            {
                str = str.Remove(str.Length - 1, 1);
            }

            return str.ToString();
        }
    }
}