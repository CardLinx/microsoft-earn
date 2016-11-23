//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Diagnostics;
using System.Web.Mvc;
using Earn.Dashboard.Web.Utils;

namespace Earn.Dashboard.Web.Attributes
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        private const string StopwatchKey = "LoggingStopWatch";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Stopwatch sw = Stopwatch.StartNew();
            filterContext.HttpContext.Items.Add(StopwatchKey, sw);
            var request = filterContext.HttpContext.Request;
            Log.Verbose("Executing \ncontroller: {0}, \naction :{1}, \nurl: {2}, \nclientIp:{3}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName,
                request.RawUrl,
                request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress);

        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Items[StopwatchKey] != null)
            {
                Stopwatch sw = (Stopwatch)filterContext.HttpContext.Items[StopwatchKey];
                sw.Stop();
                Log.Verbose("Finished executing \ncontroller: {0}, \naction: {1}, \nelapsed: {2}",
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.ActionDescriptor.ActionName,
                    sw.ElapsedMilliseconds);
                filterContext.HttpContext.Items.Remove(StopwatchKey);
            }
        }
    }
}