//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
// The query context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Encapsulation of the parameters used across all service calls. 
    /// </summary>
    public class QueryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext"/> class. 
        /// </summary>
        /// <param name="filters">query filters</param>
        /// <param name="client">the client</param>
        /// <param name="otherClients">other clients</param>
        /// <param name="sort">deas sort</param>
        /// <param name="userLocation">user location</param>
        /// <param name="userData">user data</param>
        /// <param name="inputFlights">input flights</param>
        /// <param name="parentEventId">parent event id</param>
        public QueryContext(QueryFilters filters = null, Client client = null, IEnumerable<Client> otherClients = null, DealsSort? sort = null, UserLocation userLocation = null, UserData userData = null, IEnumerable<string> inputFlights = null, Guid? parentEventId = null)
        {
            Filters = filters ?? new QueryFilters();
            Sort = sort ?? DealsSort.Relevance;
            UserLocation = userLocation;
            UserData = userData ?? new UserData();
            Client = client ?? new Client(Client.IdUnknown, Client.AppUnknown);
            OtherClients = otherClients ?? Enumerable.Empty<Client>();
            InputFlights = inputFlights;
            AssignFlightKey();
            Ranking = new QueryRanking(Client, Filters, FlightKey);
            EventId = Guid.NewGuid();
            ParentEventId = parentEventId == null ? EventId : parentEventId.Value;
        }

        #region Public Properties

        /// <summary>
        /// Gets the query filters.
        /// </summary>
        public QueryFilters Filters { get; private set; }

        /// <summary>
        /// Gets the preferred sort order for the results.
        /// </summary>
        public DealsSort Sort { get; private set; }

        /// <summary>
        /// Gets or sets the user location.
        /// </summary>
        public UserLocation UserLocation { get; set; }

        /// <summary>
        /// Gets the userIds.
        /// </summary>
        public UserData UserData { get; private set; }

        /// <summary>
        /// Gets the QueryRanking.
        /// </summary>
        public QueryRanking Ranking { get; private set; }

        /// <summary>
        /// Gets the client.
        /// </summary>
        public Client Client { get; private set; }

        /// <summary>
        /// Gets the referrer and/or other secondary clients.
        /// </summary>
        public IEnumerable<Client> OtherClients { get; private set; }

        /// <summary>
        /// Gets associated with client and user flight (key)
        /// </summary>
        public string FlightKey { get; private set; }

        /// <summary>
        /// Gets associated with client and user flight
        /// </summary>
        public IEnumerable<string> InputFlights { get; private set; }

        /// <summary>
        /// Gets associated event id
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// Gets associated parant event id
        /// </summary>
        public Guid ParentEventId { get; private set; }

        #endregion

        /// <summary>
        /// Gets users id from the context
        /// </summary>
        /// <returns>user id</returns>
        public UserIdInfo GetUserPrimaryIdInfo()
        {
            var userIdInfo = new UserIdInfo();

            if (UserData != null)
            {
                if (!string.IsNullOrWhiteSpace(UserData.Anid))
                {
                    userIdInfo.Type = UserIdType.Anid;
                    userIdInfo.Id = UserData.Anid;
                }
                else if (!string.IsNullOrWhiteSpace(UserData.Muid))
                {
                    userIdInfo.Type = UserIdType.Muid;
                    userIdInfo.Id = UserData.Muid;
                }
                else if (!string.IsNullOrEmpty(UserData.Upanid))
                {
                    userIdInfo.Type = UserIdType.Upanid;
                    userIdInfo.Id = UserData.Upanid;
                }
                else if (!string.IsNullOrEmpty(UserData.Ip))
                {
                    userIdInfo.Type = UserIdType.Ip;
                    userIdInfo.Id = UserData.Ip;
                }
            }

            return userIdInfo;
        }

        /// <summary>
        /// Get's user bucket for cache. All users are split into ~4000 buckets based on user id hash.
        /// </summary>
        /// <returns>the seed</returns>
        public int GetUserHashBucket()
        {
            var userId = GetUserPrimaryIdInfo().Id;
            if (!string.IsNullOrEmpty(userId))
            {
                return userId.GetHashCode() / 1000000 + 1000;
            }

            return -1;
        }

        /// <summary>
        /// Assigns Flight.Key
        /// </summary>
        private void AssignFlightKey()
        {
            var userId = GetUserPrimaryIdInfo().Id;

            // try to get deals server flight key from input list
            var flightKey = InputFlights != null ? InputFlights.FirstOrDefault(_ => _.StartsWith(Flight.KeyPrefix, true, CultureInfo.InvariantCulture)) : null;
            if (flightKey != null)
            {
                // get flight key from flight dictionary by full key match
                Flight flight;
                if (!Flight.List.TryGetValue(flightKey, out flight))
                {
                    // get flight key from flight dictionary by partial key match (take most recent version)
                    flight = Flight.List.Where(_ => _.Key.StartsWith(flightKey, true, CultureInfo.InvariantCulture)).OrderByDescending(_ => _.Value.Version).FirstOrDefault().Value;
                }

                if (flight != null)
                {
                    flightKey = flight.Key;
                    //// Check whether client-flight combination is valid;
                    var pcfps = QueryRanking.GetPlacementCfps(Client, flightKey, false);
                    if (pcfps != null)
                    {
                        // if client-flight combination is valid - remove from input list to avoid duplicate logging
                        InputFlights = InputFlights.Where(_ => _.ToLower() != flightKey.ToLower()).ToList();
                    }
                    else
                    {
                        // if client-flight combination is not valid set key to null
                        flightKey = null;
                    }
                }
                else
                {
                    // if not present in index set key to null
                    flightKey = null;
                }
            }

            FlightKey = flightKey ?? ClientFlightAllocation.GetFlightKey(Client.Key, userId);
        }
    }
}