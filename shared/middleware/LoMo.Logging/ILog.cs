//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Log interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Logging
{
    using System;

    /// <summary>
    /// The Log interface.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Sets the logs source
        /// </summary>
        string Source { set; }

        /// <summary>
        /// Gets or sets the default event id.
        /// </summary>
        int? DefaultEventId { set; get; }

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
        void Verbose(string format, params object[] args);

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
        void Verbose(Guid activityId, string format, params object[] args);
        
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
        void Info(string format, params object[] args);

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
        void Info(Guid activityId, string format, params object[] args);

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
        void Info(int eventId, string format, params object[] args);

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
        void Info(int eventId, Guid activityId, string format, params object[] args);

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
        void Warn(string format, params object[] args);

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
        void Warn(Guid activityId, string format, params object[] args);

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
        void Warn(int eventId, string format, params object[] args);

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
        void Warn(int eventId, Guid activityId, string format, params object[] args);

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
        void Error(string format, params object[] args);

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
        void Error(Guid activityId, string format, params object[] args);

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
        void Error(int eventId, string format, params object[] args);

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
        void Error(int eventId, Guid activityId, string format, params object[] args);

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
        void Error(Exception exception, string format, params object[] args);

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
        void Error(Guid activityId, Exception exception, string format, params object[] args);

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
        void Error(int eventId, Exception exception, string format, params object[] args);

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
        void Error(int eventId, Guid activityId, Exception exception, string format, params object[] args);

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
        void Critical(int eventId, string format, params object[] args);

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
        void Critical(int eventId, Guid activityId, string format, params object[] args);

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
        void Critical(int eventId, Exception exception, string format, params object[] args);

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
        void Critical(int eventId, Guid activityId, Exception exception, string format, params object[] args);

        #endregion
    }
}