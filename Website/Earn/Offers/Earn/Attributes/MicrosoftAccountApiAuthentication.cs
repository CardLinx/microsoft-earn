//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Helper;
using Lomo.Authentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Attributes
{
    public class MicrosoftAccountApiAuthentication : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
         HttpRequestMessage request, CancellationToken cancellationToken)
        {
            processRequest(request, cancellationToken);
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private void processRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string authCookieName = ConfigurationManager.AppSettings["AuthCookieName"];
            CookieHeaderValue cookie = request.Headers.GetCookies(authCookieName).FirstOrDefault();
            if (cookie == null)
            {
                return;
            }

            string blob = cookie[authCookieName].Value;

            if (string.IsNullOrWhiteSpace(blob))
            {
                return;
            }

            LomoUserAccountInfo accountInfo = BingSocialAccessorRepository.GetUserAccountInfo(blob);
            if (accountInfo != null && accountInfo.IsAuthenticated && accountInfo.ProviderType == IdentityProviderType.MicrosoftAccount)
            {
                ClaimsPrincipal principal = TokenHelper.GetClaimsPrincipal(null, accountInfo.Name, accountInfo.ProviderType.ToString("g"), accountInfo.UserId);
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
        }
    }
}