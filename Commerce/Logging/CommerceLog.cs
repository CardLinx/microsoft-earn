//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using System;
    using System.Diagnostics;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Logging;

    public interface ICommerceLog
    {
        /// <summary>
        /// Adds an exhaustive detail entry to the log.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Exhaustive(string format,
            params object[] args);

        /// <summary>
        /// Adds a verbose entry to the log.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Verbose(string format,
            params object[] args);

        /// <summary>
        /// Adds an information entry to the log.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Information(string format,
            params object[] args);

        /// <summary>
        /// Adds a warning entry to the log, associated with the specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Warning(string format,
            params object[] args);

        /// <summary>
        /// Adds a warning entry to the log, associated with the specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="eventId">
        /// The event ID to associate with the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Warning(string format,
            int eventId = (int) DefaultLogEntryEventId.Warning,
            params object[] args);

        /// <summary>
        /// Adds an error entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Error(string format,
            Exception exception = null,
            params object[] args);

        /// <summary>
        /// Adds an error entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="eventId">
        /// The event ID to associate with the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Error(string format,
            Exception exception = null,
            int eventId = (int) DefaultLogEntryEventId.Error,
            params object[] args);

        /// <summary>
        /// Adds a critical entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Critical(string format,
            Exception exception = null,
            params object[] args);

        /// <summary>
        /// Adds a critical entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="eventId">
        /// The event ID to associate with the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        void Critical(string format,
            Exception exception = null,
            int eventId = (int) DefaultLogEntryEventId.Critical,
            params object[] args);

        /// <summary>
        /// Adds a call completion entry to the log.
        /// </summary>
        /// <param name="callName">
        /// The name of the call whose completion to log.
        /// </param>
        /// <param name="callCompletionStatus">
        /// The call status at the completion of the call.
        /// </param>
        /// <param name="performanceInformation">
        /// The object through which analytics information can be added and obtained.
        /// </param>
        /// <param name="eventId">
        /// The event ID to use when logging call completion. Default value is DefaultLogEntryEventId.CallCompletion.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter analyticsInformation cannot be null.
        /// </exception>
        /// <remarks>
        /// This should eventually be replaced with performance counters.
        /// </remarks>
        void CallCompletion(string callName,
            CallCompletionStatus callCompletionStatus,
            PerformanceInformation performanceInformation,
            DefaultLogEntryEventId eventId = DefaultLogEntryEventId.CallCompletion);

        /// <summary>
        /// Adds a call completion entry to the log if exhaustive detail entries are added.
        /// </summary>
        /// <param name="callName">
        /// The name of the call whose completion to log.
        /// </param>
        /// <param name="callCompletionStatus">
        /// The call status at the completion of the call.
        /// </param>
        /// <param name="analyticsInformation">
        /// The object through which analytics information can be added and obtained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter analyticsInformation cannot be null.
        /// </exception>
        /// <remarks>
        /// This should eventually be replaced with performance counters.
        /// </remarks>
        void ExhaustiveCallCompletion(string callName,
            CallCompletionStatus callCompletionStatus,
            PerformanceInformation analyticsInformation);

        /// <summary>
        /// Gets the logging ID of the current activity.
        /// </summary>
        Guid ActivityId { get; }

        /// <summary>
        /// Gets or sets a value indicating whether all logging will be surpressed for this API call unless the active logging
        /// level is SourceLevels.All.
        /// </summary>
        bool OnlyLogIfVerbosityIsAll { get; set; }
    }

    /// <summary>
    /// Provides logging functionality.
    /// </summary>
    public class CommerceLog : ICommerceLog
    {
        /// <summary>
        /// Initializes a new instance of the CommerceLog class.
        /// </summary>
        /// <param name="activityId">
        /// The activity ID to place within log entries.
        /// </param>
        /// <param name="logVerbosity">
        /// The verbosity level to use when creating log entries.
        /// </param>
        /// <param name="source">
        /// The source under which to log events.
        /// </param>
        /// <remarks>
        /// This Application event log source must be added by an account with administrative permissions as the following
        /// registry entry:
        ///   Key: HKLM\SYSTEM\CurrentControlSet\Services\Eventlog\Application\LomoCommerce
        ///   REG_EXPAND_SZ Entry / Value: EventMessageFile / %Windir%\Microsoft.NET\Framework64\v4.0.30319\EventLogMessages.dll
        /// </remarks>
        public CommerceLog(Guid activityId,
                           SourceLevels logVerbosity,
                           string source)
        {
            ActivityId = activityId;
            LogVerbosity = logVerbosity;

            // Default to the EventLog if nothing has already initialized Log.Instance. This should only happen for offline tests.
            if (LogInstanceSet == false)
            {
                Log.Instance = new EventLogLog(LogVerbosity)
                {
                    Source = source
                };
                LogInstanceSet = true;
            }
        }

        /// <summary>
        /// Adds an exhaustive detail entry to the log.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Exhaustive(string format,
                               params object[] args)
        {
            // Generally, exhausive detail entries will not be included in the log.
            if (IsLoggingDisabled() == false && LogVerbosity == SourceLevels.All)
            {
                Log.Verbose(ActivityId, format, args);
            }
        }

        /// <summary>
        /// Adds a verbose entry to the log.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Verbose(string format,
                            params object[] args)
        {
            if (IsLoggingDisabled() == false)
            {
                Log.Verbose(ActivityId, format, args);
            }
        }

        /// <summary>
        /// Adds an information entry to the log.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Information(string format,
                                params object[] args)
        {
            if (IsLoggingDisabled() == false)
            {
                Log.Info((int)DefaultLogEntryEventId.Information, ActivityId, format, args);
            }
        }

        /// <summary>
        /// Adds a warning entry to the log, associated with the specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Warning(string format,
                            params object[] args)
        {
            Warning(format, (int) DefaultLogEntryEventId.Warning, args);
        }

        /// <summary>
        /// Adds a warning entry to the log, associated with the specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="eventId">
        /// The event ID to associate with the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Warning(string format,
                            int eventId = (int) DefaultLogEntryEventId.Warning,
                            params object[] args)
        {
            if (IsLoggingDisabled() == false)
            {
                //Log all warnings between 301 & 399 to the log table.
                if (eventId > 300 && eventId < 400)
                {
                    ResultCode resultCode = (ResultCode)Enum.Parse(typeof(ResultCode), eventId.ToString());
                    DashboardLogTable.Add(ActivityId, resultCode, string.Format(format, args), Config);
                }
                Log.Warn(eventId, ActivityId, format, args);
            }
        }

        /// <summary>
        /// Adds an error entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Error(string format,
                          Exception exception = null,
                          params object[] args)
        {
            Error(format, exception, (int) DefaultLogEntryEventId.Error, args);
        }

        /// <summary>
        /// Adds an error entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="eventId">
        /// The event ID to associate with the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Error(string format,
                          Exception exception = null,
                          int eventId = (int) DefaultLogEntryEventId.Error,
                          params object[] args)
        {
            if (IsLoggingDisabled() == false)
            {
                Log.Error(eventId, ActivityId, exception, format, args);
            }
        }

        /// <summary>
        /// Adds a critical entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Critical(string format,
                             Exception exception = null,
                             params object[] args)
        {
            Critical(format, exception, (int) DefaultLogEntryEventId.Critical, args);
        }

        /// <summary>
        /// Adds a critical entry to the log, including the specified exception information, if any, and associated with the
        /// specified event ID, if any.
        /// </summary>
        /// <param name="format">
        /// The format template string or full message to add to the log.
        /// </param>
        /// <param name="exception">
        /// The exception to add to the log entry (optional).
        /// </param>
        /// <param name="eventId">
        /// The event ID to associate with the log entry (optional).
        /// </param>
        /// <param name="args">
        /// Arguments to place within format string, if any.
        /// </param>
        public void Critical(string format,
                             Exception exception = null,
                             int eventId = (int) DefaultLogEntryEventId.Critical,
                             params object[] args)
        {
            if (IsLoggingDisabled() == false)
            {
                Log.Critical(eventId, ActivityId, exception, format, args);
            }
        }

        /// <summary>
        /// Adds a call completion entry to the log.
        /// </summary>
        /// <param name="callName">
        /// The name of the call whose completion to log.
        /// </param>
        /// <param name="callCompletionStatus">
        /// The call status at the completion of the call.
        /// </param>
        /// <param name="performanceInformation">
        /// The object through which analytics information can be added and obtained.
        /// </param>
        /// <param name="eventId">
        /// The event ID to use when logging call completion. Default value is DefaultLogEntryEventId.CallCompletion.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter analyticsInformation cannot be null.
        /// </exception>
        /// <remarks>
        /// This should eventually be replaced with performance counters.
        /// </remarks>
        public void CallCompletion(string callName,
                                   CallCompletionStatus callCompletionStatus,
                                   PerformanceInformation performanceInformation,
                                   DefaultLogEntryEventId eventId = DefaultLogEntryEventId.CallCompletion)
        {
            if (performanceInformation == null)
            {
                throw new ArgumentNullException("performanceInformation", "Parameter performanceInformation cannot be null.");
            }

            if (IsLoggingDisabled() == false)
            {
                Log.Info((int)eventId, ActivityId,
                         "Call to {0} completed with result {1} and these performance measures:\r\n {2}.", callName, callCompletionStatus,
                         performanceInformation.Collate());
            }
        }

        /// <summary>
        /// Adds a call completion entry to the log if exhaustive detail entries are added.
        /// </summary>
        /// <param name="callName">
        /// The name of the call whose completion to log.
        /// </param>
        /// <param name="callCompletionStatus">
        /// The call status at the completion of the call.
        /// </param>
        /// <param name="analyticsInformation">
        /// The object through which analytics information can be added and obtained.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter analyticsInformation cannot be null.
        /// </exception>
        /// <remarks>
        /// This should eventually be replaced with performance counters.
        /// </remarks>
        public void ExhaustiveCallCompletion(string callName,
                                             CallCompletionStatus callCompletionStatus,
                                             PerformanceInformation analyticsInformation)
        {
            if (analyticsInformation == null)
            {
                throw new ArgumentNullException("analyticsInformation", "Parameter analyticsInformation cannot be null.");
            }

            // Generally, exhausive detail entries will not be included in the log.
            if (LogVerbosity == SourceLevels.All)
            {
                CallCompletion(callName, callCompletionStatus, analyticsInformation, DefaultLogEntryEventId.Verbose);
            }
        }

        /// <summary>
        /// Gets the logging ID of the current activity.
        /// </summary>
        public Guid ActivityId { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether all logging will be surpressed for this API call unless the active logging
        /// level is SourceLevels.All.
        /// </summary>
        public bool OnlyLogIfVerbosityIsAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Log.Instance has been set.
        /// </summary>
        public static bool LogInstanceSet { get; set; }
        
        /// <summary>
        /// Gets or sets the value of commerce config for this log is being created.
        /// </summary>
        public static CommerceConfig Config { get; set; }

        /// <summary>
        /// Determines whether or not logging is currently disabled for this API call.
        /// </summary>
        /// <param name="logVerbosity">
        /// Specifies the current log verbosity.
        /// </param>
        /// <returns>
        /// * True if logging is disabled.
        /// * Else returns false.
        /// </returns>
        internal bool IsLoggingDisabled(SourceLevels logVerbosity)
        {
            bool result = false;

            if (OnlyLogIfVerbosityIsAll == true && logVerbosity != SourceLevels.All)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Determines whether or not logging is currently disabled for this API call.
        /// </summary>
        /// <param name="logVerbosity">
        /// Specifies the current log verbosity.
        /// </param>
        /// <returns>
        /// * True if logging is disabled.
        /// * Else returns false.
        /// </returns>
        private bool IsLoggingDisabled()
        {
            return IsLoggingDisabled(LogVerbosity);
        }

        /// <summary>
        /// Gets or sets the verbosity level to use when creating log entries.
        /// </summary>
        private SourceLevels LogVerbosity { get; set; }
        
    }
}