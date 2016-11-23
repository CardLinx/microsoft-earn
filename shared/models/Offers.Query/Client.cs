//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Xml.Linq;
    using Logging;

    /// <summary>
    /// The client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// client id is unknown
        /// </summary>
        public const string IdUnknown = "Unknown";

        /// <summary>
        /// Applies to all/any client ids
        /// </summary>
        public const string IdAll = "*";
        
        /// <summary>
        /// client app is unknown
        /// </summary>
        public const string AppUnknown = "Unknown";

        /// <summary>
        /// Applies to all/any client app
        /// </summary>
        public const string AppAll = "*";

        /// <summary>
        /// stores id value internally
        /// </summary>
        private string id = IdUnknown;

        /// <summary>
        /// stores app value internally
        /// </summary>
        private string app = AppUnknown;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Client" /> class.
        /// </summary>
        static Client()
        {
            IdMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            AppMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="userAgents">collection of user agents</param>
        /// <param name="validateId">value indicating whether check to unknown is performed</param>
        public Client(ICollection<ProductInfoHeaderValue> userAgents, bool validateId = true)
        {
            if (userAgents != null && userAgents.Count > 0)
            {
                ParseClient(userAgents.First().Product.Name, validateId);
            }

            AssignValues();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="userAgents">collection of user agents</param>
        /// <param name="validateId">value indicating whether check to unknown is performed</param>
        public Client(string userAgents, bool validateId = true)
        {
            if (!string.IsNullOrWhiteSpace(userAgents))
            {
                ParseClient(userAgents.Split(' ')[0], validateId);
            }

            AssignValues();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="clientId">the client id</param>
        /// <param name="clientApp">the client app</param>
        public Client(string clientId, string clientApp)
        {
            Id = clientId;
            App = clientApp; 
            AssignValues();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Value-to-Id Map
        /// </summary>
        public static Dictionary<string, string> IdMap { get; set; }

        /// <summary>
        /// Gets or sets Id to attributes Map
        /// </summary>
        public static Dictionary<string, XElement> IdProperties { get; set; }

        /// <summary>
        /// Gets or sets Value-to-App Map
        /// </summary>
        public static Dictionary<string, string> AppMap { get; set; }

        /// <summary>
        /// Gets original client string that was sent by the caller
        /// </summary>
        public string InputStr { get; private set; }

        /// <summary>
        /// Gets the client id. If cannot uses default.
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            } 

            private set 
            { 
                if (!string.IsNullOrEmpty(value))
                {
                    id = value;
                } 
            }
        }

        /// <summary>
        /// Gets the client app. If cannot uses default.
        /// </summary>
        public string App
        {
            get
            {
                return app;
            } 

            private set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    app = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets associated with client key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets a value indicating whether to output deals must be filtered by market
        /// </summary>
        public bool FilterDealsByMarket { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this client is monetizable
        /// </summary>
        public bool IsMonetizable { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Translates id map to xml
        /// </summary>
        /// <returns>map xml</returns>
        public static XElement IdMapToXml()
        {
            return new XElement("ClientIdMap", IdMap.Select(_ => new XElement("Client", new XAttribute("Value", _.Key), new XAttribute("Id", _.Value))));
        }

        /// <summary>
        /// Translates id properties to xml
        /// </summary>
        /// <returns>map xml</returns>
        public static XElement IdPropertiesToXml()
        {
            return new XElement("ClientProperties", IdProperties.Select(_ => _.Value));
        }

        /// <summary>
        /// Translates app map to xml
        /// </summary>
        /// <returns>map xml</returns>
        public static XElement AppMapToXml()
        {
            return new XElement("ClientAppMap", AppMap.Select(_ => new XElement("ClientApp", new XAttribute("Value", _.Key), new XAttribute("Id", _.Value))));
        }

        /// <summary>
        /// Translates xml to app map
        /// </summary>
        /// <param name="xml">app map in xml</param>
        public static void XmlToAppMap(XElement xml)
        {
            var dic = xml.Descendants("ClientApp")
                               .Select(_ => new { V = _.Attribute("Value").Value, Id = _.Attribute("Id").Value })
                               .ToDictionary(_ => _.V, _ => _.Id, StringComparer.OrdinalIgnoreCase);
            if (dic.Any())
            {
                AppMap = dic;
            }
        }

        /// <summary>
        /// Translates xml to id map
        /// </summary>
        /// <param name="xml">app map in xml</param>
        public static void XmlToIdMap(XElement xml)
        {
            var dic = xml.Descendants("Client")
                         .Select(_ => new { V = _.Attribute("Value").Value, Id = _.Attribute("Id").Value })
                         .ToDictionary(_ => _.V, _ => _.Id, StringComparer.OrdinalIgnoreCase);
            if (dic.Any())
            {
                IdMap = dic;
            }
        }

        /// <summary>
        /// Translates xml to id properties index
        /// </summary>
        /// <param name="xml">app map in xml</param>
        public static void XmlToIdProperties(XElement xml)
        {
            var dic = xml.Descendants("Client")
                         .Select(_ => new { Id = _.Attribute("Id").Value, Xml = _ })
                         .ToDictionary(_ => _.Id, _ => _.Xml, StringComparer.OrdinalIgnoreCase);
            if (dic.Any())
            {
                IdProperties = dic;
            }
        }

        /// <summary>
        /// Gets key 
        /// </summary>
        /// <param name="clientId">the client id</param>
        /// <param name="clientApp">the client app</param>
        /// <returns>the client key</returns>
        public static string GetKey(string clientId, string clientApp)
        {
            return clientId + "_" + clientApp;
        }

        /// <summary>
        /// The ToString().
        /// </summary>
        /// <returns>A string representation of the ServiceCaller.</returns>
        public override string ToString()
        {
            return Key;
        }

        /// <summary>
        /// Parse the caller from the specified caller string.
        /// </summary>
        /// <param name="clientId">client id string</param>
        private void ParseClientId(string clientId)
        {
            string str;
            IdMap.TryGetValue(clientId.ToUpperInvariant(), out str);
            Id = str;
        }

        /// <summary>
        /// Parse the caller's client from the specified callerClient string.
        /// </summary>
        /// <param name="clientApp">client app string</param>
        private void ParseClientApp(string clientApp)
        {
            string str;
            AppMap.TryGetValue(clientApp.ToUpperInvariant(), out str);
            App = str;
        }

        /// <summary>
        /// Parse the caller and its client from the specified serviceCaller string.
        /// </summary>
        /// <param name="client">client string</param>
        /// <param name="validateId">value indicating whether check to unknown is performed</param>
        private void ParseClient(string client, bool validateId)
        {
            InputStr = client;
            if (!string.IsNullOrWhiteSpace(client))
            {
                var callerSegments = client.Split('_');
                ParseClientId(callerSegments[0]);
                var callerClientString = string.Empty;
                if (callerSegments.Length > 1)
                {
                    callerClientString = callerSegments[1];
                }

                ParseClientApp(callerClientString);
            }

            if (validateId && Id == IdUnknown)
            {
                Log.Warn("Client Id is unknown. Input string=[{0}]", client);
            }
        }

        /// <summary>
        /// Assigns field values to service caller 
        /// </summary>
        private void AssignValues()
        {
            Key = GetKey(Id, App);
            
            // TODO: Consider moving to table driven configuration
            FilterDealsByMarket = Id == "Skype" || Id == "AppExTravel" || (Id == "BingOffers" && App == "Email");

            IsMonetizable = Id == "AppExTravel" || Id == "AppExWeather" || Id == "Ieb" || Id == "OutlookCom";
        }

        #endregion
    }
}