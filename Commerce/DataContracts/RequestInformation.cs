//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents information from the server regarding the request.
    /// </summary>
    [DataContract]
    public class RequestInformation
    {
        /// <summary>
        /// Gets or sets the RequestID for this request.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "request_id")]
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the server that responded to the request.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "server_id")]
        public string ServerId { get; set; }
    }
}