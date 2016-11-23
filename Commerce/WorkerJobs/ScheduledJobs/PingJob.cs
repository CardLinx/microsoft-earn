//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;

    /// <summary>
    /// Ping Job.
    /// </summary>
    public class PingJob : IScheduledJob
    {
        /// <summary>
        /// Ping Job - Just logs the message.
        /// </summary>
        /// <param name="details">
        /// Details of the job we are executing here. 
        /// </param>
        /// <param name="logger">
        /// Handle to the logger
        /// </param>
        public Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            return Task.Factory.StartNew(() => logger.Information("Executing ping job \r\n " +
                                                           "Details {0}", details));
        }
    }
}