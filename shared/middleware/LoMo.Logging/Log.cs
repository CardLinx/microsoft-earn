//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The log.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Logging
{
    using System;
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// The log.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Initializes static members of the <see cref="Log"/> class.
        /// </summary>
        static Log()
        {
            string result = ConfigurationManager.AppSettings["enableeventlog"];
            if (result == "true")
            {
                var eventlogVerbosity = SourceLevels.All;
                var eventlogVerbosityString = ConfigurationManager.AppSettings["eventlogVerbosity"];
                if (!string.IsNullOrEmpty(eventlogVerbosityString))
                {
                    //All = -1, Off = 0, Critical = 1, Error = 3,  Warning = 7, Information = 15,  Verbose = 31
                    var eventlogVerbosityInt = Convert.ToInt32(eventlogVerbosityString);
                    eventlogVerbosity = (SourceLevels) eventlogVerbosityInt;
                }

                Instance = new EventLogLog(eventlogVerbosity);
                Instance.Source = ConfigurationManager.AppSettings["eventlogsourcename"];

                var defaultEventId = ConfigurationManager.AppSettings["defaultEventLogEventId"];
                if (!string.IsNullOrEmpty(defaultEventId))
                {
                    Instance.DefaultEventId = Convert.ToInt32(defaultEventId);
                }
            }
            else
            {
                Instance = new TraceLog();
            }
        }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public static ILog Instance { private get; set; }

        /// <summary>
        /// Sets the logs source
        /// </summary>
        public static string Source
        {
            set { Instance.Source = value; }
        }

        #region Verbose

        /// <summary>
        /// Creates Verbose log 
        /// </summary>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Verbose(string format, params object[] args)
        {
            Instance.Verbose(format, args);
        }

        /// <summary>
        /// Creates Verbose log 
        /// </summary>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Verbose(Guid activityId, string format, params object[] args)
        {
            Instance.Verbose(activityId, format, args);
        }

        #endregion

        #region Info

        /// <summary>
        /// Creates Information log 
        /// </summary>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Info(string format, params object[] args)
        {
            Instance.Info(format, args);
        }

        /// <summary>
        /// Creates Information log 
        /// </summary>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Info(Guid activityId, string format, params object[] args)
        {
            Instance.Info(activityId, format, args);
        }

        /// <summary>
        /// Creates Information log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Info(int eventId, string format, params object[] args)
        {
            Instance.Info(eventId, format, args);
        }

        /// <summary>
        /// Creates Information log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Info(int eventId, Guid activityId, string format, params object[] args)
        {
            Instance.Info(eventId, activityId, format, args);
        }

        #endregion

        #region Warn

        /// <summary>
        /// Creates Warning log 
        /// </summary>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Warn(string format, params object[] args)
        {
            Instance.Warn(format, args);
        }

        /// <summary>
        /// Creates Warning log 
        /// </summary>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Warn(Guid activityId, string format, params object[] args)
        {
            Instance.Warn(activityId, format, args);
        }

        /// <summary>
        /// Creates Warning log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Warn(int eventId, string format, params object[] args)
        {
            Instance.Warn(eventId, format, args);
        }

        /// <summary>
        /// Creates Warning log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Warn(int eventId, Guid activityId, string format, params object[] args)
        {
            Instance.Warn(eventId, activityId, format, args);
        }

        #endregion

        #region Error

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(string format, params object[] args)
        {
            Instance.Error(format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(Guid activityId, string format, params object[] args)
        {
            Instance.Error(activityId, format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(int eventId, string format, params object[] args)
        {
            Instance.Error(eventId, format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(int eventId, Guid activityId, string format, params object[] args)
        {
            Instance.Error(eventId, activityId, format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="exception">
        /// the exception 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(Exception exception, string format, params object[] args)
        {
            Instance.Error(exception, format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="exception">
        /// the exception 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(Guid activityId, Exception exception, string format, params object[] args)
        {
            Instance.Error(activityId, exception, format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="exception">
        /// the exception 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(int eventId, Exception exception, string format, params object[] args)
        {
            Instance.Error(eventId, exception, format, args);
        }

        /// <summary>
        /// Creates Error log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="exception">
        /// the exception 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Error(int eventId, Guid activityId, Exception exception, string format, params object[] args)
        {
            Instance.Error(eventId, activityId, exception, format, args);
        }

        #endregion

        #region Critical

        /// <summary>
        /// Creates Critical log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Critical(int eventId, string format, params object[] args)
        {
            Instance.Critical(eventId, format, args);
        }

        /// <summary>
        /// Creates Critical log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Critical(int eventId, Guid activityId, string format, params object[] args)
        {
            Instance.Critical(eventId, activityId, format, args);
        }

        /// <summary>
        /// Creates Critical log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="exception">
        /// the exception 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Critical(int eventId, Exception exception, string format, params object[] args)
        {
            Instance.Critical(eventId, exception, format, args);
        }

        /// <summary>
        /// Creates Critical log 
        /// </summary>
        /// <param name="eventId">
        /// event id 
        /// </param>
        /// <param name="activityId">
        /// activity/ transaction id
        /// </param>
        /// <param name="exception">
        /// the exception 
        /// </param>
        /// <param name="format">
        /// format string that contains zero or more format items, which correspond to objects in the args array. 
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format. 
        /// </param>
        public static void Critical(int eventId, Guid activityId, Exception exception, string format, params object[] args)
        {
            Instance.Critical(eventId, activityId, exception, format, args);
        }

        #endregion
    }
}