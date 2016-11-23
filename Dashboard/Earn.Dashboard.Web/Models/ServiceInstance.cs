//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;

namespace Earn.Dashboard.Web.Models
{
    public class ServiceInstance
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Status { get; set; }
        public string Endpoint { get; set; }
        public List<ServiceInstance> Children { get; set; }
    }
}