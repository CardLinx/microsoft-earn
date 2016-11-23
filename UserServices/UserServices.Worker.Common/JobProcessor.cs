//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The job processor abstract class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.Worker.Common
{
    using System;
    using System.Threading;
    using Lomo.Logging;

    /// <summary>
    /// The job processor abstract class
    /// </summary>
    abstract public class JobProcessor : IJobProcessor
    {
        /// <summary>
        /// The sleep time when the jobs queue empty.
        /// </summary>
        protected static readonly TimeSpan SleepTimeWhenQueueEmpty = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The sleep time between errors.
        /// </summary>
        protected readonly TimeSpan SleepTimeAfterErrors = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Job processor abstract worker method. Implemented by individual job processors
        /// </summary>
        /// <param name="ct">
        /// The cancellation token. If ct is null the work will continue for ever otherwise it will continue until a cancel request
        /// </param>
        public abstract void DoWork(CancellationToken? ct);

        /// <summary>
        /// Handle Agent Job Processing/ Fetching Error
        /// </summary>
        /// <param name="eventCode"> The event code. </param>
        /// <param name="exception">
        /// The exception. </param>
        /// <param name="errorMessagePrefix"> The error message prefix. </param>
        /// <param name="agentId"></param>
        /// <param name="emailCargo"> The email job. </param>
        protected void HandleError(int eventCode, Exception exception, string errorMessagePrefix, string agentId, object emailCargo)
        {
            Log.Error(eventCode, exception, this.GetErrorMessage(errorMessagePrefix, agentId, emailCargo));
            Log.Verbose("Agent Id: {0} sleeping after error for {1} seconds", agentId, this.SleepTimeAfterErrors.TotalSeconds);
            Thread.Sleep(this.SleepTimeAfterErrors);
        }

        /// <summary>
        /// The get error message.
        /// </summary>
        /// <param name="prefix"> The message prefix.
        /// </param>
        /// <param name="agentId"></param>
        /// <param name="emailCargo"> The email job. </param>
        /// <returns>The error message. </returns>
        protected string GetErrorMessage(string prefix, string agentId, object emailCargo)
        {
            return string.Format(
                "{0}. Agent Id: {1}; Job Details=[{2}]",
                prefix,
                agentId,
                emailCargo);
        }
    }
}