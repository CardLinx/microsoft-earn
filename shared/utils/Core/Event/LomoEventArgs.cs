//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Lomo.Core.Event
{
    public class LomoEventArgs : EventArgs
    {
        public string Message
        {
            get; 
            private set;
        }

        public IEventContext EventContext
        {
            get; 
            private set;
        }

        public LomoEventArgs(string message, IEventContext eventContext)
        {
            this.Message = message;
            this.EventContext = eventContext;
        }

    }
}