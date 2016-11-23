//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The RankingGroup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using DotM.DataContracts.Constants;

    /// <summary>
    /// The RankingGroup
    /// </summary>
    public class RankingGroup
    {
        #region Fields

        /// <summary>
        /// Id of default ranking group;
        /// </summary>
        public const string DefaultId = "Default";

        /// <summary>
        /// Position of default ranking group in array of weighted ranks (starts with 1);
        /// </summary>
        public const int DefaultSequence = 1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="RankingGroup" /> class.
        /// </summary>
        static RankingGroup()
        {
            List = new Dictionary<string, RankingGroup>();
            SlotSequenceIndex = new Dictionary<byte, Dictionary<int, RankingGroup>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RankingGroup" /> class.
        /// </summary>
        /// <param name="rankingGroup">RankingGroup xml</param>
        public RankingGroup(XElement rankingGroup)
        {
            Id = rankingGroup.Attribute("Id").Value;
            Version = int.Parse(rankingGroup.Attribute("Version").Value);
            Key = GetKey(Id, Version);
            var nodes = rankingGroup.Nodes().Select(_ => XElement.Parse(_.ToString())).ToDictionary(_ => _.Name, _ => _);
            Rules = nodes["Rules"];

            var useRandomDealSelectionNode = Rules.Descendants("UseRandomDealSelection").FirstOrDefault();
            if (useRandomDealSelectionNode != null)
            {
                UseRandomDealSelection = bool.Parse(useRandomDealSelectionNode.Value);
            }
            
            var image = Rules.Descendants("Image").FirstOrDefault();
            if (image != null)
            {
                ImageHeight = int.Parse(image.Attribute("Height").Value);
                ImageWidth = int.Parse(image.Attribute("Width").Value);
            }

            var price = Rules.Descendants("Price").FirstOrDefault();
            if (price != null)
            {
                PriceMinimum = double.Parse(price.Attribute("Minimum").Value);
            }

            var discount = Rules.Descendants("Discount").FirstOrDefault();
            if (discount != null)
            {
                DiscountMinimum = int.Parse(discount.Attribute("Minimum").Value);
            }

            var problematicDealTitlePatterns = Rules.Descendants("ProblematicDealTitlePatterns").FirstOrDefault();
            ProblematicDealTitlePatterns = problematicDealTitlePatterns == null ? Enumerable.Empty<string>() : problematicDealTitlePatterns.Descendants("string").Select(_ => _.Value).ToList();

            var providerWeights = Rules.Descendants("ProviderWeights").FirstOrDefault();
            if (providerWeights != null)
            {
                DefaultProviderWeight = double.Parse(providerWeights.Descendants("Default").First().Attribute("Weight").Value);
                ExceptionProviderWeights = providerWeights.Descendants("Exception").Select(_ => new ProviderWeight(_.Attribute("Provider").Value, double.Parse(_.Attribute("Weight").Value))).ToList();
            }
            else
            {
                DefaultProviderWeight = 1;
                ExceptionProviderWeights = new List<ProviderWeight>();
            }
            
            var categoryWeights = Rules.Descendants("CategoryWeights").FirstOrDefault();
            if (categoryWeights != null)
            {
                DefaultCategoryWeight = double.Parse(categoryWeights.Descendants("Default").First().Attribute("Weight").Value);
                ExceptionCategoryWeights = categoryWeights.Descendants("Exception").Select(_ => new CategoryWeight(_.Attribute("Category").Value, double.Parse(_.Attribute("Weight").Value))).ToList();
            }
            else
            {
                DefaultCategoryWeight = 1;
                ExceptionCategoryWeights = new List<CategoryWeight>();
            }

            var dealTypeWeights = Rules.Descendants("DealTypeWeights").FirstOrDefault();
            if (dealTypeWeights != null)
            {
                DefaultDealTypeWeight = double.Parse(dealTypeWeights.Descendants("Default").First().Attribute("Weight").Value);
                ExceptionDealTypeWeights = dealTypeWeights.Descendants("Exception").Select(_ => new DealTypeWeight(int.Parse(_.Attribute("DealType").Value), double.Parse(_.Attribute("Weight").Value))).ToList();
            }
            else
            {
                DefaultDealTypeWeight = 1;
                ExceptionDealTypeWeights = new List<DealTypeWeight>();
            }

            if (nodes.ContainsKey("Keywords"))
            {
                var keywords = nodes["Keywords"];
                Keywords = keywords.Descendants("Keyword").Select(_ => _.Value).ToList();
            }
            else
            {
                Keywords = Enumerable.Empty<string>();
            }

            if (nodes.ContainsKey("Categories"))
            {
                var categories = nodes["Categories"];
                Categories = categories.Descendants("Category").Select(_ => _.Attribute("Name").Value).ToList();
            }
            else
            {
                Categories = Enumerable.Empty<string>();
            }

            var notFilteredId = rankingGroup.Attributes("NotFilteredId").FirstOrDefault();
            if (notFilteredId != null)
            {
                NotFilteredId = notFilteredId.Value;
                NotFilteredVersion = int.Parse(rankingGroup.Attribute("NotFilteredVersion").Value);
                NotFilteredKey = GetKey(NotFilteredId, NotFilteredVersion.Value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets ranking group list
        /// </summary>
        public static Dictionary<string, RankingGroup> List { get; set; }

        /// <summary>
        /// Gets or sets ranking group slot sequence list
        /// </summary>
        public static Dictionary<byte, Dictionary<int, RankingGroup>> SlotSequenceIndex { get; set; }

        /// <summary>
        ///  Gets the Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the Version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        ///  Gets the Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the Rules
        /// </summary>
        public XElement Rules { get; private set; }

        /// <summary>
        /// Gets a value indicating whether random selection of deals should be used
        /// </summary>
        public bool UseRandomDealSelection { get; private set; }

        /// <summary>
        /// Gets default deal type weight
        /// </summary>
        public double DefaultDealTypeWeight { get; private set; }

        /// <summary>
        /// Gets exception deal type weights
        /// </summary>
        public IEnumerable<DealTypeWeight> ExceptionDealTypeWeights { get; private set; }

        /// <summary>
        /// Gets default provider weight
        /// </summary>
        public double DefaultProviderWeight { get; private set; }

        /// <summary>
        /// Gets exception provider weights
        /// </summary>
        public IEnumerable<ProviderWeight> ExceptionProviderWeights { get; private set; }

        /// <summary>
        /// Gets default category weight
        /// </summary>
        public double DefaultCategoryWeight { get; private set; }

        /// <summary>
        /// Gets exception category weights
        /// </summary>
        public IEnumerable<CategoryWeight> ExceptionCategoryWeights { get; private set; }

        /// <summary>
        ///  Gets image_height
        /// </summary>
        public int? ImageHeight { get; private set; }

        /// <summary>
        /// Gets iamge-width
        /// </summary>
        public int? ImageWidth { get; private set; }

        /// <summary>
        ///  Gets cost threashold
        /// </summary>
        public double? PriceMinimum { get; private set; }

        /// <summary>
        ///  Gets discount threashold
        /// </summary>
        public int? DiscountMinimum { get; private set; }

        /// <summary>
        /// Gets problematic deal title patterns
        /// Subject line of type “$X for $Y” or “$X for $Y voucher” “$X Gift Certificate for $Y” suppresses.
        /// </summary>
        public IEnumerable<string> ProblematicDealTitlePatterns { get; private set; }

        /// <summary>
        /// Gets the Keywords
        /// </summary>
        public IEnumerable<string> Keywords { get; private set; }

        /// <summary>
        /// Gets the categories
        /// </summary>
        public IEnumerable<string> Categories { get; private set; }

        /// <summary>
        ///  Gets the NotFilteredId
        /// </summary>
        public string NotFilteredId { get; private set; }

        /// <summary>
        /// Gets the NotFilteredVersion
        /// </summary>
        public int? NotFilteredVersion { get; private set; }

        /// <summary>
        ///  Gets the NotFiltered Key
        /// </summary>
        public string NotFilteredKey { get; private set; }

        /// <summary>
        /// Gets the QueryRankArraySlot
        /// </summary>
        public byte? QueryRankArraySlot { get; private set; }

        /// <summary>
        /// Gets the QueryRankArraySequence
        /// </summary>
        public int? QueryRankArraySequence { get; private set; }

        /// <summary>
        /// Gets or sets temporary ranking group list to minimize time required for list swapping
        /// </summary>
        private static Dictionary<string, RankingGroup> WorkList { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Translates list of RankingGroups to xml
        /// </summary>
        /// <returns>RankingGroups xml</returns>
        public static XElement ListToXml()
        {
            return new XElement("RankingGroups", List.Select(_ => _.Value.ToXml()));
        }

        /// <summary>
        /// Translates xml to list of RankingGroups
        /// </summary>
        /// <param name="xml">ranking groups in xml</param>
        public static void XmlToWorkList(XElement xml)
        {
            var list = xml.Descendants("RankingGroup").Select(_ => new RankingGroup(_)).ToDictionary(_ => _.Key, _ => _);
            WorkList = list;
        }

        /// <summary>
        /// Returns ranking group key
        /// </summary>
        /// <param name="id">ranking group id</param>
        /// <param name="version">ranking group version</param>
        /// <returns>the key</returns>
        public static string GetKey(string id, int version)
        {
            return id + "." + version.ToString();
        }

        /// <summary>
        /// Removes not active entries from the list
        /// </summary>
        public static void RemoveNotActiveFromWorkList()
        {
            var list = PublishingVersion.List.Values
                                        .Where(_ => _.Slot != null)
                                        .Select(_ => _.SequenceRankingGroupKeyIndex)
                                        .SelectMany(_ => _.Values)
                                        .Select(_ => GetKey(_.Item1, _.Item2))
                                        .Distinct()
                                        .Select(_ => Get(_, WorkList))
                                        .ToList();
            var dic = list.ToDictionary(_ => _.Key, _ => _);

            WorkList = dic;
        }
        
        /// <summary>
        /// Updates query slot sequences on ranking group list
        /// </summary>
        public static void UpdateQuerySlotSequencesOnWorkList()
        {
            foreach (var version in PublishingVersion.List.Values.Where(_ => _.FullUpdateReplicasCompletedDate != null))
            {
                foreach (var sequence in version.SequenceRankingGroupKeyIndex)
                {
                    var rankingGroupKey = GetKey(sequence.Value.Item1, sequence.Value.Item2);
                    var rankingGroup = Get(rankingGroupKey, WorkList);
                    rankingGroup.QueryRankArraySlot = version.Slot;
                    rankingGroup.QueryRankArraySequence = sequence.Key;
                }
            }
        }

        /// <summary>
        /// Updates Slot Sequence Ranking Group Index
        /// </summary>
        public static void UpdateSlotSequenceIndexAndSwapLists()
        {
            var index = new Dictionary<byte, Dictionary<int, RankingGroup>>();
            foreach (var version in PublishingVersion.List.Values.Where(_ => _.Slot != null))
            {
                var dic = version.SequenceRankingGroupKeyIndex
                                 .Select(_ => new { Sequence = _.Key, RankingGroup = Get(GetKey(_.Value.Item1, _.Value.Item2), WorkList) })
                                 .ToDictionary(_ => _.Sequence, _ => _.RankingGroup);
                index.Add(version.Slot.Value, dic);
            }

            // though it is not atomic...
            SlotSequenceIndex = index;
            List = WorkList;
        }

        /// <summary>
        /// Translates RankingGroup in xml
        /// </summary>
        /// <returns>map xml</returns>
        public XElement ToXml()
        {
            return new XElement(
                "RankingGroup",
                new XAttribute("Id", Id),
                new XAttribute("Version", Version),
                Rules,
                Keywords.Any() ? new XElement("Keywords", Keywords.Select(_ => new XElement("Keyword", _))) : null,
                Categories.Any() ? new XElement("Categories", Categories.Select(_ => new XElement("Category", new XAttribute("Name", _)))) : null,
                NotFilteredId != null ? new XAttribute("NotFilteredId", NotFilteredId) : null,
                NotFilteredVersion != null ? new XAttribute("NotFilteredVersion", NotFilteredVersion) : null);
        }

        /// <summary>
        /// Gets ranking group from the list.
        /// </summary>
        /// <param name="key">RankingGroup key</param>
        /// <param name="list">RankingGroup list</param>
        /// <returns>RankingGroup pointer</returns>
        private static RankingGroup Get(string key, Dictionary<string, RankingGroup> list)
        {
            return list[key];
        }

        #endregion
    }
}