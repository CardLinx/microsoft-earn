//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The ClientFlightPlacement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Logging;

    /// <summary>
    /// The ClientFlightPlacement
    /// </summary>
    public class ClientFlightPlacement
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="ClientFlightPlacement" /> class.
        /// </summary>
        static ClientFlightPlacement()
        {
            List = new Dictionary<string, ClientFlightPlacement>(StringComparer.OrdinalIgnoreCase);
            Index = new Dictionary<string, Dictionary<string, ClientFlightPlacement>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFlightPlacement" /> class.
        /// </summary>
        /// <param name="client">the client</param>
        /// <param name="flight">the flight</param>
        /// <param name="placement">the placement</param>
        /// <param name="placementSequence">the placementSequence</param>
        /// <param name="rules">the rules</param>
        /// <param name="rankingGroupId">the rankingGroupId</param>
        /// <param name="rankingGroupVersion">the rankingGroupVersion</param>
        public ClientFlightPlacement(Client client, Flight flight, string placement, byte placementSequence, XElement rules, string rankingGroupId, int rankingGroupVersion)
        {
            ClientId = client.Id;
            ClientAppId = client.App;
            FlightId = flight.Id;
            FlightVersion = flight.Version;
            Placement = placement;
            Key = GetKey(Client.GetKey(ClientId, ClientAppId), Flight.GetKey(FlightId, FlightVersion), Placement);
            PlacementSequence = placementSequence;
            Rules = rules;

            DealsCountForRandomization = QueryRanking.DefaultDealsCountForRandomization;
            var dealsCountForRandomizationNode = Rules.Descendants("DealsCountForRandomization").FirstOrDefault();
            if (dealsCountForRandomizationNode != null)
            {
                int dealsCountForRandomization;
                var randomizeResultsValue = dealsCountForRandomizationNode.Attribute("Value").Value;
                if (int.TryParse(randomizeResultsValue, out dealsCountForRandomization))
                {
                    DealsCountForRandomization = dealsCountForRandomization;
                }
                else
                {
                    Log.Error("Incorrect NumberOfTopDealsToRandomize value in configuration: {0}", randomizeResultsValue);
                }
            }

            FallbackToOnlineDeals = true;
            var fallbackToOnlineDealsNode = Rules.Descendants("FallbackToOnlineDeals").FirstOrDefault();
            if (fallbackToOnlineDealsNode != null)
            {
                bool fallbackToOnlineDeals;
                var fallbackToOnlineDealsValue = fallbackToOnlineDealsNode.Attribute("Value").Value;
                if (bool.TryParse(fallbackToOnlineDealsValue, out fallbackToOnlineDeals))
                {
                    FallbackToOnlineDeals = fallbackToOnlineDeals;
                }
                else
                {
                    Log.Error("Incorrect FallbackToOnlineDeals value in configuration: {0}", fallbackToOnlineDealsValue);
                }
            }

            var countNode = Rules.Descendants("DefaultDealsCount").FirstOrDefault();
            DefaultDealsCount = countNode == null ? (byte)50 : byte.Parse(countNode.Value);
            RankingGroupId = rankingGroupId;
            RankingGroupVersion = rankingGroupVersion;
            RankingGroupKey = RankingGroup.GetKey(RankingGroupId, RankingGroupVersion);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets key to class map (Client/Flight/Placement Key, ClientFlightPlacement)
        /// </summary>
        public static Dictionary<string, ClientFlightPlacement> List { get; set; }

        /// <summary>
        /// Gets or sets client flight placements index (Client/Filght Key, Placement, ClientFlightPlacement)
        /// </summary>
        public static Dictionary<string, Dictionary<string, ClientFlightPlacement>> Index { get; set; }

        /// <summary>
        ///  Gets the client id
        /// </summary>
        public string Key { get; private set; }

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
        /// Gets the placement
        /// </summary>
        public string Placement { get; private set; }

        /// <summary>
        /// Gets the placement sequence
        /// </summary>
        public byte PlacementSequence { get; private set; }

        /// <summary>
        /// Gets the Rules
        /// </summary>
        public XElement Rules { get; private set; }

        /// <summary>
        /// Gets number of top deals to randomize.
        /// </summary>
        public int DealsCountForRandomization { get; private set; }

        /// <summary>
        /// Gets the DefaultDealsCount
        /// </summary>
        public byte DefaultDealsCount { get; private set; }

        /// <summary>
        ///  Gets the RankingGroup Id
        /// </summary>
        public string RankingGroupId { get; private set; }

        /// <summary>
        /// Gets the RankingGroup Version
        /// </summary>
        public int RankingGroupVersion { get; private set; }

        /// <summary>
        /// Gets the RankingGroup Key
        /// </summary>
        public string RankingGroupKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to show online deals in case when no other deals are present
        /// </summary>
        public bool FallbackToOnlineDeals { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Translates list of ClientFlightPlacements to xml
        /// </summary>
        /// <param name="clientFlightPlacements">list of ClientFlightPlacements</param>
        /// <returns>ClientFlightPlacements xml</returns>
        public static XElement ListToXml(IEnumerable<ClientFlightPlacement> clientFlightPlacements)
        {
            return new XElement("ClientFlightPlacements", clientFlightPlacements.Select(_ => _.ToXml()));
        }

        /// <summary>
        /// Translates list of ClientFlightPlacements to xml
        /// </summary>
        /// <returns>ClientFlightPlacements xml</returns>
        public static XElement ListToXml()
        {
            return new XElement("ClientFlightPlacements", List.Select(_ => _.Value.ToXml()));
        }

        /// <summary>
        /// Translates xml to list of ClientFlightPlacements
        /// </summary>
        /// <param name="xml">ClientFlightPlacements in xml</param>
        public static void XmlToList(XElement xml)
        {
            var dic = xml.Descendants("ClientFlightPlacement")
                         .Select(_ => new { K = GetKey(Client.GetKey(_.Attribute("ClientId").Value, _.Attribute("ClientAppId").Value), Flight.GetKey(_.Attribute("FlightId").Value, int.Parse(_.Attribute("FlightVersion").Value)), _.Attribute("Placement").Value), F = FromXml(_) })
                         .ToDictionary(_ => _.K, _ => _.F, StringComparer.OrdinalIgnoreCase);
            if (dic.Any())
            {
                List = dic;
            }
        }

        /// <summary>
        /// Translates list of placements
        /// </summary>
        public static void ListToIndex()
        {
            var dic = List.Select(_ => _.Value)
                          .GroupBy(_ => GetKey(Client.GetKey(_.ClientId, _.ClientAppId), Flight.GetKey(_.FlightId, _.FlightVersion)))
                          .Select(_ => new { _.Key, Dic = _.ToDictionary(p => p.Placement, p => GetClientFlightPlacement(p.Key), StringComparer.OrdinalIgnoreCase) })
                          .ToDictionary(_ => _.Key, _ => _.Dic, StringComparer.OrdinalIgnoreCase);
            if (dic.Any())
            {
                Index = dic;
            }
        }
        
        /// <summary>
        /// Returns client flight key
        /// </summary>
        /// <param name="clientKey">client key</param>
        /// <param name="flightKey"> flight key</param>
        /// <returns>the key</returns>
        public static string GetKey(string clientKey, string flightKey)
        {
            return clientKey + "_" + flightKey;
        }

        /// <summary>
        /// Gets ClientFlightPlacement by the key
        /// </summary>
        /// <param name="clientFlightPlacementKey">clientFlightPlacement Key</param>
        /// <returns>ClientFlightPlacement instance</returns>
        public static ClientFlightPlacement GetClientFlightPlacement(string clientFlightPlacementKey)
        {
            ClientFlightPlacement cfp;
            List.TryGetValue(clientFlightPlacementKey, out cfp);
            return cfp;
        }

        /// <summary>
        /// Removes not active instances from the list
        /// </summary>
        public static void RemoveNotActive()
        {
            var list = List.Values
                           .Where(cfp => RankingGroup.List.Values.Any(rg => rg.Key == cfp.RankingGroupKey))
                           .ToDictionary(_ => _.Key, _ => _, StringComparer.OrdinalIgnoreCase);
            List = list;
        }

        /// <summary>
        /// Translates xml to ClientFlightPlacement
        /// </summary>
        /// <param name="xe">ClientFlightPlacement xml</param>
        /// <returns>ClientFlightPlacement class</returns>
        public static ClientFlightPlacement FromXml(XElement xe)
        {
            var client = new Client(xe.Attribute("ClientId").Value, xe.Attribute("ClientAppId").Value);
            var flight = new Flight(xe.Attribute("FlightId").Value, int.Parse(xe.Attribute("FlightVersion").Value));
            var placement = xe.Attribute("Placement").Value;
            var placementSequence = byte.Parse(xe.Attribute("PlacementSequence").Value);
            var rules = XElement.Parse(xe.Nodes().First().ToString());
            var rankingGroupId = xe.Attribute("RankingGroupId").Value;
            var rankingGroupVersion = int.Parse(xe.Attribute("RankingGroupVersion").Value);
            return new ClientFlightPlacement(client, flight, placement, placementSequence, rules, rankingGroupId, rankingGroupVersion);
        }

        /// <summary>
        /// Translates ClientFlightPlacement in xml
        /// </summary>
        /// <returns>ClientFlightPlacement xml</returns>
        public XElement ToXml()
        {
            return new XElement(
                "ClientFlightPlacement",
                new XAttribute("ClientId", ClientId),
                new XAttribute("ClientAppId", ClientAppId),
                new XAttribute("FlightId", FlightId),
                new XAttribute("FlightVersion", FlightVersion),
                new XAttribute("Placement", Placement),
                new XAttribute("PlacementSequence", PlacementSequence),
                Rules,
                new XAttribute("RankingGroupId", RankingGroupId),
                new XAttribute("RankingGroupVersion", RankingGroupVersion));
        }

        /// <summary>
        /// Returns client flight placement key
        /// </summary>
        /// <param name="clientKey">client key</param>
        /// <param name="flightKey">flight key</param>
        /// <param name="placement">the placement</param>
        /// <returns>the key</returns>
        private static string GetKey(string clientKey, string flightKey, string placement)
        {
            return GetKey(clientKey, flightKey) + "_" + placement;
        }

        #endregion
    }
}