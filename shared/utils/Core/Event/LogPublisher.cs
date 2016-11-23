//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Lomo.Core.Event
{
    using System.Diagnostics;

    public abstract class LogPublisher
    {
        public event EventHandler<LomoLogEventArgs> LogMethodEvent;

        protected virtual void OnRaiseLogMethodEvent(TraceEventType eventType, string message, IEventContext eventContext)
        {
            var handler = LogMethodEvent;
            if (handler != null)
            {
                var e = new LomoLogEventArgs(eventType, message, eventContext);
                handler(this, e);
            }
        }
    }
}