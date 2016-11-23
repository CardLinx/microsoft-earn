//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Definiton of an item to be logged by the business analytics pipeline.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace AnalyticsClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    /// <summary>
    /// Definiton of an item to be logged by the business analytics pipeline.
    /// </summary>
    public class AzureTableAnalyticsItem : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the AzureTableAnalyticsItem class
        /// </summary>
        public AzureTableAnalyticsItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AzureTableAnalyticsItem class
        /// </summary>
        /// <param name="partitionKey">
        /// the partion key
        /// </param>
        /// <param name="rowKey">
        /// the row key
        /// </param>
        public AzureTableAnalyticsItem(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Date
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the action
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the client Id
        /// </summary>
        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the referrer client id.
        /// </summary>
        [JsonProperty(PropertyName = "referrer_client_id")]
        public string ReferrerClientId { get; set; }

        /// <summary>
        /// Gets or sets the client app type
        /// </summary>
        [JsonProperty(PropertyName = "cliet_app_type")]
        public string ClientAppType { get; set; }

        /// <summary>
        /// Gets or sets the flights information
        /// </summary>
        [JsonProperty(PropertyName = "flights")] 
        public string Flights { get; set; }

        /// <summary>
        /// Gets or sets the session id
        /// </summary>
        [JsonProperty(PropertyName = "session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the guid
        /// </summary>
        [JsonProperty(PropertyName = "tracking_id")]
        public Guid TrackingId { get; set; }

        /// <summary>
        /// Gets or sets the correlation id
        /// </summary>
        [JsonProperty(PropertyName = "correlation_id")]
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the guid
        /// </summary>
        [JsonProperty(PropertyName = "deal_id")]
        public Guid DealId { get; set; }
              
        /// <summary>
        /// Gets or sets the primery UserId
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalUserId
        /// </summary>
        [JsonProperty(PropertyName = "ad_user_ids")]
        public string AdditionalUserIds { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds spent on this call
        /// </summary>
        [JsonProperty(PropertyName = "milliseconds")]
        public long Milliseconds { get; set; }

        /// <summary>
        /// Gets or sets the data center name
        /// </summary>
        [JsonProperty(PropertyName = "data_center")]
        public string DataCenter { get; set; }

        /// <summary>
        /// Open bucket to store any other data
        /// </summary>
        [JsonProperty(PropertyName = "payload", NullValueHandling = NullValueHandling.Ignore)]
        public string Payload { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Copies the item to this instance
        /// </summary>
        /// <param name="item">
        /// analytics item
        /// </param>
        public void ReadFrom(AnalyticsItem item)
        {
            string flightsStr = string.Empty;
            string additionalUserIdsStr = string.Empty;
            if (item.Flights != null)
            {
                HashSet<string> uniqueFlights = new HashSet<string>(item.Flights);
                var orderedFlights = uniqueFlights.OrderBy(elem => elem); // Make flights list deterministic
                flightsStr = string.Join(",", orderedFlights);
            }
            if (item.AdditionalUserIds != null)
            {
                HashSet<string> uniqueUserIds = new HashSet<string>(item.AdditionalUserIds);
                var orderedUserIds = uniqueUserIds.OrderBy(elem => elem); // Make additional user ids list deterministic
                additionalUserIdsStr = string.Join(",", orderedUserIds);
            }


            this.Action = item.Action;
            this.Date = item.Date;
            this.DealId = item.DealId;
            this.UserId = item.UserId;
            this.AdditionalUserIds = additionalUserIdsStr;
            this.SessionId = item.SessionId;
            this.TrackingId = item.EventId;
            this.CorrelationId = item.ParentEventId;
            this.ClientId = item.ClientId;
            this.ReferrerClientId = item.ReferrerClientId;
            this.ClientAppType = item.ClientAppType;
            this.Flights = flightsStr;
            this.Milliseconds = item.Milliseconds;
            this.DataCenter = item.DataCenter;
            if (item.JPayload != null)
            {
                this.Payload = JsonConvert.SerializeObject(item.JPayload);
            }
            else
            {
                //Backward compatibility
                this.Payload = item.Payload;
            }
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="AzureTableAnalyticsItem"/>.
        /// </returns>
        public AzureTableAnalyticsItem Clone()
        {
            return new AzureTableAnalyticsItem
                       {
                           Action = this.Action,
                           Date = this.Date,
                           DealId = this.DealId,
                           UserId = this.UserId,
                           AdditionalUserIds = this.AdditionalUserIds,
                           SessionId = this.SessionId,
                           TrackingId = this.TrackingId,
                           CorrelationId = this.CorrelationId,
                           ClientId = this.ClientId,
                           ReferrerClientId = this.ReferrerClientId,
                           ClientAppType = this.ClientAppType,
                           Flights = this.Flights,
                           Milliseconds = this.Milliseconds,
                           Payload = this.Payload,
                           DataCenter = this.DataCenter
                       };
        }

        #endregion
    }
}