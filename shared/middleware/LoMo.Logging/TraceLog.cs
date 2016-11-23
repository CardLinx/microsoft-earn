//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The trace log.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/**
 * You can add this to your configuration file
 <configuration> 
    <system.diagnostics> 
        <sources> 
            <source name="LoMoTraceSource" switchName="SourceSwitch" switchType="System.Diagnostics.SourceSwitch">
                <listeners> 
                    <add name="console" type="System.Diagnostics.ConsoleTraceListener" initializeData="false" />
                    <remove name="Default" />
                </listeners> 
            </source> 
        </sources> 
        <switches> 
            <!-- You can set the level at which tracing is to occur --> 
            <add name="SourceSwitch" value="Verbose" />
            <!-- You can turn tracing off --> 
            <!--add name="SourceSwitch" value="Off" -->
        </switches> 
        <trace autoflush="true" indentsize="4"></trace>
    </system.diagnostics> 
</configuration>
**/
namespace Lomo.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// The trace log.
    /// </summary>
    public class TraceLog : ILog
    {
        #region members

        /// <summary>
        /// The trace source.
        /// </summary>
        private readonly TraceSource traceSource;

        /// <summary>
        /// flag for local logging
        /// </summary>
        private bool localLogging = false;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="sourceLevels">
        /// Source level to filter by. the default is no filter by source level
        /// </param>
        public TraceLog(SourceLevels sourceLevels = SourceLevels.All)
            : this(new List<TraceListener>(), sourceLevels)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLog"/> class.
        /// </summary>
        /// <param name="traceListeners">
        /// The trace listeners.
        /// </param>
        /// <param name="sourceLevels">
        /// Source level to filter by. the default is no filter by source level
        /// </param>
        /// <param name="localLog">
        /// flag to check if local logging is enabled
        /// </param>
        public TraceLog(IEnumerable<TraceListener> traceListeners, SourceLevels sourceLevels = SourceLevels.All, bool localLog = false)
        {
            traceSource = new TraceSource("LoMoTraceSource", sourceLevels);
            foreach (var traceListener in traceListeners)
            {
                traceSource.Listeners.Add(traceListener);
            }

            localLogging = localLog;
        }

        #endregion Constructors

        #region ILog imp

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { private get; set; }

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
            Log(TraceEventType.Verbose, null, null, null, format, args);
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
            Log(TraceEventType.Verbose, null, activityId, null, format, args);
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
            Log(TraceEventType.Information, null, null, null, format, args);
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
            Log(TraceEventType.Information, null, activityId, null, format, args);
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
            Log(TraceEventType.Information, eventId, null, null, format, args);
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
            Log(TraceEventType.Information, eventId, activityId, null, format, args);
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
            Log(TraceEventType.Warning, null, null, null, format, args);
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
            Log(TraceEventType.Warning, null, activityId, null, format, args);
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
            Log(TraceEventType.Warning, eventId, null, null, format, args);
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
            Log(TraceEventType.Warning, eventId, activityId, null, format, args);
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
            Log(TraceEventType.Error, null, null, null, format, args);
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
            Log(TraceEventType.Error, null, activityId, null, format, args);
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
            Log(TraceEventType.Error, eventId, null, null, format, args);
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
            Log(TraceEventType.Error, eventId, activityId, null, format, args);
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
            Log(TraceEventType.Error, null, null, exception, format, args);
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
            Log(TraceEventType.Error, null, activityId, exception, format, args);
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
            Log(TraceEventType.Error, eventId, null, exception, format, args);
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
            Log(TraceEventType.Error, eventId, activityId, exception, format, args);
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
            Log(TraceEventType.Critical, eventId, null, null, format, args);
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
            Log(TraceEventType.Critical, eventId, activityId, null, format, args);
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
            Log(TraceEventType.Critical, eventId, null, exception, format, args);
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
            Log(TraceEventType.Critical, eventId, activityId, exception, format, args);
        }

        #endregion

        #region private members

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
        private void Log(TraceEventType eventType, int? eventId, Guid? activityId, Exception exception, string format, params object[] args)
        {
            if (eventId == null)
            {
                eventId = string.IsNullOrEmpty(format) ? 0 : Math.Abs(format.GetHashCode());
            }

            var msg = new StringBuilder();
            if (activityId.HasValue)
            {
                msg.Append(string.Format("Activity Id: {0};", activityId));
            }

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
                        msg.Append(format + string.Format(" [Couldn't format your message with the given args. Args Count: {0}];", args.Length));
                    }
                }
            }

            if (exception != null)
            {
                msg.Append(string.Format("Error: {0};", new ExceptionFormatter().GetMessage(exception)));
            }

            string logDatetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            msg.AppendFormat("Datetime={0};", logDatetime);

            this.traceSource.TraceEvent(eventType, eventId.Value, msg.ToString());

            if (localLogging)
            {
                this.traceSource.TraceData(eventType, eventId.Value, msg.ToString());
            }
        }

        #endregion
    }
}