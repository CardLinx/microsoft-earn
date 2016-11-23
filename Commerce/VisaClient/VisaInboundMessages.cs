//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.VisaClient
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Response to Visa
    /// </summary>
    [DataContract]
    public class VisaEpmResponse
    {
        /// <summary>
        /// ErrorMessage text
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "error_msg")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// StatusCode number
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "status_code")]
        public string StatusCode { get; set; }
    }

    /// <summary>
    ///  Generated using Json2CSharp
    /// </summary>
    public class MessageElementsCollection
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Generated using Json2CSharp
    /// </summary>
    public class UserDefinedFieldsCollection
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Generated using Json2CSharp
    /// </summary>
    public class EndPointMessageRequest
    {
        public string CardId { get; set; }
        public string ExternalUserId { get; set; }
        public List<MessageElementsCollection> MessageElementsCollection { get; set; }
        public string MessageId { get; set; }
        public string MessageName { get; set; }
        public List<UserDefinedFieldsCollection> UserDefinedFieldsCollection { get; set; }
        public string UserProfileId { get; set; }
    }
}