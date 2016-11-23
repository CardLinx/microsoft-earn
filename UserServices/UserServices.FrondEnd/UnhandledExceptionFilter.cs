//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The unhandled exception filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd
{
    using System.Web.Http;
    using System.Web.Http.Filters;

    using Lomo.Logging;

    /// <summary>
    ///     The unhandled exception filter.
    /// </summary>
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        #region Public Methods and Operators

        /// <summary>
        /// The on exception.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception.GetType() != typeof(HttpResponseException))
            {
                Log.Error(context.Exception, "Unhandled api error");
            }
        }

        #endregion
    }
}