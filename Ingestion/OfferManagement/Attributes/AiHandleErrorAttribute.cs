//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.Attributes
{
    using System.Web.Mvc;

    using Microsoft.ApplicationInsights;

    public class AiHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.HttpContext != null && filterContext.Exception != null)
            {
                // If customError is Off, then AI HTTPModule will report the exception
                // If it is On / RemoteOnly (default) - then we need to explicitly track the exception
                if (filterContext.HttpContext.IsCustomErrorEnabled)
                {
                    var telemetry = new TelemetryClient();
                    telemetry.TrackException(filterContext.Exception);
                }
            }

            base.OnException(filterContext);
        }
    }
}