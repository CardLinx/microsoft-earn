//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;

namespace Earn.DataContract.Commerce
{
    public class IssueCreditsRequest
    {
        public Guid UserId { get; set; }

        public string Issuer { get; set; }

        public double Amount { get; set; }

        public string Explanation { get; set; }
    }
}