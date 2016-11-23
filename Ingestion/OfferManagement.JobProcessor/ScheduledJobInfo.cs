//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;

namespace OfferManagement.JobProcessor
{
    public class ScheduledJobInfo
    {
        public string JobId { get; set; }

        public JobType JobType { get; set; }

        public DateTime JobScheduledTime { get; set; }

        public DateTime JobCompletedTime { get; set; }

        public IDictionary<string, string> JobPayload { get; set; }

        public override string ToString()
        {
            return
                $"JobId : {JobId}, JobType : {JobType}, ScheduledTime : {JobScheduledTime}, CompletedTime : {JobCompletedTime} ";
        }
    }
}