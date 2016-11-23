//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the Merchant Email cargo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the merchant email cargo
    /// </summary>
    [DataContract]
    public class MerchantReportEmailCargo : EmailCargo
    {
        /// <summary>
        /// Gets or sets the from date for the report
        /// </summary>
        [JsonProperty(PropertyName = "fromdate")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Gets or sets the to date for the report
        /// </summary>
        [JsonProperty(PropertyName = "todate")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Gets or sets the schedule for receiving merchant email
        /// </summary>
        [JsonProperty(PropertyName = "scheduletype")]
        public string ScheduleType { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Job Id: {0}; User Id: {1}; FromDate: {2}; ToDate : {3}; Schedule: {4}; EmailAddress: {5}; EmailRenderingServiceURL: {6}",
                this.Id, this.UserId, this.FromDate, this.ToDate, this.ScheduleType, this.EmailAddress, this.EmailRenderingServiceAddress);
        }
    }
}