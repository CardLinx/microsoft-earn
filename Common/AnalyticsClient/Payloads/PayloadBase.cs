//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.CodeDom;
using Newtonsoft.Json;

namespace AnalyticsClient.Payloads
{
    /// <summary>
    /// The payload base.
    /// </summary>
    public class PayloadBase
    {
        public const string ParentEventActionJPropertyName = "parent_action";

        public const string DeviceJProperty = "device";

        public const string FdLocationJProperty = "fd_loc";

        /// <summary>
        /// Gets or sets the parent event action
        /// </summary>
        [JsonProperty(PropertyName = ParentEventActionJPropertyName)]
        public string ParentEventAction { get; set; }

        /// <summary>
        /// Gets or sets the device name
        /// </summary>
        [JsonProperty(PropertyName = DeviceJProperty, NullValueHandling = NullValueHandling.Ignore)]
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the frontdoor location
        /// </summary>
        [JsonProperty(PropertyName = FdLocationJProperty, NullValueHandling = NullValueHandling.Ignore)]
        public FdLocation FdLocation { get; set; }
    
    }
}