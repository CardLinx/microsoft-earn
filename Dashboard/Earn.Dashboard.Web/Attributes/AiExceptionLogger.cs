//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Microsoft.ApplicationInsights;

namespace Earn.Dashboard.Web.Attributes
{
    public class AiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context != null && context.Exception != null)
            {
                // check for System.Web.HttpException - The remote host closed the connection. The error code is 0x80070057.
                // This happens when browser closes connection.
                bool log = true;
                if (context.Exception is HttpException)
                {
                    var ex = context.Exception as HttpException;
                    if (ex.ErrorCode == unchecked((int)0x80070057)) //Error Code = -2147024809
                    {
                        log = false;
                    }
                }

                if (log)
                {
                    var telemetry = new TelemetryClient();
                    telemetry.TrackException(context.Exception, new Dictionary<string, string> { { "Source", "AiExceptionLogger" }, { "url", context.Request.RequestUri.ToString() } });
                    Utils.Log.Error(context.Exception, "url: {0}", context.Request.RequestUri);
                }
            }

            base.Log(context);
        }
    }
}