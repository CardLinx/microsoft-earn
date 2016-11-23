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
    /// Job to run via Scheduler.
    /// </summary>
    public interface IScheduledJob
    {
        /// <summary>
        /// Actual execution happens here. Contains all the business logic.
        /// </summary>
        /// <param name="details">
        /// Details of job we are executing.
        /// </param>
        /// <param name="logger">
        /// Commerce Logger
        /// </param>
        Task Execute(ScheduledJobDetails details, CommerceLog logger);
    }
}