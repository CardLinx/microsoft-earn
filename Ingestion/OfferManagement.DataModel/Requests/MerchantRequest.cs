//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OfferManagement.DataModel
{
    [DataContract]
    public class MerchantRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public Dictionary<string,string> ExtendedAttributes { get; set; }

        public IList<Payment> Payments { get; set; }
    }
}