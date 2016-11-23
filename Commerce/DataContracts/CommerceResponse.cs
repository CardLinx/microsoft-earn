//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Abstract base class for Commerce API response objects.
    /// </summary>
    [DataContract]
    public abstract class CommerceResponse
    {
        /// <summary>
        /// Gets or sets the RequestInformation object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "request_information")]
        public RequestInformation RequestInformation { get; set; }

        /// <summary>
        /// Gets or sets the ResultSummary object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "result_summary")]
        public ResultSummary ResultSummary { get; set; }
    }
}