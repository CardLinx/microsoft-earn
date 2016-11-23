//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The http only filter attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// The http only filter attribute.
    /// </summary>
    public class HttpsOnlyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The on action executing.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!string.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "HTTPS Required");
                actionContext.Response = response;
            }
        }
    }
}