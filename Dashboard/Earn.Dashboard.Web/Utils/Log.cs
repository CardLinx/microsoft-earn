//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Diagnostics;
using System.Text;

namespace Earn.Dashboard.Web.Utils
{
    public static class Log
    {
        private static readonly TraceSource TraceSource;

        static Log()
        {
            TraceSource = new TraceSource("EarnTraceSource", SourceLevels.All);
        }

        public static void Verbose(string format, params object[] args)
        {
            LogEvent(TraceEventType.Verbose, 5, null, format, args);
        }

        public static void Info(string format, params object[] args)
        {
            LogEvent(TraceEventType.Information, 4, null, format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            LogEvent(TraceEventType.Warning, 3, null, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            LogEvent(TraceEventType.Error, 2, null, format, args);
        }
        public static void Error(Exception exception, string format, params object[] args)
        {
            LogEvent(TraceEventType.Error, 2, exception, format, args);
        }

        public static void Critical(string format, params object[] args)
        {
            LogEvent(TraceEventType.Critical, 1, null, format, args);
        }

        public static void Critical(Exception exception, string format, params object[] args)
        {
            LogEvent(TraceEventType.Critical, 1, exception, format, args);
        }

        private static void LogEvent(TraceEventType eventType, int eventId, Exception exception, string format, params object[] args)
        {
            var msg = new StringBuilder();
            if (!string.IsNullOrEmpty(format))
            {
                if (args == null || args.Length == 0)
                {
                    msg.Append(format + ";");
                }
                else
                {
                    try
                    {
                        string formatedMsg = string.Format(format, args);
                        msg.Append(formatedMsg + ";");
                    }
                    catch (Exception)
                    {
                        msg.Append(format + string.Format(" [Couldn't format the message with the given args. Args Count: {0}];", args.Length));
                    }
                }
            }

            if (exception != null)
            {
                msg.Append(string.Format("Error: {0};", exception));
            }

            string logDatetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            msg.AppendFormat("Datetime={0};", logDatetime);
            TraceSource.TraceEvent(eventType, eventId, msg.ToString());
        }
    }
}