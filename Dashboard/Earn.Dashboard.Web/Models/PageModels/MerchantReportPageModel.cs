//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using Earn.DataContract.Commerce;

namespace Earn.Dashboard.Web.Models.PageModels
{
    public class MerchantReportPageModel
    {
        public IEnumerable<CardBrand> CardBrands { get; set; }

        public IEnumerable<TransactionType> TransactionTypes { get; set; }

        public IEnumerable<string> Merchants { get; set; }
    }
}