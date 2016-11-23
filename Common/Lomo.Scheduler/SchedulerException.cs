//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;

    /// <summary>
    /// Scheduler related Exception
    /// </summary>
    class SchedulerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public SchedulerException(string message)
            : base(message)
        {
        }
    }
}