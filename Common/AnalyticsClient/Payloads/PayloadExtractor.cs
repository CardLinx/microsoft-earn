//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The query payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalyticsClient.Payloads
{
    using System.IO;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// The payload extractor.
    /// </summary>
    public static class PayloadExtractor
    {
        /// <summary>
        /// Extract query payload from analytics item.
        /// </summary>
        /// <param name="item">the analytics item</param>
        /// <returns>the query payload object or null if not exists</returns>
        public static QueryPayload GetQueryPayload(AzureTableAnalyticsItem item)
        {
            if (!string.IsNullOrEmpty(item.Payload))
            {
                try
                {
                    return JsonConvert.DeserializeObject<QueryPayload>(item.Payload);
                }
                catch
                {
                    //xmlSerializer for backward compatibility
                    // Type is query. Take the deals from the payload
                    var xmlSerializer = new XmlSerializer(typeof(QueryPayload));
                    return (QueryPayload)xmlSerializer.Deserialize(new StringReader(item.Payload));
                }
            }

            return null;
        }

        /// <summary>
        /// The get conversion payload.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="ConversionPayload"/>.
        /// </returns>
        public static ConversionPayload GetConversionPayload(AzureTableAnalyticsItem item)
        {
            if (!string.IsNullOrEmpty(item.Payload))
            {
                return JsonConvert.DeserializeObject<ConversionPayload>(item.Payload);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the deal redemption payload from the specified table item.
        /// </summary>
        /// <param name="item">
        /// The table item from which to retrieve the deal redemption payload.
        /// </param>
        /// <returns>
        /// The <see cref="DealRedemptionPayload"/>.
        /// </returns>
        public static DealRedemptionPayload GetDealRedemptionPayload(AzureTableAnalyticsItem item)
        {
            if (!string.IsNullOrEmpty(item.Payload))
            {
                return JsonConvert.DeserializeObject<DealRedemptionPayload>(item.Payload);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the deal settlement payload from the specified table item.
        /// </summary>
        /// <param name="item">
        /// The table item from which to retrieve the deal settlement payload.
        /// </param>
        /// <returns>
        /// The <see cref="DealSettlementPayload"/>.
        /// </returns>
        public static DealSettlementPayload GetDealSettlementPayload(AzureTableAnalyticsItem item)
        {
            if (!string.IsNullOrEmpty(item.Payload))
            {
                return JsonConvert.DeserializeObject<DealSettlementPayload>(item.Payload);
            }

            return null;
        }
    }
}