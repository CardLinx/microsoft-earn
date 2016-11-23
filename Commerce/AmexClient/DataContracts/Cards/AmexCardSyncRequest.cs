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
    public class AmexCardSyncRequest
    {
        public AmexCardSyncRequest()
        {
            MessageId = AmexConstants.MessageId;
            PartnerId = AmexConstants.PartnerId;
            DistributionChannel = AmexConstants.DistributionChannel;
            LanguageCode = AmexConstants.LanguageCode;
            CountyCode = AmexConstants.CountryCode;
            DiscountIndicator = AmexConstants.DiscountIndicator;
        }

        /// <summary>
        /// Gets or sets Message Id
        /// *** Required ***
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "msgId")]
        [JsonProperty(PropertyName = "msgId")]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets Partner Id
        /// *** Required ***
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "partnerId")]
        [JsonProperty(PropertyName = "partnerId")]
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets Language Code
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "langCd")]
        [JsonProperty(PropertyName = "langCd")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets County Code
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "ctryCd")]
        [JsonProperty(PropertyName = "ctryCd")]
        public string CountyCode { get; set; }

        /// <summary>
        /// Gets or sets Distribution Channel
        /// *** Required ***
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "distrChan")]
        [JsonProperty(PropertyName = "distrChan")]
        public string DistributionChannel { get; set; }

        /// <summary>
        /// Gets or sets Discount Indicator
        /// *** Required ***
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "discInd")]
        [JsonProperty(PropertyName = "discInd")]
        public string DiscountIndicator { get; set; }

        /// <summary>
        /// Gets or sets Card Number
        /// *** Required ***
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "cardNbr")]
        [JsonProperty(PropertyName = "cardNbr")]
        public string CardNumber { get; set; }

        ///// <summary>
        ///// Gets or sets Card Member First Name
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "cmFirstNm")]
        //[JsonProperty(PropertyName = "cmFirstNm")]
        //public string CardMemberFirstName { get; set; }

        ///// <summary>
        ///// Gets or sets Card Member Last Name
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "cmLastNm")]
        //[JsonProperty(PropertyName = "cmLastNm")]
        //public string CardMemberLastName { get; set; }

        ///// <summary>
        ///// Gets or sets Card Member Email
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "cmEmailId")]
        //[JsonProperty(PropertyName = "cmEmailId")]
        //public string CardMemberEmail { get; set; }

        ///// <summary>
        ///// Gets or sets Card Member Contact Number
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "cmContantNo")]
        //[JsonProperty(PropertyName = "cmContantNo")]
        //public string CardMemberContactNumber { get; set; }

        /// <summary>
        /// Gets or sets Card Member Alias 1
        /// *** Required ***
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "cmAlias1")]
        [JsonProperty(PropertyName = "cmAlias1")]
        public string CardToken1 { get; set; }

        ///// <summary>
        ///// Gets or sets Card Member Alias 2
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "cmAlias2")]
        //[JsonProperty(PropertyName = "cmAlias2")]
        //public string CardToken2 { get; set; }

        ///// <summary>
        ///// Gets or sets Card Member Alias 3
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "cmAlias3")]
        //[JsonProperty(PropertyName = "cmAlias3")]
        //public string CardToken3 { get; set; }

        ///// <summary>
        ///// Gets or sets Global Push Notification Opting Indicator
        ///// </summary>
        //[DataMember(EmitDefaultValue = false, Name = "glblOptPushNt")]
        //[JsonProperty(PropertyName = "glblOptPushNt")]
        //public string GlobalPushNotificationOptingIndicator { get; set; }
    }
}