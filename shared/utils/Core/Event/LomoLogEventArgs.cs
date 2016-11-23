//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Event
{
    using System.Diagnostics;

    public class LomoLogEventArgs : LomoEventArgs
    {
        public TraceEventType EventType
        {
            get; private set;
        }
        public LomoLogEventArgs(TraceEventType eventType, string message, IEventContext eventContext)
            : base(message, eventContext)
        {
            this.EventType = eventType;
        }
    }
}