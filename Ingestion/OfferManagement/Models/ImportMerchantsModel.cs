//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using OfferManagement.DataModel;

namespace OfferManagement.Models
{
    public class ImportMerchantsModel
    {
        public string ProviderId { get; set; }

        public string FileName { get; set; }

        public MerchantFileType MerchantFileType { get; set; }

        public string Author { get; set; }

        public override string ToString()
        {
            return $"{ProviderId};{FileName};{MerchantFileType}";
        }
    }
}