//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Threading.Tasks;

namespace OfferManagement.JobProcessor
{
    public interface IScheduledJob
    {
        Task ExecuteAsync(ScheduledJobInfo scheduledJobInfo);
    }
}