//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Represents an analytics item to be added
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnalyticsClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using Lomo.Logging;

    /// <summary>
    /// The analytics item.
    /// </summary>
    public class AnalyticsItem
    {
        #region Constants

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameDate = "t";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameAction = "a";

        /// <summary>
        /// xml client id attribute name
        /// </summary>
        private const string AttributeNameClientId = "c";

        /// <summary>
        /// xml referrer client id attribute name
        /// </summary>
        private const string AttributeNameReferrerClientId = "rc";
        
        /// <summary>
        /// xml client app type attribute name
        /// </summary>
        private const string AttributeNameClientAppType = "cat";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameFlights = "f";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameAdditionalUserIds = "au";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameSessionId = "s";
        
        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameDealId = "d";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameTrackingId = "r";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameCorrelationId = "ci";

        /// <summary>
        /// xml attribute name
        /// </summary>
        private const string AttributeNameUserId = "u";

        /// <summary>
        /// xml attribute name for time in milliseconds
        /// </summary>
        private const string AttributeNameMilliseconds = "ms";

        /// <summary>
        /// xml attribute name for data center
        /// </summary>
        private const string AttributeNameDataCenter = "dc";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the AnalyticsItem class. 
        /// </summary>
        public AnalyticsItem()
        {
            this.Date = DateTime.UtcNow;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Date
        /// </summary>
        [JsonProperty(PropertyName = "t")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the action
        /// </summary>
        [JsonProperty(PropertyName = "a")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the ClientId
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the referrer client id.
        /// </summary>
        [JsonProperty(PropertyName = "rc")]
        public string ReferrerClientId { get; set; }

        /// <summary>
        /// Gets or sets the Client App Id
        /// </summary>
        [JsonProperty(PropertyName = "cat")]
        public string ClientAppType { get; set; }

        /// <summary>
        /// Gets or sets the Flights 
        /// </summary>
        [JsonProperty(PropertyName = "f")]
        public List<string> Flights { get; set; }

        /// <summary>
        /// Gets or sets the session id
        /// </summary>
        [JsonProperty(PropertyName = "s")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the EventId
        /// This is the ID that is used by external partners
        /// to give us reports on the deals purchased via our server.
        /// </summary>
        [JsonProperty(PropertyName = "e_id")]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the ParentEventId
        /// This is the ID that is used internally, to correlate
        /// a deal click back to its query context.
        /// </summary>
        [JsonProperty(PropertyName = "pe_id")]
        public Guid ParentEventId { get; set; }

        /// <summary>
        /// Gets or sets the DealId
        /// </summary>
        [JsonProperty(PropertyName = "d")]
        public Guid DealId { get; set; }

        /// <summary>
        /// Gets or sets the primery UserId
        /// </summary>
        [JsonProperty(PropertyName = "u")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalUserIds
        /// </summary>
        [JsonProperty(PropertyName = "au")]
        public List<string> AdditionalUserIds { get; set; } 

        /// <summary>
        /// Gets or sets the number of milliseconds spent on this call
        /// </summary>
        [JsonProperty(PropertyName = "ms")]
        public long Milliseconds { get; set; }

        /// <summary>
        /// Gets or sets Includes the data center that this entry logged from
        /// </summary>
        [JsonProperty(PropertyName = "dc")]
        public string DataCenter { get; set; }

        /// <summary>
        /// Gets or sets the payload
        /// </summary>
        /// TODO - deprecated, still here for backward compatability
        [JsonProperty(PropertyName = "p", NullValueHandling = NullValueHandling.Ignore)]
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets the jpayload
        /// </summary>
        [JsonProperty(PropertyName = "jp", NullValueHandling = NullValueHandling.Ignore)]
        public JObject JPayload { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an AnalyticsItem from an xml node
        /// </summary>
        /// <param name="node">
        /// xml node
        /// </param>
        /// <returns>
        /// an instance of an AnalyticsItem class
        /// </returns>
        /// TODO - should be deprecated. Keep for backward compatibility for now
        public static AnalyticsItem Parse(XElement node)
        {
            try
            {
                
                var item = new AnalyticsItem
                {
                    Date = node.GetAttributeValue(AttributeNameDate, DateTime.MinValue),
                    Action = node.GetAttributeValue(AttributeNameAction, string.Empty),
                    ClientId = node.GetAttributeValue(AttributeNameClientId, string.Empty),
                    ReferrerClientId = node.GetAttributeValue(AttributeNameReferrerClientId, string.Empty),
                    ClientAppType = node.GetAttributeValue(AttributeNameClientAppType, string.Empty),
                    Flights = Utils.ToList(node.GetAttributeValue(AttributeNameFlights, string.Empty)),
                    SessionId = node.GetAttributeValue(AttributeNameSessionId, string.Empty),
                    EventId = node.GetAttributeValue(AttributeNameTrackingId, Guid.Empty),
                    ParentEventId = node.GetAttributeValue(AttributeNameCorrelationId, Guid.Empty),
                    DealId = node.GetAttributeValue(AttributeNameDealId, Guid.Empty),
                    UserId = node.GetAttributeValue(AttributeNameUserId, string.Empty),
                    AdditionalUserIds = Utils.ToList(node.GetAttributeValue(AttributeNameAdditionalUserIds, string.Empty)),
                    Milliseconds = node.GetAttributeValue(AttributeNameMilliseconds, 0),
                    DataCenter = node.GetAttributeValue(AttributeNameDataCenter, string.Empty)
                };
                if (node.FirstNode != null)
                {
                    item.Payload = node.FirstNode.ToString();
                }

                return item;
            }
            catch
            {
                Log.Error("invalid analytics item found in the queue");
                return null;
            }
        }

        #endregion
    }
}