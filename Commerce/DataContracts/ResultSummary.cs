//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the possible results and of a service invocation and their explanations.
    /// </summary>
    [DataContract]
    public class ResultSummary
    {
        /// <summary>
        /// Gets or sets the ResultCode for this result.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the explanation for the result code.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "explanation")]
        public string Explanation { get; set; }
    }
}