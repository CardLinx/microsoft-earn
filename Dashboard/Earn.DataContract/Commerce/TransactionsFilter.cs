//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;

namespace Earn.DataContract.Commerce
{
    public class TransactionsFilter
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<int> TransactionTypes { get; set; }

        public List<int> CardBrandIds { get; set; }

        public string Last4Digits { get; set; }

        public string MerchantName { get; set; }

        public Guid? UserId { get; set; }
    }
}