//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    public class RewardNetworkFeedInformation : FeedInformation
    {
        [DataMember(EmitDefaultValue = false, Name = "feed_folder")]
        [JsonProperty("feed_folder")]
        public string FeedFolder { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "api_key")]
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "auth_token_url")]
        [JsonProperty("auth_token_url")]
        public string AuthTokenUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "feed_file_name")]
        [JsonProperty("feed_file_name")]
        public string FeedFileName { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "merchant_info_url")]
        [JsonProperty("merchant_info_url")]
        public string MerchantInfoUrl { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "image_url")]
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}