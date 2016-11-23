//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.Attributes
{
    using System.Web.Http.ExceptionHandling;

    using Microsoft.ApplicationInsights;

    public class AiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context != null && context.Exception != null)
            {
                var telemetry = new TelemetryClient();
                telemetry.TrackException(context.Exception);
            }

            base.Log(context);
        }
    }
}