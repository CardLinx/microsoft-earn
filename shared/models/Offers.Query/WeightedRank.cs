//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Weighted (Static) Rank.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using DotM.DataContracts;
    using DotM.DataContracts.Constants;
    using Lomo.Logging;

    /// <summary>
    /// The weighted (static) rank
    /// </summary>
    public static class WeightedRank
    {
        /// <summary>
        /// Random generator for use in random rank.
        /// </summary>
        private static Random rand = new Random();

        /// <summary>
        /// Static ranks for each deal, as read from azure blob storage.
        /// This is here to support indepedent flighting by Yagil who is external to our team. 
        /// (note by jletch, 11/21/2013)
        /// </summary>
        private static IDictionary<Guid, byte> blobDrivenRanks = new Dictionary<Guid, byte>();

        /// <summary>
        /// Lock for serializing access to the blobDrivenRanks object.
        /// </summary>
        private static ReaderWriterLockSlim blobDrivenRanksLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Stores the most recent time that the ranksFromBlob object was refreshed from the blob store.
        /// </summary>
        private static DateTime? lastBlobUpdateUtc = null;

        /// <summary>
        /// Defines the interval of time after which the ranksFromBlobCache is considered stale.
        /// If it is stale, it will be refreshed from the blob store upon the next access.
        /// </summary>
        private static TimeSpan maxStaleness = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets rank from rank array by ranking group sequence
        /// </summary>
        /// <param name="sequence">the ranking group sequence</param>
        /// <param name="weightedRanks">the weighted rank</param>
        /// <param name="publishingVersion">publishing version on the deal</param>
        /// <param name="queryPublishingVersion">publishing version of the query</param>
        /// <returns>the group rank</returns>
        public static byte GetRank(int sequence, byte[] weightedRanks, int publishingVersion, int queryPublishingVersion = 0)
        {
            byte res = 0;
            try
            {
                if (queryPublishingVersion == 0)
                {
                    queryPublishingVersion = PublishingVersion.List.FirstOrDefault().Key;
                }

                // if publishing version on deal does not match current don't return deal
                res = publishingVersion == queryPublishingVersion ? weightedRanks[sequence - 1] : (byte)0;
            }
            catch (Exception e)
            {
                Log.Warn("publishingVersion={0} sequence={1} weightedRanks.Count()={2}: {3}", publishingVersion, sequence, weightedRanks.Count(), e);
            }

            return res;
        }

        /// <summary>
        /// Various rules for adjusting the static rank
        /// </summary>
        /// <param name="deal">the deal</param>
        /// <param name="staticRank">static rank</param>
        /// <returns>adjusted rank</returns>
        public static byte GetAdjustedStaticRank(Deal deal, byte staticRank)
        {
            // Adjust the static rank

            // punish the at-home deals for now
            if (deal.LocationType == LocationType.NotSpecified)
            {
                staticRank = (byte)(staticRank / 2);
            }

            // adjust based on discount percentage 
            // Value – 
            // If Percent Savings >=25 Then +1
            // If percent Savings >=40 then +1 more
            if (deal.DealInfo != null)
            {
                if (deal.DealInfo.VoucherDiscountPercent >= 25)
                {
                    staticRank += 1;
                }

                if (deal.DealInfo.VoucherDiscountPercent >= 40)
                {
                    staticRank += 1;
                }
            }

            // Image resolutaion bonus
            if (deal.Images != null && deal.Images.Any(_ => _.Height >= 250 && _.Width >= 250))
            {
                staticRank += 1;
            }

            // If len Description field >= 100 then +1
            // chinese deals are different
            var avoidCountryOrRegions = new string[] { "CN", "JP", "HK", "TW", "KR" }; // China, Japen, Hong Kong, Taiwan, South Korea
            if (deal.Business != null && deal.Business.Locations != null)
            {
                var location = deal.Business.Locations.FirstOrDefault();
                if (location != null)
                {
                    if (!avoidCountryOrRegions.Contains(location.CountryOrRegion))
                    {
                        if (deal.Description != null && deal.Description.Length > 100)
                        {
                            staticRank += 1;
                        }
                    }
                }
            }

            // cap it
            if (staticRank > 100)
            {
                staticRank = 100;
            }

            return staticRank;
        }

        /// <summary>
        /// Calculates weighted ranks array. If static rank is not passed assumes that weighted ranks array has been populatated.
        /// </summary>
        /// <param name="deal">the deal</param>
        /// <param name="staticRank">the static rank</param>
        public static void Calculate(Deal deal, byte staticRank)
        {
            if (deal.DealRank == null)
            {
                deal.DealRank = new DealRank();
            }

            deal.DealRank.StaticRank = staticRank;
            deal.DealRank.PublishingVersions = new [] { 0, 0 };

            /******************************/
            /* adjusting the static rank  */
            /******************************/
            //// if static rank is 0, that means we actually don't want it to show up anywhere
            if (staticRank > 0)
            {
                staticRank = GetAdjustedStaticRank(deal, staticRank);
            }

            var slotSequences = RankingGroup.SlotSequenceIndex;
            foreach (var slot in slotSequences.Keys)
            {
                deal.DealRank.PublishingVersions[slot] = PublishingVersion.List.Values.FirstOrDefault(_ => _.Slot == slot).Version;

                var weightedRanks = new byte[slotSequences[slot].Count];

                if (slot == 0)
                {
                    deal.DealRank.WeightedRanks = weightedRanks;
                }
                else
                {
                    deal.DealRank.WeightedRanks1 = weightedRanks;
                }

                var dealProvider = deal.DealProvider;
                foreach (var rankigGroupSequence in slotSequences[slot])
                {
                    var rankingGroup = rankigGroupSequence.Value;
                    var sequence = rankigGroupSequence.Key;
                    var categories = deal.Categories != null ? deal.Categories.Select(_ => _.CategoryName).ToList() : new List<string>();
                    var weight = GetRankWeight(rankingGroup, dealProvider.ProviderId, dealProvider.SubProviderId, categories, deal.DealType);

                    // Apply special logic
                    if (rankingGroup.Id == "BingOffers")
                    {
                        weight = deal.Images != null && deal.Images.Any(i => i.ImageStatus != DealImage.ImageStatusType.DefaultImageBeingUsed) ? weight : 0;
                    }

                    // staticRank should be preserved
                    var staticRankInt = staticRank;

                    var rulesNode = rankingGroup.Rules.Descendants("UseCtrInRank").FirstOrDefault();
                    var pureCtrFlight = rulesNode != null && bool.Parse(rulesNode.Attribute("Value").Value);
                    if (pureCtrFlight)
                    {
                        double? ctr = null;
                        if (deal.TargetingData != null && deal.TargetingData.Ctrs != null)
                        {
                            var activeCtr = deal.TargetingData.Ctrs.Where(d => d.CtrLabel == CtrLabel.Default).FirstOrDefault();
                            if (activeCtr != null)
                            {
                                ctr = activeCtr.CtrValue;
                            }
                        }

                        staticRankInt = ComputeStaticRankFromCtr(ctr);
                        weight = weight == 0.0 ? 0.0 : 1.0;
                    }

                    rulesNode = rankingGroup.Rules.Descendants("IsRandom").FirstOrDefault();
                    if (rulesNode != null)
                    {
                        var exceptions = rulesNode.Descendants("ExceptionProvider").Select(_ => _.Attributes("Name").First().Value);
                        if (!exceptions.Any(_ => _.ToLower() == deal.DealProvider.ProviderId.ToLower()))
                        {
                            lock (rand)
                            {
                                staticRankInt = (byte)(rand.Next(80) + 10); // range from 10-90.
                            }
                        }
                    }

                    rulesNode = rankingGroup.Rules.Descendants("IsBlobDriven").FirstOrDefault();
                    if (rulesNode != null)
                    {
                        var exceptions = rulesNode.Descendants("ExceptionProvider").Select(_ => _.Attributes("Name").First().Value);
                        if (!exceptions.Any(_ => _.ToLower() == deal.DealProvider.ProviderId.ToLower()))
                        {
                            staticRankInt = GetBlobDrivenRank(deal.Id);
                        }
                    }

                    weightedRanks[sequence - 1] = (byte)(staticRankInt * weight);

                    if (weightedRanks[sequence - 1] > 0)
                    {
                        if (!AreRankingGroupRulesSatisfied(deal, rankingGroup))
                        {
                            weightedRanks[sequence - 1] = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the new rank    
        /// </summary>
        /// <param name="deal">the deal</param>
        /// <returns>value indicating whether the deal's rank info is updated or stayed the same</returns>
        public static bool UpdateRank(Deal deal)
        {
            // no need to update problematic deals
            if (deal == null)
            {
                return false;
            }

            ////if (deal.DealStatusId != null && deal.DealStatusId != DealStatus.Active)
            ////{
            ////    return false;
            ////}

            // skip problematic deals // || deal.DealRank.StaticRank == 0)
            if (deal.DealRank == null)
            {
                return false;
            }

            var oldData = deal.DealRank.WeightedRanks != null ? deal.DealRank.WeightedRanks.ToArray() : new byte[0];
            var oldData1 = deal.DealRank.WeightedRanks1 != null ? deal.DealRank.WeightedRanks1.ToArray() : new byte[0];
            var oldPublihingVersions = deal.DealRank.PublishingVersions ?? new int[0];
            var staticRank = deal.DealRank.StaticRank;
            Calculate(deal, staticRank);
            return !oldData.SequenceEqual(deal.DealRank.WeightedRanks != null ? deal.DealRank.WeightedRanks.ToArray() : new byte[0])
                || !oldData1.SequenceEqual(deal.DealRank.WeightedRanks1 != null ? deal.DealRank.WeightedRanks1.ToArray() : new byte[0])
                || oldPublihingVersions.SequenceEqual(deal.DealRank.PublishingVersions);
        }

        /// <summary>
        /// Refresh the in-memory blob-driven deal ranks from the blob storage.
        /// </summary>
        /// <param name="ranks">the ranks</param>
        public static void RefreshBlobDrivenRanks(IDictionary<Guid, byte> ranks)
        {
            blobDrivenRanksLock.EnterWriteLock();
            blobDrivenRanks = ranks;
            blobDrivenRanksLock.ExitWriteLock();

            lastBlobUpdateUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Indicates whether the blob driven ranks require a refresh.
        /// </summary>
        /// <returns>value indicating whether refresh is required</returns>
        public static bool BlobDrivenRanksNeedsRefresh()
        {
            return lastBlobUpdateUtc == null || DateTime.UtcNow - lastBlobUpdateUtc > maxStaleness;
        }

        /// <summary>
        /// Gets the blob-driven rank for the deal with the given id.
        /// </summary>
        /// <param name="dealId">Id of the deal.</param>
        /// <returns>The blob-based static rank of the deal, as it exists in the in-memory dictionary.</returns>
        public static byte GetBlobDrivenRank(string dealId)
        {
            Guid dealGuid = Guid.Empty;
            byte rank = 50;
            if (Guid.TryParse(dealId, out dealGuid))
            {
                blobDrivenRanksLock.EnterReadLock();
                if (blobDrivenRanks.ContainsKey(dealGuid))
                {
                    rank = blobDrivenRanks[dealGuid];
                }

                blobDrivenRanksLock.ExitReadLock();
            }

            return rank;
        }

        /// <summary>
        /// Maps a ctr value to an integer in the static rank range [0-100].
        /// Ctr values between 1.0 and 1e-7 get mapped via a logscale
        /// to the range 20-90.
        /// Ctr values below 1e-7 but greater than zero are mpaaed to 10.
        /// Ctr values of zero are mapped to 5.
        /// If ctr has no value (e.g. the deal is too new to have enough serves
        /// to compute a proper ctr), we give it a middling-high value.
        /// So we get:
        /// CTR        RANK
        /// ---------------
        /// 0.0         05   // ctr of zero is mapped to 05
        /// lt 1e-7     10   // ctr greater than zero but less than 1e-7 mapped to 10
        /// 0.0000001   20   // ctrs above 1e-7 are mapped via logscale into the range [20-90]
        /// 0.000001    30
        /// 0.00001     40
        /// 0.0001      50
        /// 0.001       60
        /// 0.01        70
        /// 0.1         80
        /// 1.0         90
        /// ?.?         Default - random. // deals without ctr values are given a random rank in [10-70].
        ///                       There are many deals (~15% of our corpus) that aren't served enough to 
        ///                       allow ctr to be computed.  We assign these deals random static rank to 
        ///                       give random subsets of them a chance to be served enough to gather real
        ///                       data.  
        /// </summary>
        /// <param name="ctr">the ctr</param>
        /// <returns>the rank</returns>
        public static byte ComputeStaticRankFromCtr(double? ctr)
        {
            if (!ctr.HasValue || ctr.Value < 0 || ctr.Value > 1.0)
            {
                int r = 0;
                lock (rand)
                {
                    r = rand.Next(60);
                }

                return (byte)(10 + r); // min: 10, max: 70
            }

            double ctrVal = ctr.Value;
            if (ctrVal == 0.0)
            {
                return 5;
            }
            else if (ctrVal < 1e-7)
            {
                return 10;
            }
            else
            {
                // ctr ranges from 1e-7 to 1.0, inclusive.
                // logCtr thus ranges from -7 (ctr 1e-7) to 0 (ctr 1.0)
                double logCtr = Math.Log10(ctrVal);

                // scaled ctr ranges from 70 (ctr 1e-7) to 0 (ctr 1.0)
                double scaled = -10.0 * logCtr;

                // final ctr ranges from 20 (ctr 1e-7) to 90 (ctr 1.0)
                double final = 90.0 - scaled;

                return (byte)Math.Floor(final);
            }
        }

        /// <summary>
        /// Checks whether Campaign Rule is Satisfied
        /// </summary>
        /// <param name="deal">the deal</param>
        /// <param name="rankingGroup">the rule</param>
        /// <returns>value indicating whether rule is satisfied</returns>
        private static bool AreRankingGroupRulesSatisfied(Deal deal, RankingGroup rankingGroup)
        {
            var res = false;
            if (deal.DealType != DealType.CardLinkDeal // don't go through checks for card link deals
                &&
                (rankingGroup.ImageHeight != null
                || rankingGroup.PriceMinimum != null
                || rankingGroup.DiscountMinimum != null
                || rankingGroup.Categories.Any()
                || rankingGroup.Keywords.Any()
                || rankingGroup.ProblematicDealTitlePatterns.Any()))
            {
                do
                {
                    // check image dimensions
                    if (rankingGroup.ImageHeight != null && deal.Images == null)
                    {
                        break;
                    }

                    if (rankingGroup.ImageHeight != null
                        &&
                        !deal.Images.Any(_ => _.ImageStatus == DealImage.ImageStatusType.GoodImage
                                              && _.Height >= rankingGroup.ImageHeight
                                              && _.Width >= rankingGroup.ImageWidth))
                    {
                        break;
                    }

                    // check value
                    if (rankingGroup.PriceMinimum != null && deal.Price < rankingGroup.PriceMinimum)
                    {
                        break;
                    }

                    if (deal.DealInfo == null)
                    {
                        break;
                    }

                    if (rankingGroup.DiscountMinimum != null &&
                        deal.DealInfo.VoucherDiscountPercent < rankingGroup.DiscountMinimum)
                    {
                        break;
                    }

                    // check content
                    if (rankingGroup.Categories.Any()
                        &&
                        !deal.Categories.Any(_ => rankingGroup.Categories.Any(rc => string.Equals(rc, _.CategoryName))))
                    {
                        break;
                    }

                    if (rankingGroup.Keywords.Any())
                    {
                        if (!rankingGroup.Keywords.Any(_ => deal.Description.Contains(_) || deal.Title.Contains(_)))
                        {
                            break;
                        }
                    }

                    if (rankingGroup.ProblematicDealTitlePatterns.Any() && deal.Title != null)
                    {
                        // match title or businessname
                        if (
                            rankingGroup.ProblematicDealTitlePatterns.Any(
                                _ => ((new Regex(_, RegexOptions.IgnoreCase)).IsMatch(deal.Title)
                                      ||
                                      (deal.Business != null &&
                                       new Regex(_, RegexOptions.IgnoreCase).IsMatch(deal.Business.Name)))))
                        {
                            break;
                        }
                    }

                    res = true;
                }
                while (false);
            }
            else
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Gets Rank Weight
        /// </summary>
        /// <param name="rankingGroup">the rule</param>
        /// <param name="providerId">provider id</param>
        /// <param name="subProviderId">sub provider id</param>
        /// <param name="categories">category names</param>
        /// <param name="dealtype">the deal type</param>
        /// <returns>rank weight</returns>
        private static double GetRankWeight(RankingGroup rankingGroup, string providerId, string subProviderId, IList<string> categories, int dealtype)
        {
            var subProviderKey = string.IsNullOrEmpty(subProviderId) ? null : providerId + "/" + subProviderId.ToLower();

            var wP = rankingGroup.DefaultProviderWeight;
            foreach (var exception in rankingGroup.ExceptionProviderWeights)
            {
                if (string.Equals(exception.Provider, providerId, StringComparison.OrdinalIgnoreCase)
                    ||
                    (subProviderId != null && string.Equals(exception.Provider, subProviderKey, StringComparison.OrdinalIgnoreCase)))
                {
                    wP = exception.Weight;
                    break;
                }
            }

            var wC = rankingGroup.DefaultCategoryWeight;
            foreach (var exception in rankingGroup.ExceptionCategoryWeights)
            {
                if (categories.Any(_ => string.Equals(exception.Category, _, StringComparison.OrdinalIgnoreCase)))
                {
                    wC = exception.Weight;
                    break;
                }
            }

            var wDt = rankingGroup.DefaultDealTypeWeight;
            foreach (var exception in rankingGroup.ExceptionDealTypeWeights)
            {
                if (exception.DealType == dealtype)
                {
                    wDt = exception.Weight;
                    break;
                }
            }

            return wP * wC * wDt;
        }
    }
}