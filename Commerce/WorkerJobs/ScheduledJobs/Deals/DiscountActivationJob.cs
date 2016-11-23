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
    /// DiscountActivationJob Job
    /// This tells the deal server that discount is ready 
    /// </summary>
    public class DiscountActivationJob : IScheduledJob
    {
        public Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            return Task.Factory.StartNew(() => { });
        }
    }
}