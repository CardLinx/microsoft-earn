//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earn.Models
{
    public class AnalyticsModel
    {
        /// <summary>
        /// Gets or sets the authenticated user id.
        /// </summary>
        [JsonProperty(PropertyName = "auid")]
        public string AuthenticatedUserId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unique browser Id.
        /// </summary>
        [JsonProperty(PropertyName = "bid")]
        public Guid BrowserId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unique session Id.
        /// </summary>
        [JsonProperty(PropertyName = "sid")]
        public Guid SessionId
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the client IP address.
        /// </summary>
        [JsonProperty(PropertyName = "ipa")]
        public string IPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        [JsonProperty(PropertyName = "ua")]
        public string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        [JsonProperty(PropertyName = "dtype")]
        public string DeviceType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the visted page's url.
        /// </summary>
        [JsonProperty(PropertyName = "purl")]
        public string PageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the visted page's title.
        /// </summary>
        [JsonProperty(PropertyName = "ptitle")]
        public string PageTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the server time stamp.
        /// </summary>
        [JsonProperty(PropertyName = "st")]
        public DateTimeOffset ServerTimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the evenmt name.
        /// </summary>
        [JsonProperty(PropertyName = "eid")]
        public string EventId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets additional metadata about the event.
        /// </summary>
        [JsonProperty(PropertyName = "einfo")]
        public string EventInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        [JsonProperty(PropertyName = "etype")]
        public string EventType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the referrer.
        /// </summary>
        [JsonProperty(PropertyName = "cmp_ref")]
        public string CampaignReferrer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the referrer.
        /// </summary>
        [JsonProperty(PropertyName = "cmp_source")]
        public string CampaignSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the referrer.
        /// </summary>
        [JsonProperty(PropertyName = "cmp_name")]
        public string CampaignName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flight id of the client.
        /// </summary>
        [JsonProperty(PropertyName = "flid")]
        public string FlightId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is a new or a re-visiting user.
        /// </summary>
        [JsonProperty(PropertyName = "newUser")]
        public bool NewUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is authenticated.
        /// </summary>
        [JsonProperty(PropertyName = "ia")]
        public bool IsAuthenticated
        {
            get;
            set;
        }
    }
}