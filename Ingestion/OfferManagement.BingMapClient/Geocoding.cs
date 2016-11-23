//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Lomo.Logging;

namespace OfferManagement.BingMapClient
{
    public class Geocoding
    {
        public class Point
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
        }

        public class Address
        {
            public string addressLine { get; set; }
            public string adminDistrict { get; set; }
            public string adminDistrict2 { get; set; }
            public string countryRegion { get; set; }
            public string formattedAddress { get; set; }
            public string locality { get; set; }
            public string postalCode { get; set; }
        }

        public class GeocodePoint
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
            public string calculationMethod { get; set; }
            public List<string> usageTypes { get; set; }
        }

        public class Resource
        {
            public string __type { get; set; }
            public List<double> bbox { get; set; }
            public string name { get; set; }
            public Point point { get; set; }
            public Address address { get; set; }
            public string confidence { get; set; }
            public string entityType { get; set; }
            public List<GeocodePoint> geocodePoints { get; set; }
            public List<string> matchCodes { get; set; }
        }

        public class ResourceSet
        {
            public int estimatedTotal { get; set; }
            public List<Resource> resources { get; set; }
        }

        public class Response
        {
            public string authenticationResultCode { get; set; }
            public string brandLogoUri { get; set; }
            public string copyright { get; set; }
            public List<ResourceSet> resourceSets { get; set; }
            public int statusCode { get; set; }
            public string statusDescription { get; set; }
            public string traceId { get; set; }
        }

        private const string BingGeocodeUrl = "http://dev.virtualearth.net/REST/v1/Locations?CountryRegion=US&adminDistrict={0}" +
                                              "&postalCode={1}&addressLine={2}&locality={3}&key=TODO_BINGMAPS_API_KEY_HERE";
        
        public static Resource GetLocation(string address, string state, string zip, string city = null)
        {
            Resource response = null;

            try
            {
                using (var client = new WebClient())
                {
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        address = Uri.EscapeDataString(address);
                    }
                    var fullurl = string.Format(BingGeocodeUrl, state, zip, address, city);
                    var responseString = client.DownloadString(fullurl);

                    {
                        var responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                        if (responseObj.statusCode != 200) return null;
                        if (responseObj.resourceSets.Count > 0)
                        {
                            if (responseObj.resourceSets.ElementAt(0).resources.Count > 0)
                                response = responseObj.resourceSets.ElementAt(0).resources.ElementAt(0);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(e, "Error in geocoding");
            }

            return response;
        }

        public static async Task<Resource> GetLocationAsync(string address, string state, string zip, string city = null)
        {
            Resource response = null;

            using (var client = new WebClient())
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    address = Uri.EscapeDataString(address);
                }
                var fullurl = string.Format(BingGeocodeUrl, state, zip, address, city);
                var responseString = await client.DownloadStringTaskAsync(fullurl);

                {
                    var responseObj = JsonConvert.DeserializeObject<Response>(responseString);
                    if (responseObj.statusCode != 200) return null;
                    if (responseObj.resourceSets.Count > 0)
                    {
                        if (responseObj.resourceSets.ElementAt(0).resources.Count > 0)
                            response = responseObj.resourceSets.ElementAt(0).resources.ElementAt(0);
                    }
                }
            }

            return response;
        }

        //public Resource response { get; set; }
    }
}