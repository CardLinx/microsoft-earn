//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using CsvHelper.Configuration;
using OfferManagement.DataModel.Partners.Visa;

namespace VisaMids.Console.Map
{
    public sealed class OfferMap : CsvClassMap<VisaOffer>
    {
        public OfferMap()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
            Map(m => m.CampaignName).Name("CampaignName");
            Map(m => m.CountryCode).Name("CountryCode");
            Map(m => m.Template).Name("Template");
            //Map(m => m.Merchants).ConvertUsing(row => new List<OfferManagement.DataModel.Partners.Visa.VisaMerchant>
            //        {
            //            new OfferManagement.DataModel.Partners.Visa.VisaMerchant
            //            {
            //                Mid = row.GetField<string>("Mid"),
            //                Sid = row.GetField<string>("Sid")

            //            }
            //        }
            //);
        }
    }
}