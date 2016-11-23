//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Azure.Utils
{
    public class SchedulerQueueInfo
    {
        public string AccountName { get; set; }

        public string QueueName { get; set; }

        public string SasToken { get; set; }
    }
}