//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the response to a generic Mastercard call.
    /// </summary>
    [DataContract(Namespace = "")]
    public class MasterCardAuthorizationResponse
    {
        /// <summary>
        /// Gets or sets the ResponseCode.
        /// </summary>
        [DataMember]
        public string ResponseCode { get; set; }
    }
}