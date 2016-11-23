//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Earn.DataContract.WebAnalytics
{
    public class RequestFilter
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string CampaignId { get; set; }

        public EventFilter EventId { get; set; }
    }
}