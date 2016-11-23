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
    public class SimpleWebTokenAuthAttribute : IApiAuthAttribute
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
            Thread.CurrentPrincipal = new CustomPrincipal(new CustomIdentity("0000000000000000000000000000000000000000",
                                                                             "All", "Test"), new string[] { });
        }

        #endregion
    }
}