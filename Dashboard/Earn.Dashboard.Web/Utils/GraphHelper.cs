//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Earn.Dashboard.Web.Utils
{
    public static class GraphHelper
    {
        public const string GraphResourceId = "https://graph.windows.net";
        public static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static readonly string ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static readonly string AADInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static readonly string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public static readonly string PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        public static readonly string Authority = GraphHelper.AADInstance + GraphHelper.TenantId;

        public static string SignedInUserId
        {
            get { return ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value; }
        }

        public static string UserObjectId
        {
            get { return ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value; }
        }

        public static ActiveDirectoryClient ActiveDirectoryClient()
        {
            Uri servicePointUri = new Uri(GraphResourceId);
            Uri serviceRoot = new Uri(servicePointUri, TenantId);
            return new ActiveDirectoryClient(serviceRoot, async () => await AcquireTokenSilentAsync());
        }

        public static async Task<string> AcquireTokenSilentAsync()
        {
            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientCred = new ClientCredential(ClientId, ClientSecret);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            //AuthenticationContext authenticationContext = new AuthenticationContext(Authority, new ADALTokenCache(SignedInUserId));
            AuthenticationContext authenticationContext = new AuthenticationContext(Authority, false);
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(GraphResourceId, clientCred, new UserIdentifier(UserObjectId, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}