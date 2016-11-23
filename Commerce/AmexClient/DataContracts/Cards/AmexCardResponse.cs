//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents Amex Auth Request.
    /// </summary>
    [DataContract]
    public class AmexCardResponse
    {
        /// <summary>
        /// Gets or sets Correlation Id
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "correlationId")]
        [JsonProperty(PropertyName = "correlationId")]
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets Status
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "status")]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Response Code
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "respCd")]
        [JsonProperty(PropertyName = "respCd")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets Response Description
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "respDesc")]
        [JsonProperty(PropertyName = "respDesc")]
        public string ResponseDescription { get; set; }
        
        /// <summary>
        /// Gets or sets Card Member Alias 1
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "cmAlias1")]
        [JsonProperty(PropertyName = "cmAlias1")]
        public string CardToken1 { get; set; }

        /// <summary>
        /// Gets or sets Card Member Alias 2
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "cmAlias2")]
        [JsonProperty(PropertyName = "cmAlias2")]
        public string CardToken2 { get; set; }
    }
}