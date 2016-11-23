//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class Merchant
    {
        [DataMember(EmitDefaultValue = false, Name = "id")]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "name")]
        public string Name { get; set; }
    }
}