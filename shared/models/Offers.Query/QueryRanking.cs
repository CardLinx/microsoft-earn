//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Holds rakning info
    /// </summary>
    public class QueryRanking
    {
        /// <summary>
        /// Default number of top deals to randomize
        /// </summary>
        public const int DefaultDealsCountForRandomization = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRanking" /> class.
        /// </summary>
        /// <param name="client">the client</param>
        /// <param name="filters">query filters</param>
        /// <param name="flightKey">query flight key</param>
        public QueryRanking(Client client, QueryFilters filters, string flightKey)
        {
            var defaultRankingGroupSequence = RankingGroup.DefaultSequence;
            var pcfps = GetPlacementCfps(client, flightKey, true);
            var featureRankingGroupSequences = new List<int>();
            var featureRankingGroupCounts = new List<byte>();
            var sequenceToPlacementIndex = new Dictionary<int, string>();
            var isFiltered = (filters.QueryKeywords != null && filters.QueryKeywords.Any()) || (filters.Categories != null && filters.Categories.Any());
            foreach (var pcfp in pcfps.OrderBy(_ => _.Value.PlacementSequence))
            {
                var cfpInt = pcfp.Value;
                RankingGroup rankingGroup;
                if (!RankingGroup.List.TryGetValue(cfpInt.RankingGroupKey, out rankingGroup))
                {
                    Log.Warn("QueryRanking: Ranking group for {0} key is not found.", cfpInt.RankingGroupKey);
                    rankingGroup = RankingGroup.List.Values.FirstOrDefault(_ => _.Id == RankingGroup.DefaultId);
                    if (rankingGroup == null)
                    {
                        Log.Warn("QueryRanking: RankingGroup.List.Count()={0}", RankingGroup.List.Count());
                        throw new Exception("QueryRanking: Default ranking group is not found.");
                    }
                }

                // switch to unfiltered ranking group if query has explicit filters
                RankingGroup notFilteredRankingGroup;
                if (isFiltered && rankingGroup.NotFilteredKey != null && RankingGroup.List.TryGetValue(rankingGroup.NotFilteredKey, out notFilteredRankingGroup))
                {
                    rankingGroup = notFilteredRankingGroup;
                }

                var sequence = rankingGroup.QueryRankArraySequence != null
                                    ? rankingGroup.QueryRankArraySequence.Value
                                    : RankingGroup.DefaultSequence;
                if (cfpInt.Placement == "Default")
                {
                    UseRandomDealSelection = rankingGroup.UseRandomDealSelection;
                    defaultRankingGroupSequence = sequence;
                    DealsCountForRandomization = cfpInt.DealsCountForRandomization;
                    FallbackToOnlineDeals = cfpInt.FallbackToOnlineDeals;

                    // There are cases when the same ranking group is used for default and other placements. 
                    // For these cases it better to have mapping to default
                    if (sequenceToPlacementIndex.ContainsKey(sequence))
                    {
                        sequenceToPlacementIndex.Remove(sequence);
                    }

                    sequenceToPlacementIndex.Add(sequence, cfpInt.Placement);
                }
                else
                {
                    featureRankingGroupSequences.Add(sequence);
                    featureRankingGroupCounts.Add(cfpInt.DefaultDealsCount);
                    if (!sequenceToPlacementIndex.ContainsKey(sequence))
                    {
                        sequenceToPlacementIndex.Add(sequence, cfpInt.Placement);
                    }
                }
            }

            DefaultRankingGroupSequence = defaultRankingGroupSequence;
            FeatureRankingGroupSequences = featureRankingGroupSequences.ToList();
            FeatureRankingGroupCounts = featureRankingGroupCounts.ToList();
            SequenceToPlacementIndex = sequenceToPlacementIndex;

            RankArraySlot = RankingGroup.List.Values.FirstOrDefault().QueryRankArraySlot.Value;
            PublishingVersion = Query.PublishingVersion.List.FirstOrDefault().Key;
        }

        /// <summary>
        /// Gets a value indicating whether random selection of deals should be used
        /// </summary>
        public bool UseRandomDealSelection { get; private set; }
        
        /// <summary>
        /// Gets the ranking group id.
        /// </summary>
        public int DefaultRankingGroupSequence { get; private set; }

        /// <summary>
        /// Gets list of other ranking group ids.
        /// </summary>
        public IEnumerable<int> FeatureRankingGroupSequences { get; private set; }

        /// <summary>
        /// Gets list of ranking group counts.
        /// </summary>
        public IEnumerable<byte> FeatureRankingGroupCounts { get; private set; }

        /// <summary>
        /// Gets rank array slot used for querying.
        /// </summary>
        public byte RankArraySlot { get; private set; }
        
        /// <summary>
        /// Gets publishing version used for querying.
        /// </summary>
        public int PublishingVersion { get; private set; }

        /// <summary>
        /// Gets number of top deals to randomize.
        /// </summary>
        public int DealsCountForRandomization { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to show online deals in case when no other deals are present
        /// </summary>
        public bool FallbackToOnlineDeals { get; private set; }
        
        /// <summary>
        /// Gets sequence-to-placement index
        /// </summary>
        public Dictionary<int, string> SequenceToPlacementIndex { get; private set; }

        /// <summary>
        /// (placement->client-flight-placement) dictonary for client and flight
        /// </summary>
        /// <param name="client">the client</param>
        /// <param name="flightKey">the flight key</param>
        /// <param name="allowToUseDefaultFlight">value indicating whether method is allowed to map to the default flight</param>
        /// <returns>(placement->client-flight-placement) dictonary</returns>
        public static Dictionary<string, ClientFlightPlacement> GetPlacementCfps(Client client, string flightKey, bool allowToUseDefaultFlight)
        { 
            var clientFlightKey = ClientFlightPlacement.GetKey(client.Key, flightKey);
            Dictionary<string, ClientFlightPlacement> pcfps;
            if (!ClientFlightPlacement.Index.TryGetValue(clientFlightKey, out pcfps))
            {
                clientFlightKey = ClientFlightPlacement.GetKey(Client.GetKey(client.Id, Client.AppAll), flightKey);
                if (!ClientFlightPlacement.Index.TryGetValue(clientFlightKey, out pcfps))
                {
                    var clientKey = Client.GetKey(Client.IdAll, Client.AppAll);
                    clientFlightKey = ClientFlightPlacement.GetKey(clientKey, flightKey);
                    if (!ClientFlightPlacement.Index.TryGetValue(clientFlightKey, out pcfps))
                    {
                        if (allowToUseDefaultFlight)
                        {
                            Log.Warn("QueryRanking: Placement dictionary for {0} client with {1} flight is not found.", clientKey, flightKey);
                            var cfp = ClientFlightPlacement.List.Values
                                        .Where(_ => _.ClientId == Client.IdAll && _.ClientAppId == Client.AppAll && _.FlightId == Flight.IdDefault)
                                        .OrderByDescending(_ => _.FlightVersion)
                                        .FirstOrDefault();
                            if (cfp != null)
                            {
                                clientFlightKey = ClientFlightPlacement.GetKey(Client.GetKey(cfp.ClientId, cfp.ClientAppId), Flight.GetKey(cfp.FlightId, cfp.FlightVersion));
                                ClientFlightPlacement.Index.TryGetValue(clientFlightKey, out pcfps);
                            }
                            else
                            {
                                Log.Warn("QueryRanking: ClientFlightPlacement.List.Count()={0}", ClientFlightPlacement.List.Count());
                            }

                            if (pcfps == null)
                            {
                                throw new Exception("QueryRanking: Placement dictionary for *_* client with default flight is not found.");
                            }
                        }
                    }
                }
            }

            return pcfps;
        }
    }
}