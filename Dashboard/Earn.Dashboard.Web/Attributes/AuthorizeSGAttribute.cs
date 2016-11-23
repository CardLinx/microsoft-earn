//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Earn.Dashboard.Web.Models;
using Earn.Dashboard.Web.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.ActiveDirectory.GraphClient;

namespace Earn.Dashboard.Web.Attributes
{
    public class AuthorizeSGAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.Session == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (base.AuthorizeCore(httpContext))
            {
                if (httpContext.Session[Config.IsAuthorized] == null)
                {
                    if (AuthorizedUsersCache.Instance.Contains(GraphHelper.UserObjectId))
                    {
                        httpContext.Session.Add(Config.IsAuthorized, true);
                        httpContext.Session.Add(Config.Role, AuthorizedUsersCache.Instance[GraphHelper.UserObjectId]);
                        return true;
                    }
                    else
                    {
                        try
                        {
                            ActiveDirectoryClient activeDirectoryClient = GraphHelper.ActiveDirectoryClient();
                            // user thumbnail photo
                            var users = Task.Run(async () => await activeDirectoryClient.Users.Where(u => u.ObjectId.Equals(GraphHelper.UserObjectId)).ExecuteAsync()).Result;
                            IUser user = users.CurrentPage.ToList().First();
                            if (user.ThumbnailPhoto.ContentType != null)
                            {
                                DataServiceStreamResponse response = Task.Run(async () => await user.ThumbnailPhoto.DownloadAsync()).Result;
                                using (var ms = new MemoryStream())
                                {
                                    response.Stream.CopyTo(ms);
                                    string imageBase64 = Convert.ToBase64String(ms.ToArray());
                                    string image = string.Format("data:image/gif;base64,{0}", imageBase64);
                                    httpContext.Session.Add(Config.ThumbnailPhoto, image);
                                    httpContext.Response.Cookies.Add(new HttpCookie(Config.ThumbnailPhotoCookie) { Secure = true, Value = HttpUtility.UrlEncode(image) });
                                }
                            }

                            List<string> userRoles = new List<string>();
                            // earnit-admin security group
                            bool? isUserInRole = Task.Run(async () => await activeDirectoryClient.IsMemberOfAsync(Config.SecurityGroups[Config.Roles.Admin], GraphHelper.UserObjectId)).Result;
                            if (isUserInRole.GetValueOrDefault(false))
                            {
                                userRoles.Add(Config.Roles.Admin);
                            }

                            // earnit security group
                            isUserInRole = Task.Run(async () => await activeDirectoryClient.IsMemberOfAsync(Config.SecurityGroups[Config.Roles.User], GraphHelper.UserObjectId)).Result;
                            if (isUserInRole.GetValueOrDefault(false))
                            {
                                userRoles.Add(Config.Roles.User);
                            }

                            // earnit-support security group
                            isUserInRole = Task.Run(async () => await activeDirectoryClient.IsMemberOfAsync(Config.SecurityGroups[Config.Roles.Support], GraphHelper.UserObjectId)).Result;
                            if (isUserInRole.GetValueOrDefault(false))
                            {
                                userRoles.Add(Config.Roles.Support);
                            }

                            if (userRoles.Any())
                            {
                                httpContext.Session.Add(Config.IsAuthorized, true);
                                httpContext.Session.Add(Config.Role, userRoles);
                                AuthorizedUsersCache.Instance.AddToCache(GraphHelper.UserObjectId, userRoles);
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Config.IsProduction)
                            {
                                Log.Critical(ex, "ActiveDirectoryClient failed in AuthorizeSGAttribute");
                                TelemetryClient telemetry = new TelemetryClient();
                                telemetry.TrackException(ex);
                            }
                            else
                            {
                                List<string> admin = new List<string> { Config.Roles.Admin };
                                httpContext.Session.Add(Config.IsAuthorized, true);
                                httpContext.Session.Add(Config.Role, admin);
                                AuthorizedUsersCache.Instance.AddToCache(GraphHelper.UserObjectId, admin);
                                return true;
                            }
                        }

                        httpContext.Session.Add(Config.IsAuthorized, false);
                        httpContext.Session.Add(Config.Role, new List<string> { Config.Roles.Visitor });
                        AuthorizedUsersCache.Instance.Remove(GraphHelper.UserObjectId);
                    }
                }

                return (bool)httpContext.Session[Config.IsAuthorized];
            }

            if (!string.IsNullOrWhiteSpace(this.Roles))
            {
                string[] authorizedRoles = this.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> userRoles = (List<string>)httpContext.Session[Config.Role];

                return authorizedRoles.Intersect(userRoles).Any();
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                TelemetryClient telemetry = new TelemetryClient();
                var properties = new Dictionary<string, string>
                {
                    {"Name", filterContext.HttpContext.User.Identity.Name},
                    {"Url", filterContext.HttpContext.Request.Url.ToString()}
                };

                telemetry.TrackEvent("Authorized Access", properties);
                var result = new ViewResult { ViewName = "~/Views/Error/AccessDenied.cshtml" };
                filterContext.Result = result;
            }
            else
                base.HandleUnauthorizedRequest(filterContext);
        }
    }
}