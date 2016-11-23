//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal.DataModel
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class OffersEmailSchedule
    {
        [JsonProperty(PropertyName = "Url")]
        [DataMember(IsRequired = true, Name = "Url")]
        public string TemplateUrl { get; set; }

        [JsonProperty(PropertyName = "IsClo")]
        [DataMember(IsRequired = true, Name = "IsClo")]
        public bool IsCloUserTargeted { get; set; }

        [JsonProperty(PropertyName = "IsTest")]
        [DataMember(IsRequired = true, Name = "IsTestEmail")]
        public bool IsTestEmail { get; set; }

        [JsonProperty(PropertyName = "Metadata")]
        [DataMember(IsRequired = true, Name = "Metadata")]
        public string MetaData { get; set; }

        [JsonProperty(PropertyName = "ScheduleId")]
        [DataMember(IsRequired = true, Name = "ScheduleId")]
        public int ScheduleId { get; set; }

        [JsonProperty(PropertyName = "CampaignName")]
        [DataMember(IsRequired = true, Name = "CampaignName")]
        public string CampaignName { get; set; }
    }
}