//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System.Web.Http.Controllers;

    /// <summary>
    ///     Interface for the ApiAuth attribute.
    /// </summary>
    public interface IApiAuthAttribute
    {
        /// <summary>
        /// Attempts to authorize the user for the request.
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        void AuthorizeUser(HttpActionContext actionContext);
    }
}