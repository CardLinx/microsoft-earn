//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Logging
{
    using System;
    using System.Diagnostics;

    public sealed class EventLogLogger : ILog
    {
        private readonly EventLog _eventLog = new EventLog();
        private readonly int _eventID;

        public EventLogLogger(string source, int eventID)
        {
            _eventLog.Source = source;
            _eventID = eventID;
        }

        public void Information(string message)
        {
            _eventLog.WriteEntry(Formatter.Format(message), EventLogEntryType.Information, _eventID);
        }

        public void Warning(string message)
        {
            _eventLog.WriteEntry(Formatter.Format(message), EventLogEntryType.Warning, _eventID);
        }

        public void Error(string message)
        {
            _eventLog.WriteEntry(Formatter.Format(message), EventLogEntryType.Error, _eventID);
        }

        public void Error(Exception e)
        {
            _eventLog.WriteEntry(Formatter.Format(e), EventLogEntryType.Error, _eventID);
	        e = e.InnerException;
			if (e != null)
				_eventLog.WriteEntry(Formatter.Format(e), EventLogEntryType.Error, _eventID);
        }
    }
}