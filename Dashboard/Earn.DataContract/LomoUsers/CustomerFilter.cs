//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Earn.DataContract.LomoUsers
{
    public class CustomerFilter
    {
        public string Email { get; set; }

        public string MSIDorPUID { get; set; }

        public Guid? UserId { get; set; }
    }
}