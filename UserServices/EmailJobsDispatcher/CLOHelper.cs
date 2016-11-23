//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace DealsEmailDispatcher
{
    class GeocodePoint
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    struct Coordinates
    {
        public double CardLinkRegionTop;

        public double CardLinkRegionLeft;

        public double CardLinkRegionBottom;

        public double CardLinkRegionRight;
    }

    class CloLocationData
    {
        public string LocationName { get; set; }

        public Coordinates Coordinates { get; set; }
    }

    class CloGeocodepoint
    {
        public string LocationId { get; set; }

        public GeocodePoint GeocodePoint { get; set; }
    }

    public static class CloHelper
    {
        const string GeoCodeApi = "http://dev.virtualearth.net/REST/v1/Locations?query={0}&o=xml&include=queryParse&maxResults=1&key=Avp6aoktXdlNSkfjblIN-4rUyWFdcwZ0HNa2MPCfelOB5p6g1bZUGBs5HNAjve2Y";

        const string VeNameSpace = "{http://schemas.microsoft.com/search/local/ws/rest/v1}";

        private static readonly List<string> CloLocations = new List<string>();

        private static readonly List<string> NonCloLocations = new List<string>();

        private static List<CloGeocodepoint> cloGeocodepoints = new List<CloGeocodepoint>();

        private static readonly List<CloLocationData> CloLocationDatas = new List<CloLocationData>();

        public static Tuple<bool, string> IsCloRegion(string locationId)
        {
            if (!CloLocationDatas.Any())
            {
                BuildCoordinateTable();
            }

            Tuple<bool, string> tuple = new Tuple<bool, string>(false, string.Empty);
            if (locationId.Contains("us:postal:"))
            {
                locationId = locationId.Replace("us:postal:", "");
            }
            bool isCloRegion = CloLocations.Contains(locationId);
            
            if (isCloRegion)
            {
                foreach (CloGeocodepoint cloGeocodepoint in cloGeocodepoints)
                {
                    if (cloGeocodepoint.LocationId == locationId)
                    {
                        tuple = IsCloRegion(cloGeocodepoint.GeocodePoint.Latitude, cloGeocodepoint.GeocodePoint.Longitude);
                        break;
                    }
                }
            }

            if (!isCloRegion && !NonCloLocations.Contains(locationId))
            {
                GeocodePoint geocodePoint = GetGeoDataFromVirtualEarth(locationId);
                if (geocodePoint != null)
                {
                    tuple = IsCloRegion(geocodePoint.Latitude, geocodePoint.Longitude);
                    if (tuple.Item1)
                    {
                        cloGeocodepoints.Add(new CloGeocodepoint()
                            {
                                LocationId = locationId,
                                GeocodePoint = geocodePoint
                            });
                        CloLocations.Add(locationId);
                    }
                    else
                    {
                        NonCloLocations.Add(locationId);
                    }
                }
                else
                {
                    Console.WriteLine("Unable to determine Lat,Lon for location {0}", locationId);
                }
            }

            return tuple;
        }

        private static void BuildCoordinateTable()
        {
            Coordinates coordinates = new Coordinates()
                {
                    CardLinkRegionTop = 48.152344,

                    CardLinkRegionLeft = -123.239565,

                    CardLinkRegionBottom = 46.905246,

                    CardLinkRegionRight = -121.83722
                };
            CloLocationDatas.Add(new CloLocationData()
                {
                    LocationName = "Seattle",
                    Coordinates = coordinates
                });

            coordinates.CardLinkRegionTop = 33.8736;
            coordinates.CardLinkRegionLeft = -112.6092;
            coordinates.CardLinkRegionBottom = 32.9278;
            coordinates.CardLinkRegionRight = -111.1120;
            CloLocationDatas.Add(new CloLocationData()
            {
                LocationName = "Phoenix",
                Coordinates = coordinates
            });

            coordinates.CardLinkRegionTop = 42.7006;
            coordinates.CardLinkRegionLeft = -71.6236;
            coordinates.CardLinkRegionBottom = 41.9075;
            coordinates.CardLinkRegionRight = -70.4145;
            CloLocationDatas.Add(new CloLocationData()
            {
                LocationName = "Boston",
                Coordinates = coordinates
            });
        }

        private static Tuple<bool, string> IsCloRegion(double latitude, double longitude)
        {
            foreach (CloLocationData cloLocationData in CloLocationDatas)
            {
                bool isCloRegion = latitude >= cloLocationData.Coordinates.CardLinkRegionBottom &&
                              latitude <= cloLocationData.Coordinates.CardLinkRegionTop &&
                              longitude >= cloLocationData.Coordinates.CardLinkRegionLeft &&
                              longitude <= cloLocationData.Coordinates.CardLinkRegionRight;
                if (isCloRegion)
                {
                    return new Tuple<bool, string>(true, cloLocationData.LocationName);
                }
            }

            return new Tuple<bool, string>(false, string.Empty);
        }

        private static GeocodePoint GetGeoDataFromVirtualEarth(string geoString)
        {
            GeocodePoint geocodePoint = null;
            try
            {
                var geoServiceUrl = string.Format(GeoCodeApi, geoString);
                var client = new WebClient();
                var str = client.DownloadString(geoServiceUrl);
                var xml = XElement.Parse(str);

                geocodePoint = new GeocodePoint();
                var pointNode = xml.Descendants(VeNameSpace + "Point").FirstOrDefault();
                var latNode = pointNode != null ? pointNode.Descendants(VeNameSpace + "Latitude").FirstOrDefault() : null;
                var lonNode = pointNode != null ? pointNode.Descendants(VeNameSpace + "Longitude").FirstOrDefault() : null;
                if (latNode != null && lonNode != null)
                {
                    geocodePoint.Latitude = double.Parse(latNode.Value);
                    geocodePoint.Longitude = double.Parse(lonNode.Value);
                }

            }
            catch
            {
            }

            return geocodePoint;
        }


    }
}