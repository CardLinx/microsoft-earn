//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents Amex Auth Response.
    /// </summary>
    [DataContract(Name = "AuthorizationData", Namespace = "")]
    public class AmexAuthResponse
    {
        /// <summary>
        /// Gets or sets the ResponseCode.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "ResponseCode")]
        public string ResponseCode { get; set; }
    }
}