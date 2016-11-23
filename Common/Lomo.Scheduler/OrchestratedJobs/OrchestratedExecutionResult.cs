//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    /// <summary>
    /// Represents possible results of orchestrated job and task execution.
    /// </summary>
    public enum OrchestratedExecutionResult
    {
        /// <summary>
        /// Indicates execution completed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates execution completed abnormally and should be retried.
        /// </summary>
        NonTerminalError,

        /// <summary>
        /// Indicates execution completed abnormally, and the job should be aborted.
        /// </summary>
        TerminalError
    }
}