//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using CsvHelper.Configuration;
using OfferManagement.DataModel;

namespace VisaMids.Console.Map
{
    public sealed class MerchantMap : CsvClassMap<Merchant>
    {
        public MerchantMap()
        {
            Map(m => m.Name).Name("Name");
            References<LocationMap>(m => m.Location);

         }
    }

    public sealed class LocationMap : CsvClassMap<Location>
    {
        public LocationMap()
        {
            Map(m => m.Address).Name("Street Address");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.Zip).Name("Zip");
        }
    }
}