//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The ClientFlightAllocation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Lomo.Logging;

    /// <summary>
    /// The ClientFlightAllocation
    /// </summary>
    public class ClientFlightAllocation
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="ClientFlightAllocation" /> class.
        /// </summary>
        static ClientFlightAllocation()
        {
            List = new Dictionary<string, ClientFlightAllocation>(StringComparer.OrdinalIgnoreCase);
            ClientKeyFlightKeyIndex = new Dictionary<string, Dictionary<byte, string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFlightAllocation" /> class.
        /// </summary>
        /// <param name="client">the client</param>
        /// <param name="flight">the flight</param>
        /// <param name="allocationVersion">the allocation version</param>
        /// <param name="publishingVersion">the publishing version</param>
        /// <param name="trafficAllocationPercent">the trafficAllocationPercent</param>
        /// <param name="startDate">the startDate</param>
        /// <param name="endDate">the endDate</param>
        /// <param name="seedChangeIntervalSeconds">the seedChangeIntervalSeconds</param>
        public ClientFlightAllocation(Client client, Flight flight, int allocationVersion, int publishingVersion, byte trafficAllocationPercent, DateTime? startDate = null, DateTime? endDate = null, int? seedChangeIntervalSeconds = null)
        {
            ClientId = client.Id;
            ClientAppId = client.App;
            FlightId = flight.Id;
            FlightVersion = flight.Version;
            AllocationVersion = allocationVersion;
            PublishingVersion = publishingVersion;
            Key = GetKey(ClientId, ClientAppId, FlightId, FlightVersion, AllocationVersion);
            TrafficAllocationPercent = trafficAllocationPercent;
            StartDate = startDate;
            EndDate = endDate;
            SeedChangeIntervalSeconds = seedChangeIntervalSeconds;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the seed change interval in seconds for allocation set
        /// </summary>
        public static int? AllocationSetSeedChangeIntervalSeconds { get; private set; }

        /// <summary>
        /// Gets or sets client flight allocation Map
        /// </summary>
        public static Dictionary<string, ClientFlightAllocation> List { get; set; }

        /// <summary>
        /// Gets or sets client flight map
        /// </summary>
        public static Dictionary<string, Dictionary<byte, string>> ClientKeyFlightKeyIndex { get; set; }

        /// <summary>
        ///  Gets the client id
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        ///  Gets the client app id
        /// </summary>
        public string ClientAppId { get; private set; }

        /// <summary>
        /// Gets the flight id
        /// </summary>
        public string FlightId { get; private set; }

        /// <summary>
        /// Gets the flight version
        /// </summary>
        public int FlightVersion { get; private set; }

        /// <summary>
        /// Gets the allocation version
        /// </summary>
        public int AllocationVersion { get; private set; }

        /// <summary>
        /// Gets the allocation version
        /// </summary>
        public int PublishingVersion { get; private set; }

        /// <summary>
        /// Gets the Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the TrafficAllocationPercent
        /// </summary>
        public byte TrafficAllocationPercent { get; private set; }

        /// <summary>
        /// Gets the StartDate
        /// </summary>
        public DateTime? StartDate { get; private set; }

        /// <summary>
        /// Gets the EndDate
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Gets the seed change interval in seconds
        /// </summary>
        public int? SeedChangeIntervalSeconds { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets allocation set seed change interval seconds
        /// </summary>
        /// <param name="seedChangeIntervalSeconds">the seed change interval seconds</param>
        public static void SetAllocationSetSeedChangeIntervalSeconds(int? seedChangeIntervalSeconds)
        {
            AllocationSetSeedChangeIntervalSeconds = seedChangeIntervalSeconds;
        }

        /// <summary>
        /// Translates list of client flight allocations to xml
        /// </summary>
        /// <returns>client flight allocations xml</returns>
        public static XElement ListToXml()
        {
            return new XElement("ClientFlightAllocations", List.Select(_ => _.Value.ToXml()));
        }

        /// <summary>
        /// Translates xml to list of client flight allocations
        /// </summary>
        /// <param name="xml">client flight allocations in xml</param>
        public static void XmlToList(XElement xml)
        {
            var dic = xml.Descendants("ClientFlightAllocation")
                         .Select(_ => new { F = FromXml(_) })
                         .ToDictionary(_ => _.F.Key, _ => _.F, StringComparer.OrdinalIgnoreCase);
            if (dic.Any())
            {
                List = dic;
            }
        }

        /// <summary>
        /// Translates list of flight allocations into client-flight index
        /// </summary>
        public static void ListToClientKeyFlightKeyIndex()
        {
            var clientKeyFlightKeyIndex = new Dictionary<string, Dictionary<byte, string>>(StringComparer.OrdinalIgnoreCase);

            var dic = List.Select(_ => _.Value)
                          .GroupBy(_ => Client.GetKey(_.ClientId, _.ClientAppId))
                          .Select(_ => new { _.Key, Dic = _.ToDictionary(a => a.Key, a => a, StringComparer.OrdinalIgnoreCase) })
                          .ToDictionary(_ => _.Key, _ => _.Dic, StringComparer.OrdinalIgnoreCase);
            foreach (var clientId in Client.IdMap.Select(_ => _.Value).Distinct().Union(new[] { Client.IdUnknown }))
            {
                foreach (var clientAppId in Client.AppMap.Select(_ => _.Value).Distinct().Union(new[] { Client.AppUnknown }))
                {
                    Dictionary<string, ClientFlightAllocation> dicInt;
                    var clientKey = Client.GetKey(clientId, clientAppId);
                    dic.TryGetValue(clientKey, out dicInt);
                    if (dicInt == null)
                    {
                        dic.TryGetValue(Client.GetKey(clientId, Client.AppAll), out dicInt);
                    }
                    
                    if (dicInt == null)
                    {
                        dic.TryGetValue(Client.GetKey(Client.IdAll, Client.AppAll), out dicInt);
                    }

                    var allocationsAll = Enumerable.Empty<ClientFlightAllocation>();
                    if (dicInt != null)
                    {
                        allocationsAll = dicInt.Values;
                    }
                            
                    var allocations = allocationsAll.GroupBy(_ => new { FlightKey = Flight.GetKey(_.FlightId, _.FlightVersion), _.FlightId })
                                                    .Select(_ => new { _.Key.FlightKey, _.Key.FlightId, _.First().TrafficAllocationPercent })
                                                    .OrderBy(_ => _.FlightId == Flight.IdDefault)
                                                    .ThenByDescending(_ => _.FlightKey) // favor more recent version 
                                                    .ToList();
                    var bucketFlightMap = new Dictionary<byte, string>();
                    var boundary = (byte)0;
                    foreach (var allocation in allocations)
                    {
                        for (int i = 0; i < allocation.TrafficAllocationPercent; i++)
                        {
                            bucketFlightMap.Add(boundary, allocation.FlightKey);
                            boundary++;
                            if (boundary > 99)
                            {
                                break;
                            }
                        }

                        if (boundary > 99)
                        {
                            break;
                        }
                    }

                    clientKeyFlightKeyIndex.Add(clientKey, bucketFlightMap);
                }
            }

            ClientKeyFlightKeyIndex = clientKeyFlightKeyIndex;
        }

        /// <summary>
        /// Get flight key from client id and user id
        /// </summary>
        /// <param name="clientKey">client key</param>
        /// <param name="userId">user id</param>
        /// <returns>flight key</returns>
        public static string GetFlightKey(string clientKey, string userId)
        {
            var bucket = StringToBucket(userId, AllocationSetSeedChangeIntervalSeconds);
            Dictionary<byte, string> bucketFlightDic;
            string flightKey = null;
            ClientKeyFlightKeyIndex.TryGetValue(clientKey, out bucketFlightDic);
            if (bucketFlightDic != null)
            {
                bucketFlightDic.TryGetValue(bucket, out flightKey);
            }

            if (flightKey == null)
            {
                Log.Warn("ClientFlightAllocation: Flight for {0} client and {1} bucket is not found", clientKey, bucket);
                Log.Warn("ClientFlightAllocation: {0} client {1}found in ClientKeyFlightKeyIndex{2}", clientKey, bucketFlightDic == null ? "not " : string.Empty, bucketFlightDic != null ? " but bucketFlightDic is empty" : string.Empty);
                //// Get default flight key
                var cfa = List.Values.FirstOrDefault(_ => _.ClientId == Client.IdAll && _.ClientAppId == Client.AppAll && _.FlightId == Flight.IdDefault);
                if (cfa != null)
                {
                    flightKey = Flight.GetKey(cfa.FlightId, cfa.FlightVersion);
                }
                else
                {
                    Log.Warn("ClientFlightAllocation: ClientFlightAllocation.List.Count()={0}", List.Count()); 
                    throw new Exception("ClientFlightAllocation: Default flight for *_* client is not found.");
                }
            }

            return flightKey;
        }

        /// <summary>
        /// Translates xml to client flight allocation
        /// </summary>
        /// <param name="xe">client flight allocation xml</param>
        /// <returns>client flight allocation</returns>
        public static ClientFlightAllocation FromXml(XElement xe)
        {
            var client = new Client(xe.Attribute("ClientId").Value, xe.Attribute("ClientAppId").Value);
            var flight = new Flight(xe.Attribute("FlightId").Value, int.Parse(xe.Attribute("FlightVersion").Value));
            var allocationVersion = int.Parse(xe.Attribute("AllocationVersion").Value);
            var publishingVersion = int.Parse(xe.Attribute("PublishingVersion").Value);
            var trafficAllocation = xe.Descendants("TrafficAllocation").First();
            var trafficAllocationPercent = byte.Parse(trafficAllocation.Attribute("Percent").Value);
            var seedChangeIntervalSecondsAtt = trafficAllocation.Attributes("SeedChangeIntervalSeconds").FirstOrDefault();
            var seedChangeIntervalSeconds = seedChangeIntervalSecondsAtt != null ? (int?)int.Parse(seedChangeIntervalSecondsAtt.Value) : null;
            var startDateAtt = xe.Attributes("StartDate").FirstOrDefault();
            var startDate = startDateAtt != null ? (DateTime?)DateTime.Parse(startDateAtt.Value) : null;
            var endDateAtt = xe.Attributes("EndDate").FirstOrDefault();
            var endDate = endDateAtt != null ? (DateTime?)DateTime.Parse(endDateAtt.Value) : null;
            return new ClientFlightAllocation(client, flight, allocationVersion, publishingVersion, trafficAllocationPercent, startDate, endDate, seedChangeIntervalSeconds);
        }

        /// <summary>
        /// Translates client flight allocation to xml
        /// </summary>
        /// <returns>client flight allocations xml</returns>
        public XElement ToXml()
        {
            return new XElement(
                "ClientFlightAllocation",
                new XAttribute("ClientId", ClientId),
                new XAttribute("ClientAppId", ClientAppId),
                new XAttribute("FlightId", FlightId),
                new XAttribute("FlightVersion", FlightVersion),
                new XAttribute("AllocationVersion", AllocationVersion),
                new XAttribute("PublishingVersion", PublishingVersion),
                new XElement(
                    "Rules",
                    new XElement(
                        "TrafficAllocation",
                        new XAttribute("Percent", TrafficAllocationPercent),
                        SeedChangeIntervalSeconds != null ? new XAttribute("SeedChangeIntervalSeconds", SeedChangeIntervalSeconds) : null)),
                StartDate != null ? new XAttribute("StartDate", StartDate) : null,
                EndDate != null ? new XAttribute("EndDate", EndDate) : null);
        }

        /// <summary>
        /// Returns client flight allocation key
        /// </summary>
        /// <param name="clientId">client id</param>
        /// <param name="clientAppId">client app id</param>
        /// <param name="flightId">flight id</param>
        /// <param name="flightVersion">flight version</param>
        /// <param name="allocationVersion">the allocation version</param>
        /// <returns>the key</returns>
        private static string GetKey(string clientId, string clientAppId, string flightId, int flightVersion, int allocationVersion)
        {
            return Client.GetKey(clientId, clientAppId) + "_" + Flight.GetKey(flightId, flightVersion) + "_" + allocationVersion.ToString();
        }

        /// <summary>
        /// Convert string to percent bucket
        /// </summary>
        /// <param name="str">the string</param>
        /// <param name="seedChangeIntervalSeconds">seed change interval in seconds</param>
        /// <returns>percent bucket</returns>
        private static byte StringToBucket(string str, int? seedChangeIntervalSeconds)
        {
            str = str ?? string.Empty;
            var now = DateTime.UtcNow;
            Random rand;
            if (seedChangeIntervalSeconds != null)
            {
                var timeSeed = (int)(now.Ticks / TimeSpan.TicksPerSecond / seedChangeIntervalSeconds);
                var hash = str.GetHashCode();
                rand = new Random(timeSeed ^ hash);
            }
            else
            {
                rand = new Random();
            }

            var bucket = (byte)(100 * rand.NextDouble());
            return bucket;
        }
        
        #endregion
    }
}