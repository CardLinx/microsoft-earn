//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    using System;

    /// <summary>
    /// Represents the current state of a job.
    /// </summary>
    [Flags]
    public enum ScheduledJobState
    {
        /// <summary>
        /// Unspecified Job State
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Job is Running
        /// </summary>
        Running = 1,

        /// <summary>
        /// Job is Paused
        /// </summary>
        Paused = 2,

        /// <summary>
        /// Job is Cancelled
        /// </summary>
        Canceled = 4,

        /// <summary>
        /// Job is Complete
        /// </summary>
        Completed = 8,

        /// <summary>
        /// All relevant job states
        /// </summary>
        All = Running | Paused | Canceled | Completed
    }
}