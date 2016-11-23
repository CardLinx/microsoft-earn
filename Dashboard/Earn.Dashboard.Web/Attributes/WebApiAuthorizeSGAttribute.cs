//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Earn.Dashboard.Web.Models;
using Earn.Dashboard.Web.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.ActiveDirectory.GraphClient;

namespace Earn.Dashboard.Web.Attributes
{
    public class WebApiAuthorizeSGAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (AuthorizeRequest(actionContext))
            {
                return;
            }

            HandleUnauthorizedRequest(actionContext);
        }

        public bool AuthorizeRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            string userObjectId = GraphHelper.UserObjectId;
            if (AuthorizedUsersCache.Instance.Contains(userObjectId))
            {
                return true;
            }
            else
            {
                try
                {
                    ActiveDirectoryClient activeDirectoryClient = GraphHelper.ActiveDirectoryClient();
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
                        return false;
                    }
                    else
                    {
                        AuthorizedUsersCache.Instance.AddToCache(userObjectId, new List<string> { Config.Roles.Admin });
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
        }
    }
}