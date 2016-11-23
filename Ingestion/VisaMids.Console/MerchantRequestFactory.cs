//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using OfferManagement.DataModel;
using VisaMids.Console.Map;

namespace VisaMids.Console
{
    public class MerchantFactory
    {
        public static IList<Merchant> LoadFeatures(string featureFileName)
        {
            using (var fs = File.OpenRead(featureFileName))
            {
                using (var sr = new StreamReader(fs))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.RegisterClassMap<MerchantMap>();
                    csv.Configuration.TrimFields = true;
                    csv.Configuration.TrimHeaders = true;
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.IgnoreBlankLines = true;
                    var records = csv.GetRecords<Merchant>().ToList();
                    return records;
                }
            }

        }
    }
}