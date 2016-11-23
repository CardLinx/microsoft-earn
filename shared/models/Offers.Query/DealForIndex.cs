//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Abbreviated deal for indexing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.DataModels.Offers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DotM.DataContracts;
    using DotM.DataContracts.Constants;
    using Lomo.GeoSpatial;
    /// <summary>
    /// The deal status.
    /// </summary>
    public enum Status : byte
    {
        /// <summary>
        /// Th active
        /// </summary>
        Active = 200,

        /// <summary>
        /// Deal with no valid images
        /// </summary>
        NoImage = 100,

        /// <summary>
        /// Sold out
        /// </summary>
        SoldOut = 90,

        /// <summary>
        /// The pending
        /// </summary>
        Pending = 80,

        /// <summary>
        /// The expired
        /// </summary>
        Expired = 70,

        /// <summary>
        /// The deleted
        /// </summary>
        Deleted = 60,

        /// <summary>
        /// Deal has inappropriate words specified in the policy check blacklist
        /// </summary>
        PolicyCheckIssue = 50,

        /// <summary>
        /// The invalid
        /// </summary>
        Invalid = 0
    }

    /// <summary>
    /// Deal data needed for indexes 
    /// </summary>
    public class DealForIndex : IRankable
    {
        /// <summary>
        /// Gets or sets a value indicating whether in-memory keywords index is on.
        /// </summary>
        public static volatile bool KeywordsOn = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DealForIndex" /> class.
        /// </summary>
        /// <param name="deal">dotm deal</param>
        public DealForIndex(Deal deal)
        {
            Id = Guid.Parse(deal.Id);
            ProviderDealId = deal.ProviderDealId;
            ProviderId = deal.DealProvider.ProviderId;

            if (deal.StartTime != null)
            {
                StartTime = DateTime.Parse(deal.StartTime).ToUniversalTime();
            }

            if (deal.EndTime != null)
            {
                EndTime = DateTime.Parse(deal.EndTime).ToUniversalTime();
            }

            StatusByte = (byte)FromStatusId(deal.DealStatusId);

            WeightedRanks = deal.DealRank.WeightedRanks;
            WeightedRanks1 = deal.DealRank.WeightedRanks1;
            PublishingVersions = deal.DealRank.PublishingVersions ?? new[] { 0, 0 };
            if (PublishingVersions.Length < 2)
            {
                if (PublishingVersions.Length == 1)
                {
                    PublishingVersions = new[] { PublishingVersions[0], 0 };
                }
                else
                {
                    PublishingVersions = new[] { 0, 0 };
                }
            }

            BusinessId = deal.Business.BusinessId;
            BusinessIds = deal.Business.Locations != null
                        ? deal.Business.Locations.Select(_ => _.BusinessId).Where(_ => !string.IsNullOrEmpty(_)).Concat(new[] { BusinessId }).Distinct().ToList() 
                        : new List<string>();
            if (BusinessIds.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentNullException(string.Format("Deal with provider deal id = {0} has null id", deal.ProviderDealId));
            }

            Categories = deal.Categories ?? new List<Category>();

            // don't include CN in the keyword index
            Keywords = (deal.Business.Locations != null && deal.Business.Locations.Any(_ => _.CountryOrRegion == "CN") 
                         ? null 
                         : ((KeywordsOn && deal.Keywords != null) ? new HashSet<string>(deal.Keywords) : null)
                       )  
                       ?? new HashSet<string>();

            Locations = deal.Business.Locations != null && deal.LocationType == LocationType.BusinessAddress ? deal.Business.Locations.Select(_ => new Point(_.Latitude, _.Longitude)).ToList() : new List<Point>();
            HasSingleLocation = Locations.Count() == 1;
            if (HasSingleLocation)
            {
                var loc = Locations.First();
                Lat = loc.Latitude;
                Lon = loc.Longitude;
                Loc = loc;
            }

            TileIds = Locations.Select(_ => new Tile(_).Id).ToList();

            SeoFullUrl = deal.SeoUrl != null ? deal.SeoUrl.FullUrl : null;
        }

        #region Public Properties

        /// <summary>
        /// Gets Id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the Deal Id assigned by the deal provider
        /// This field is used during ingestion and should not be used for any other purpose.
        /// The value in this field is a compound value comprising a provider prefix followed by the id of the deal, as given by the provider.
        /// </summary>
        public string ProviderDealId { get; private set; }

        /// <summary>
        /// Gets DealProvider.
        /// </summary>
        public string ProviderId { get; private set; }

        /// <summary>
        /// Gets BusinessId
        /// </summary>
        public string BusinessId { get; private set; }
        
        /// <summary>
        /// Gets BusinessIds (includes BusinessId)
        /// </summary>
        public List<string> BusinessIds { get; private set; }

        /// <summary>
        /// Gets the weighted ranks array 0 for each deal
        /// </summary>
        public byte[] WeightedRanks { get; private set; }

        /// <summary>
        /// Gets the weighted ranks array 1 for each deal
        /// </summary>
        public byte[] WeightedRanks1 { get; private set; }

        /// <summary>
        /// Gets corresponding to weighted ranks slots publishing versions
        /// </summary>
        public int[] PublishingVersions { get; private set; }

        /// <summary>
        ///     Gets Status.
        /// </summary>
        public byte StatusByte { get; private set; }

        /// <summary>
        ///     Gets StartTime.
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        ///     Gets EndTime.
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// Gets the locations.
        /// </summary>
        public List<Point> Locations { get; private set; }

        /// <summary>
        /// Gets the lat of single location.
        /// </summary>
        public double Lat { get; private set; }

        /// <summary>
        /// Gets the lon of single location.
        /// </summary>
        public double Lon { get; private set; }

        /// <summary>
        /// Gets the Point of single location.
        /// </summary>
        public Point Loc { get; private set; }

        /// <summary>
        /// Gets a value indicating whether deal has single location.
        /// </summary>
        public bool HasSingleLocation { get; private set; }

        /// <summary>
        ///     Gets TileIds.
        /// </summary>
        public IEnumerable<int> TileIds { get; private set; }

        /// <summary>
        /// Gets Category
        /// </summary>
        public List<Category> Categories { get; private set; }

        /// <summary>
        /// Gets SeoUrl
        /// </summary>
        public string SeoFullUrl { get; private set; }

        /// <summary>
        /// Gets keywords
        /// </summary>
        public HashSet<string> Keywords { get; set; }

        #endregion

        /// <summary>
        /// Converts status to status id
        /// </summary>
        /// <param name="status">deal status</param>
        /// <returns>deal status id</returns>
        public static string ToStatusId(Status status)
        {
            string statusId;
            switch (status)
            {
                case Status.Active:
                    statusId = DealStatus.Active;
                    break;
                case Status.NoImage:
                    statusId = DealStatus.NoImage;
                    break;
                case Status.SoldOut:
                    statusId = DealStatus.SoldOut;
                    break;
                case Status.Pending:
                    statusId = DealStatus.Pending;
                    break;
                case Status.Expired:
                    statusId = DealStatus.Expired;
                    break;
                case Status.Deleted:
                    statusId = DealStatus.Deleted;
                    break;
                case Status.PolicyCheckIssue:
                    statusId = DealStatus.PolicyCheckIssue;
                    break;
                case Status.Invalid:
                    statusId = DealStatus.Invalid;
                    break;
                default:
                    statusId = DealStatus.Invalid;
                    break;
            }

            return statusId;
        }

        #region IRankable

        /// <summary>
        /// Returns deal Id.
        /// </summary>
        /// <returns>deal Id.</returns>
        public Guid GetId()
        {
            return Id;
        }

        /// <summary>
        /// Gets deal end time
        /// </summary>
        /// <returns>the end time</returns>
        public DateTime? GetEndTime()
        {
            return EndTime;
        }

        /// <summary>
        /// Gets deal start time
        /// </summary>
        /// <returns>the start time</returns>
        public DateTime? GetStartTime()
        {
            return StartTime;
        }

        /// <summary>
        /// Gets default rank for query context
        /// </summary>
        /// <param name="qc">the query context</param>
        /// <returns>the default rank</returns>
        public byte GetRank(QueryContext qc)
        {
            return WeightedRank.GetRank(
                qc.Ranking.DefaultRankingGroupSequence,
                qc.Ranking.RankArraySlot == 0 ? WeightedRanks : WeightedRanks1 ?? WeightedRanks,
                qc.Ranking.RankArraySlot == 0 ? PublishingVersions[0] : PublishingVersions[1],
                qc.Ranking.PublishingVersion);
        }

        #endregion

        /// <summary>
        /// Converts status id to status
        /// </summary>
        /// <param name="statusId">deal status id</param>
        /// <returns>deal status</returns>
        private Status FromStatusId(string statusId)
        {
            Status status;
            if (string.IsNullOrWhiteSpace(statusId))
            {
                status = Status.Active;
            }
            else
            {
                switch (statusId.ToLower())
                {
                    case DealStatus.Active:
                        status = Status.Active;
                        break;
                    case DealStatus.NoImage:
                        status = Status.NoImage;
                        break;
                    case DealStatus.SoldOut:
                        status = Status.SoldOut;
                        break;
                    case DealStatus.Pending:
                        status = Status.Pending;
                        break;
                    case DealStatus.Expired:
                        status = Status.Expired;
                        break;
                    case DealStatus.Deleted:
                        status = Status.Deleted;
                        break;
                    case DealStatus.PolicyCheckIssue:
                        status = Status.PolicyCheckIssue;
                        break;
                    case DealStatus.Invalid:
                        status = Status.Invalid;
                        break;
                    default:
                        status = Status.Invalid;
                        break;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets deal BusinessId
        /// </summary>
        /// <returns>the business id</returns>
        public string GetBusinessId()
        {
            return BusinessId;
        }
    }
}