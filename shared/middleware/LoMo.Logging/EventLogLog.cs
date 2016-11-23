//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The event log.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Logging
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// The event log.
    /// </summary>
    public class EventLogLog : ILog
    {
        #region Data Members

        /// <summary>
        /// The EventLog object to which log entries will be sent.
        /// </summary>
        private readonly EventLog eventLog;

        /// <summary>
        /// The level of EventType to commit to the EventLog.
        /// </summary>
        private readonly SourceLevels verbosity;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogLog"/> class.
        /// </summary>
        /// <param name="logVerbosity">
        /// The level of EventType to commit to the EventLog.
        /// </param>
        public EventLogLog(SourceLevels logVerbosity)
        {
            eventLog = new EventLog();
            verbosity = logVerbosity;
        }

        #endregion

        #region ILogger imp

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source
        {
            private get
            {
                return eventLog.Source;
            }

            set
            {
                eventLog.Source = value;
            }
        }

        /// <summary>
        /// Gets or sets the default event id.
        /// </summary>
        public int? DefaultEventId
        {
            get; 
            set;
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Verbose(string format, params object[] args)
        {
            Log(EventType.Verbose, null, null, null, format, args);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Verbose(Guid activityId, string format, params object[] args)
        {
            Log(EventType.Verbose, null, activityId, null, format, args);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Info(string format, params object[] args)
        {
            Log(EventType.Information, null, null, null, format, args);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.   
        /// </param>
        public void Info(Guid activityId, string format, params object[] args)
        {
            Log(EventType.Information, null, activityId, null, format, args);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Info(int eventId, string format, params object[] args)
        {
            Log(EventType.Information, eventId, null, null, format, args);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Info(int eventId, Guid activityId, string format, params object[] args)
        {
            Log(EventType.Information, eventId, activityId, null, format, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Warn(string format, params object[] args)
        {
            Log(EventType.Warning, null, null, null, format, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Warn(Guid activityId, string format, params object[] args)
        {
            Log(EventType.Warning, null, activityId, null, format, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Warn(int eventId, string format, params object[] args)
        {
            Log(EventType.Warning, eventId, null, null, format, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Warn(int eventId, Guid activityId, string format, params object[] args)
        {
            Log(EventType.Warning, eventId, activityId, null, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(string format, params object[] args)
        {
            Log(EventType.Error, null, null, null, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(Guid activityId, string format, params object[] args)
        {
            Log(EventType.Error, null, activityId, null, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(int eventId, string format, params object[] args)
        {
            Log(EventType.Error, eventId, null, null, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(int eventId, Guid activityId, string format, params object[] args)
        {
            Log(EventType.Error, eventId, activityId, null, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(Exception exception, string format, params object[] args)
        {
            Log(EventType.Error, null, null, exception, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(Guid activityId, Exception exception, string format, params object[] args)
        {
            Log(EventType.Error, null, activityId, exception, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(int eventId, Exception exception, string format, params object[] args)
        {
            Log(EventType.Error, eventId, null, exception, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Error(int eventId, Guid activityId, Exception exception, string format, params object[] args)
        {
            Log(EventType.Error, eventId, activityId, exception, format, args);
        }

        /// <summary>
        /// The critical.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Critical(int eventId, string format, params object[] args)
        {
            Log(EventType.Critical, eventId, null, null, format, args);
        }

        /// <summary>
        /// The critical.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Critical(int eventId, Guid activityId, string format, params object[] args)
        {
            Log(EventType.Critical, null, activityId, null, format, args);
        }

        /// <summary>
        /// The critical.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Critical(int eventId, Exception exception, string format, params object[] args)
        {
            Log(EventType.Critical, eventId, null, exception, format, args);
        }

        /// <summary>
        /// The critical.
        /// </summary>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Critical(int eventId, Guid activityId, Exception exception, string format, params object[] args)
        {
            Log(EventType.Critical, eventId, activityId, exception, format, args);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="eventId">
        /// The event id.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void Log(EventType eventType, int? eventId, Guid? activityId, Exception exception, string format, params object[] args)
        {
            if (AddEntry(eventType) == true)
            {
                var message = new StringBuilder();
                if (!string.IsNullOrEmpty(format))
                {
                    if (args == null || args.Length == 0)
                    {
                        message.Append(format + ";");
                    }
                    else
                    {
                        try
                        {
                            string formatedMsg = string.Format(format, args);
                            message.Append(formatedMsg + ";");
                        }
                        catch (Exception)
                        {
                            message.Append(format + string.Format(" [Couldn't format your message with the given args. Args Count: {0}];", args.Length));
                        }
                    }
                }

                // Determine EventLogEntryType from EventType.
                EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
                switch (eventType)
                {
                    case EventType.Verbose:
                    case EventType.Information:
                        eventLogEntryType = EventLogEntryType.Information;
                        break;
                    case EventType.Warning:
                        eventLogEntryType = EventLogEntryType.Warning;
                        break;
                    case EventType.Error:
                    case EventType.Critical:
                        eventLogEntryType = EventLogEntryType.Error;
                        break;
                }

                var entryMessage = FormatEntryMessage(message.ToString(), activityId, exception);

                // Windows event log can't be longer than 32766 characters.  But it errors for most numbers larger than 31877
                if (entryMessage.Length > 31000)
                {
                    entryMessage = entryMessage.Substring(0, 31000);
                }


                var eventIdValue = 0;

                if (eventId.HasValue)
                {
                    eventIdValue = eventId.Value;
                }
                else if (DefaultEventId.HasValue)
                {
                    eventIdValue = DefaultEventId.Value;
                }

                if (eventIdValue > 0)
                {
                    eventLog.WriteEntry(entryMessage, eventLogEntryType, eventIdValue);
                }
                else
                {
                    eventLog.WriteEntry(entryMessage, eventLogEntryType);
                }
            }
        }

        /// <summary>
        /// Formats the log entry message from provided information.
        /// </summary>
        /// <param name="message">
        /// The message to format.
        /// </param>
        /// <param name="activityId">
        /// The activity id.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the message (optional).
        /// </param>
        /// <returns>
        /// The formatted message.
        /// </returns>
        private string FormatEntryMessage(string message, Guid? activityId, Exception exception = null)
        {
            // Prepend request ID.
            var stringBuilder = new StringBuilder();
            if (activityId.HasValue)
            {
                stringBuilder.Append("Activity ID: ");
                stringBuilder.Append(activityId);
                stringBuilder.Append("\r\n\r\n");
            }

            stringBuilder.Append(message);

            // Add exception if specified.
            if (exception != null)
            {
                stringBuilder.Append("\r\n\r\n");
                stringBuilder.Append("Exception:\r\n\r\n");
                stringBuilder.Append(exception);
            }

            return stringBuilder.ToString();
        }
        
        /// <summary>
        /// Determines whether or not a log entry of the specified type will be entered based on the current verbosity level.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <returns>
        /// * True if the log entry will be entered.
        /// * Else returns false.
        /// </returns>
        private bool AddEntry(EventType eventType)
        {
            bool result = false;

            switch (verbosity)
            {
                case SourceLevels.All:
                case SourceLevels.Verbose:
                    result = true;
                    break;
                case SourceLevels.Information:
                    result = eventType <= EventType.Information;
                    break;
                case SourceLevels.Warning:
                    result = eventType <= EventType.Warning;
                    break;
                case SourceLevels.Error:
                    result = eventType <= EventType.Error;
                    break;
                case SourceLevels.Critical:
                    result = eventType <= EventType.Critical;
                    break;
            }

            return result;
        }

        #endregion
    }
}