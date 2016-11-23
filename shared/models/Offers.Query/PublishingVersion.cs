//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The PublishingVersion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The PublishingVersion
    /// </summary>
    public class PublishingVersion
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="PublishingVersion" /> class.
        /// </summary>
        static PublishingVersion()
        {
            List = new Dictionary<int, PublishingVersion>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishingVersion" /> class.
        /// </summary>
        /// <param name="xe">PublishingVersion xml</param>
        public PublishingVersion(XElement xe)
        {
            Version = int.Parse(xe.Attribute("Version").Value);
            var slot = xe.Attributes("Slot").FirstOrDefault();
            if (slot != null)
            {
                Slot = byte.Parse(slot.Value);
            }

            var definitionCompletedDate = xe.Attributes("DefinitionCompletedDate").FirstOrDefault();
            if (definitionCompletedDate != null)
            {
                DefinitionCompletedDate = DateTime.Parse(definitionCompletedDate.Value);
            }

            var fullUpdateMasterStartedDate = xe.Attributes("FullUpdateMasterStartedDate").FirstOrDefault();
            if (fullUpdateMasterStartedDate != null)
            {
                FullUpdateMasterStartedDate = DateTime.Parse(fullUpdateMasterStartedDate.Value);
            }

            var fullUpdateMasterCompletedDate = xe.Attributes("FullUpdateMasterCompletedDate").FirstOrDefault();
            if (fullUpdateMasterCompletedDate != null)
            {
                FullUpdateMasterCompletedDate = DateTime.Parse(fullUpdateMasterCompletedDate.Value);
            }

            var fullUpdateReplicasCompletedDate = xe.Attributes("FullUpdateReplicasCompletedDate").FirstOrDefault();
            if (fullUpdateReplicasCompletedDate != null)
            {
                FullUpdateReplicasCompletedDate = DateTime.Parse(fullUpdateReplicasCompletedDate.Value);
            }

            SequenceRankingGroupKeyIndex = xe.Descendants("RankingGroup")
                                             .ToDictionary(
                                                    _ => int.Parse(_.Attribute("Sequence").Value),
                                                    _ => new Tuple<string, int>(_.Attribute("Id").Value, int.Parse(_.Attribute("Version").Value)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Value-to-Id Map
        /// </summary>
        public static Dictionary<int, PublishingVersion> List { get; set; }

        /// <summary>
        /// Gets the SequenceRankingGroupKeyIndex
        /// </summary>
        public Dictionary<int, Tuple<string, int>> SequenceRankingGroupKeyIndex { get; private set; }

        /// <summary>
        /// Gets the publishing version identifier
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Gets the rank array slot
        /// </summary>
        public byte? Slot { get; private set; }

        /// <summary>
        /// Gets the DefinitionCompletedDate
        /// </summary>
        public DateTime? DefinitionCompletedDate { get; private set; }

        /// <summary>
        /// Gets the FullUpdateMasterStartedDate
        /// </summary>
        public DateTime? FullUpdateMasterStartedDate { get; private set; }

        /// <summary>
        /// Gets the FullUpdateMasterCompletedDate
        /// </summary>
        public DateTime? FullUpdateMasterCompletedDate { get; private set; }

        /// <summary>
        /// Gets the FullUpdateReplicasCompletedDate
        /// </summary>
        public DateTime? FullUpdateReplicasCompletedDate { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Translates list of PublishingVersions to xml
        /// </summary>
        /// <param name="publishingVersions">list of PublishingVersions</param>
        /// <returns>PublishingVersions xml</returns>
        public static XElement ListToXml(IEnumerable<PublishingVersion> publishingVersions)
        {
            return new XElement("PublishingVersions", publishingVersions.Select(_ => _.ToXml()));
        }

        /// <summary>
        /// Translates list of PublishingVersions to xml
        /// </summary>
        /// <returns>PublishingVersions xml</returns>
        public static XElement ListToXml()
        {
            return new XElement("PublishingVersions", List.Select(_ => _.Value.ToXml()));
        }

        /// <summary>
        /// Translates xml to list of PublishingVersions
        /// </summary>
        /// <param name="xml">PublishingVersions in xml</param>
        public static void XmlToList(XElement xml)
        {
            var dic = xml.Descendants("PublishingVersion")
                         .Select(_ => new { K = int.Parse(_.Attribute("Version").Value), F = new PublishingVersion(_) })
                         .ToDictionary(_ => _.K, _ => _.F);
            if (dic.Any())
            {
                List = dic;
            }
        }

        /// <summary>
        /// Leaves only queryable entry in the list
        /// </summary>
        public static void RemoveNotFullyUpdated()
        {
            var list = List.Values
                           .Where(_ => _.Slot != null && _.FullUpdateReplicasCompletedDate != null)
                           .OrderByDescending(_ => _.Version)
                           .Take(1)
                           .ToDictionary(_ => _.Version, _ => _);
            List = list;
        }

        /// <summary>
        /// Translates PublishingVersion in xml
        /// </summary>
        /// <returns>PublishingVersion xml</returns>
        public XElement ToXml()
        {
            return new XElement(
                "PublishingVersion",
                new XAttribute("Version", Version),
                Slot != null ? new XAttribute("Slot", Slot) : null,
                DefinitionCompletedDate != null ? new XAttribute("DefinitionCompletedDate", DefinitionCompletedDate) : null,
                FullUpdateMasterStartedDate != null ? new XAttribute("FullUpdateMasterStartedDate", FullUpdateMasterStartedDate) : null,
                FullUpdateMasterCompletedDate != null ? new XAttribute("FullUpdateMasterCompletedDate", FullUpdateMasterCompletedDate) : null,
                FullUpdateReplicasCompletedDate != null ? new XAttribute("FullUpdateReplicasCompletedDate", FullUpdateReplicasCompletedDate) : null,
                new XElement(
                    "RankingGroups", 
                    SequenceRankingGroupKeyIndex.Select(
                        _ => new XElement(
                                "RankingGroup", 
                                new XAttribute("Sequence", _.Key), 
                                new XAttribute("Id", _.Value.Item1),
                                new XAttribute("Version", _.Value.Item2)))));
        }

        #endregion
    }
}