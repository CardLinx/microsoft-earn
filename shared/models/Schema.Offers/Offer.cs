//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ProtoBuf;
    using System;

    [DataContract]
    [ProtoContract]
    public class Offer
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [ProtoMember(1)]
        public string Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "status")]
        [ProtoMember(2)]
        public Status Status { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "description")]
        [ProtoMember(3)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "end_time")]
        [ProtoMember(4)]
        public DateTime EndTime { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "start_time")]
        [ProtoMember(5)]
        public DateTime StartTime { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "price")]
        [ProtoMember(6)]
        public double Price { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "title")]
        [ProtoMember(7)]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "transaction_url")]
        [ProtoMember(8)]
        public string TransactionUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "impression_cap")]
        [ProtoMember(9)]
        public int ImpressionCap { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "weight")]
        [ProtoMember(10)]
        public double Weight { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "conversion_value")]
        [ProtoMember(11)]
        public double ConversionValue { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "segments")]
        [ProtoMember(12)]
        public List<int> Segments { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "recommendation_type")]
        [ProtoMember(13)]
        public RecommendationType RecommendationType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "catalog")]
        [ProtoMember(14)]
        public Catalog Catalog { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "group_name")]
        [ProtoMember(15)]
        public string GroupName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "supported_apps")]
        [ProtoMember(16)]
        public List<SupportedApp> SupportedApps { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "reward_details")]
        [ProtoMember(17)]
        public RewardDetails RewardDetails { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "conversion")]
        [ProtoMember(18)]
        public Conversion Conversion { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "impression_cap_per_user")]
        [ProtoMember(19)]
        public int ImpressionCapPerUser { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "campaign_id")]
        [ProtoMember(20)]
        public int CampaignId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "feature_id")]
        [ProtoMember(21)]
        public int FeatureId { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "experiment_bucket_index")]
        [ProtoMember(23)]
        public int ExperimentBucketIndex { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "modified_by")]
        [ProtoMember(24)]
        public string ModifiedBy { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "modified_date")]
        [ProtoMember(25)]
        public DateTime ModifiedDate { get; set; }
    }
}