//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Flight.
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
    /// The Flight
    /// </summary>
    public class Flight
    {
        /// <summary>
        /// default flight id
        /// </summary>
        public const string IdDefault = "Default";

        /// <summary>
        /// flight id prefix
        /// </summary>
        public const string KeyPrefix = "DealsServer";
 
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="Flight" /> class.
        /// </summary>
        static Flight()
        {
            List = new Dictionary<string, Flight>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flight" /> class.
        /// </summary>
        /// <param name="id">flight id</param>
        /// <param name="version">flight version</param>
        /// <param name="rules">flight rules</param>
        /// <param name="externalId">external Id</param>
        /// <param name="description">flight description</param>
        public Flight(string id, int version, XElement rules = null, string externalId = null, string description = null)
        {
            Id = id;
            Version = version;
            Key = GetKey(Id, Version);
            ExternalId = externalId;
            Description = description;
            Rules = rules ?? new XElement("Rules", new XElement("Regions", new XElement("Region", "bo:*")));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets flight list indexed on flight key (case insensitive)
        /// </summary>
        public static Dictionary<string, Flight> List { get; set; }

        /// <summary>
        ///  Gets the Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the Version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Gets the Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the ExternalId
        /// </summary>
        public string ExternalId { get; private set; }

        /// <summary>
        /// Gets the Description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the Rules
        /// </summary>
        public XElement Rules { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns flight key
        /// </summary>
        /// <param name="id">flight id</param>
        /// <param name="version">flight version</param>
        /// <returns>the key</returns>
        public static string GetKey(string id, int version)
        {
            return KeyPrefix + "." + id + "." + version.ToString();
        }

        /// <summary>
        /// Translates list of flights to xml
        /// </summary>
        /// <param name="flights">list of flights</param>
        /// <returns>flights xml</returns>
        public static XElement ListToXml(IEnumerable<Flight> flights)
        {
            return new XElement("Flights", flights.Select(_ => _.ToXml()));
        }

        /// <summary>
        /// Translates list of flights to xml
        /// </summary>
        /// <returns>flights xml</returns>
        public static XElement ListToXml()
        {
            return new XElement("Flights", List.Select(_ => _.Value.ToXml()));
        }

        /// <summary>
        /// Translates xml to list of flights
        /// </summary>
        /// <param name="xml">flights in xml</param>
        public static void XmlToList(XElement xml)
        {
            var dic = xml.Descendants("Flight")
                         .Select(_ => new { K = GetKey(_.Attribute("Id").Value, int.Parse(_.Attribute("Version").Value)), F = FromXml(_) })
                         .ToDictionary(_ => _.K, _ => _.F);
            if (dic.Any())
            {
                List = dic;
            }
        }

        /// <summary>
        /// Removes not active entries from the list
        /// </summary>
        public static void RemoveNotActive()
        {
            var list = List.Values
                           .Where(f => ClientFlightPlacement.List.Values.Any(p => f.Id == p.FlightId && f.Version == p.FlightVersion))
                           .ToDictionary(_ => GetKey(_.Id, _.Version), _ => _);
            List = list;
        }

        /// <summary>
        /// Checks if the given flight name is an internal deal server flight
        /// </summary>
        /// <param name="flightName">flight name to check</param>
        /// <returns>true or false if the flight name is an internal deal server flight</returns>
        public static bool IsInternalDealServerFlight(string flightName)
        {
            bool isInternalDealsServerFlight = false;

            if (!string.IsNullOrEmpty(flightName))
            {
                var flightNameParts = flightName.Split(new[] { '.' });
                if (flightNameParts.Length > 0 && String.Compare(flightNameParts[0], KeyPrefix, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    isInternalDealsServerFlight = true;
                }
            }

            return isInternalDealsServerFlight;
        }

        /// <summary>
        /// Gets flight from the list.
        /// </summary>
        /// <param name="key">flight key</param>
        /// <returns>the flight</returns>
        public static Flight Get(string key)
        {
            Flight flight;
            if (key == null)
            {
                flight = List.Values.FirstOrDefault(_ => _.Id == "Default");
            }
            else
            {
                List.TryGetValue(key, out flight);
            }

            if (flight == null)
            {
                Log.Error("Flight is null.");
            }

            return flight;
        }

        /// <summary>
        /// Translates xml to Fligt 
        /// </summary>
        /// <param name="xe">flight xml</param>
        /// <returns>the flight</returns>
        public static Flight FromXml(XElement xe)
        {
            var id = xe.Attribute("Id").Value;
            var version = int.Parse(xe.Attribute("Version").Value);
            //// external id always comes populated from the database
            var externalId = xe.Attributes("ExternalId").FirstOrDefault().Value;
            var descrAtt = xe.Attributes("Description").FirstOrDefault();
            var descrValue = descrAtt != null ? descrAtt.Value : null;
            var rules = XElement.Parse(xe.Nodes().First().ToString());
            return new Flight(id, version, rules, externalId, descrValue);
        }

        /// <summary>
        /// Translates flight in xml
        /// </summary>
        /// <returns>flight xml</returns>
        public XElement ToXml()
        {
            return new XElement(
                "Flight",
                new XAttribute("Id", Id),
                new XAttribute("Version", Version),
                ExternalId != null ? new XAttribute("ExternalId", ExternalId) : null,
                Description != null ? new XAttribute("Description", Description) : null,
                Rules);
        }

        #endregion
    }
}