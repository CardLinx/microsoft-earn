//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization.Test
{
    using System.Security.Cryptography.X509Certificates;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Net;
    using System.Threading;

    /// <summary>
    /// A mock implementation of the IApiAuthAttribute interface.
    /// </summary>
    public class MutualSslAuthAttribute : IApiAuthAttribute
    {
        #region Public Methods and Operators

        /// <summary>
        /// Attempts to authorize the user for the request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public void AuthorizeUser(HttpActionContext actionContext)
        {
            // A certificate will only be specified when attempting to test an Unauthorized scenario. Otherwise, no certificate
            // should be presented, because there's little guarantee as to which certificates are installed on anyone's dev box.
            X509Certificate2 certificate = actionContext.Request.GetClientCertificate();
            if (certificate == null)
            {
                Thread.CurrentPrincipal = new CustomPrincipal(new CustomIdentity("0000000000000000000000000000000000000000",
                                                                                 "Fake Certificate", "Test"), new string[] {});

            }
            else
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        #endregion
    }
}