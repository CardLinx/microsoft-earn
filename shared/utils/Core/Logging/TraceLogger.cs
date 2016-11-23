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

namespace Lomo.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public sealed class TraceLogger : ILog
    {
        #region Private Members

        /// <summary>
        /// The trace source.
        /// </summary>
        private readonly TraceSource traceSource;

        /// <summary>
        /// The event Id
        /// </summary>
        private readonly int eventId;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogger"/> class.
        /// </summary>
        /// <param name="sourceLevels">
        /// Source level to filter by. the default is no filter by source level
        /// </param>
        /// <param name="eventId">Event Id</param>
        /// <param name="source">Source Name</param>
        public TraceLogger(SourceLevels sourceLevels = SourceLevels.All, int eventId = 100, string source = "LoMoTraceSource")
            : this(new List<TraceListener>(), sourceLevels, eventId, source)
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
        /// <param name="eventId">Event Id</param>
        /// <param name="source">Source Name</param>
        public TraceLogger(IEnumerable<TraceListener> traceListeners, SourceLevels sourceLevels = SourceLevels.All, int eventId = 100, string source = "LoMoTraceSource")
        {
            this.eventId = eventId;
            traceSource = new TraceSource(source, sourceLevels);
            foreach (var traceListener in traceListeners)
            {
                traceSource.Listeners.Add(traceListener);
            }
        }

        #endregion Constructors

        #region ILog Iplementation

        public void Information(string message)
        {
            Log(TraceEventType.Information, null, message);
        }

        public void Warning(string message)
        {
            Log(TraceEventType.Warning, null, message);
        }

        public void Error(string message)
        {
            Log(TraceEventType.Error, null, message);
        }

        public void Error(Exception e)
        {
            Log(TraceEventType.Error, e, null);
	        e = e.InnerException;
            if (e != null)
            {
                Log(TraceEventType.Error, e, null);
            }
        }

        #endregion ILog Iplementation

        #region Private Methods

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
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
        private void Log(TraceEventType eventType, Exception exception, string format)
        {
            var msg = new StringBuilder();
            if (!string.IsNullOrEmpty(format))
            {
                msg.Append(format + ";");
            }

            if (exception != null)
            {
                msg.Append(string.Format("Error: {0};", Formatter.Format(exception)));
            }

            string logDatetime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            msg.AppendFormat("Datetime={0};", logDatetime);

            this.traceSource.TraceEvent(eventType, this.eventId, msg.ToString());
        }

        #endregion Private Methods
    }
}