//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The geo spatial.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.GeoSpatial
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Xml.Linq;

    using Newtonsoft.Json;

    /// <summary>
    ///     The geo spatial.
    /// </summary>
    public static class GeoSpatial
    {
        #region Constants

        /// <summary>
        /// Translates degrees to meters
        /// </summary>
        public const double MetersPerDegree = Math.PI * EarthRadius / 180.0;

        /// <summary>
        /// Earth Radius in meters
        /// </summary>
        private const double EarthRadius = 6378137;

        /// <summary>
        ///     The mile to meters.
        /// </summary>
        private const double MetersPerMile = 1609.344;

        /// <summary>
        /// VE name space
        /// </summary>
        private const string VENameSpace = "{http://schemas.microsoft.com/search/local/ws/rest/v1}";

        /// <summary>
        /// Reverse geo coding API
        /// </summary>
        private const string ReverseGeoCodeAPI = "http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?o=xml&key=Ai3JT7jY__YvjMfBzBwE_yZN2jWMzSFfRwLcIQLQw1uTiTFJPHCWVEq6BraHwp22";

        /// <summary>
        /// Geo coding API 
        /// </summary>
        private const string GeoCodeAPI = "http://dev.virtualearth.net/REST/v1/Locations?query={0}&o=xml&include=queryParse&maxResults=1&key=Avp6aoktXdlNSkfjblIN-4rUyWFdcwZ0HNa2MPCfelOB5p6g1bZUGBs5HNAjve2Y";

        /// <summary>
        /// LoMoLobby API 
        /// </summary>
        private const string LoMoLobbyApi = "http://platform.bing.com/LoMo?q={0}";

        #endregion

        #region Enums

        /// <summary>
        ///     The geo distance unit.
        /// </summary>
        public enum GeoDistanceUnit
        {
            /// <summary>
            ///     Length measurement in the US and UK, in miles and feet
            /// </summary>
            GeoDistanceImperial = 1,

            /// <summary>
            ///     International Standard Unit
            /// </summary>
            GeoDistanceMetricSystem = 2,

            /// <summary>
            ///     Non-SI units
            /// </summary>
            GeoDistanceNonSI = 3
        }

        /// <summary>
        /// Source API to query for location co ordinates
        /// </summary>
        public enum GeoSource
        {
            /// <summary>
            /// Virtual Earth endpoint
            /// </summary>
            VirtualEarth = 1,

            /// <summary>
            /// Lobby Service endpoint
            /// </summary>
            LoMoLobby = 2
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// This function converts distance in meters to miles
        /// </summary>
        /// <param name="distance">
        /// decimal degrees 
        /// </param>
        /// <returns>
        /// distance in miles 
        /// </returns>
        public static double FromMetersToMiles(double distance)
        {
            return distance / MetersPerMile;
        }

        /// <summary>
        /// This function converts distance in miles to meters
        /// </summary>
        /// <param name="distance">
        /// distance in miles 
        /// </param>
        /// <returns>
        /// distance in meters 
        /// </returns>
        public static double FromMilesToMeters(double distance)
        {
            return distance * MetersPerMile;
        }

        /// <summary>
        /// Returns the radius for the location based on the bounding box
        /// </summary>
        /// <param name="boundingBox">Bounding box</param>
        /// <returns>the radius</returns>
        public static double GetRadiusFromBoundingBox(BoundingBox boundingBox)
        {
            var distanceMeters = GetGeoDistanceMeters(boundingBox.SouthLatitude, boundingBox.WestLongitude, boundingBox.NorthLatitude, boundingBox.EastLongitude);
            return 0.5 * distanceMeters;
        }

        /// <summary>
        /// Returns approximate distance between two points in square degrees
        /// </summary>
        /// <param name="point">the point</param>
        /// <param name="point0">the point 0</param>
        /// <param name="cosLat0Sqr">the square(cos(lat0))</param>
        /// <returns>distance between two points in square degrees</returns>
        public static double PlaneDistanceSqrDegree(Point point, Point point0, double? cosLat0Sqr = null)
        {
            double cosLatSqr;
            if (cosLat0Sqr == null)
            {
                var cosLat = Math.Cos(point0.Latitude * Math.PI / 180);
                cosLatSqr = cosLat * cosLat;
            }
            else
            {
                cosLatSqr = cosLat0Sqr.Value;
            }

            var deltaLat = point.Latitude - point0.Latitude;
            var deltaLon = point.Longitude - point0.Longitude;
            return (deltaLat * deltaLat) + (deltaLon * deltaLon * cosLatSqr);
        }

        /// <summary>
        /// Returns approximate distance between two points in square degrees
        /// </summary>
        /// <param name="point">the point</param>
        /// <param name="point0">the point 0</param>
        /// <param name="cosLat0Sqr">the square(cos(lat0))</param>
        /// <returns>distance between two points in square degrees</returns>
        public static double PlaneDistanceSqrDegree(Point point, Point point0, double cosLat0Sqr)
        {
            var deltaLat = point.Latitude - point0.Latitude;
            var deltaLon = point.Longitude - point0.Longitude;
            return (deltaLat * deltaLat) + (deltaLon * deltaLon * cosLat0Sqr);
        }

        /// <summary>
        /// Returns approximate distance between two points in square degrees
        /// </summary>
        /// <param name="lat">the latitude</param>
        /// <param name="lon">the longitude</param>
        /// <param name="lat0">the latitude 0</param>
        /// <param name="lon0">the longitude 0</param>
        /// <param name="cosLat0Sqr">the square(cos(lat0))</param>
        /// <returns>distance between two points in square degrees</returns>
        public static double PlaneDistanceSqrDegree(double lat, double lon, double lat0, double lon0, double? cosLat0Sqr = null)
        {
            double cosLatSqr;
            if (cosLat0Sqr == null)
            {
                var cosLat = Math.Cos(lat0 * Math.PI / 180);
                cosLatSqr = cosLat * cosLat;
            }
            else
            {
                cosLatSqr = cosLat0Sqr.Value;
            }

            var deltaLat = lat - lat0;
            var deltaLon = lon - lon0;
            return (deltaLat * deltaLat) + (deltaLon * deltaLon * cosLatSqr);
        }

        /// <summary>
        /// Returns approximate distance between two points in square degrees
        /// </summary>
        /// <param name="lat">the latitude</param>
        /// <param name="lon">the longitude</param>
        /// <param name="lat0">the latitude 0</param>
        /// <param name="lon0">the longitude 0</param>
        /// <param name="cosLat0Sqr">the square(cos(lat0))</param>
        /// <returns>distance between two points in square degrees</returns>
        public static double PlaneDistanceSqrDegree(double lat, double lon, double lat0, double lon0, double cosLat0Sqr)
        {
            var deltaLat = lat - lat0;
            var deltaLon = lon - lon0;
            return (deltaLat * deltaLat) + (deltaLon * deltaLon * cosLat0Sqr);
        }

        /// <summary>
        /// Returns approximate distance between two points in meters
        /// </summary>
        /// <param name="point">the point</param>
        /// <param name="point0">the point 0</param>
        /// <param name="cosLat0Sqr">the square(cos(lat0))</param>
        /// <returns>distance between two points in meters</returns>
        public static double PlaneDistanceMeters(Point point, Point point0, double? cosLat0Sqr = null)
        {
            var distSqrDegree = PlaneDistanceSqrDegree(point, point0, cosLat0Sqr);
            return Deg2Rad(Math.Sqrt(distSqrDegree)) * EarthRadius;
        }

        /// <summary>
        /// Returns approximate distance between two points in meters
        /// </summary>
        /// <param name="lat">the latitude</param>
        /// <param name="lon">the longitude</param>
        /// <param name="lat0">the latitude 0</param>
        /// <param name="lon0">the longitude 0</param>
        /// <param name="cosLat0Sqr">the square(cos(lat0))</param>
        /// <returns>distance between two points in meters</returns>
        public static double PlaneDistanceMeters(double lat, double lon, double lat0, double lon0, double? cosLat0Sqr = null)
        {
            var distSqrDegree = PlaneDistanceSqrDegree(lat, lon, lat0, lon0, cosLat0Sqr);
            return Deg2Rad(Math.Sqrt(distSqrDegree)) * EarthRadius;
        }

        /// <summary>
        /// Returns the distance between the two input points in meters.
        /// </summary>
        /// <param name="lat1">Point 1 Latitude</param>
        /// <param name="lon1">Point 1 Longitude</param>
        /// <param name="lat2">Point 2 Latitude</param>
        /// <param name="lon2">Point 2 Longitude</param>
        /// <returns>The System.Double.</returns>
        public static double GetGeoDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var distanceKm = GetGeoDistance(lat1, lon1, lat2, lon2, GeoDistanceUnit.GeoDistanceMetricSystem);
            return distanceKm * 1000.0;
        }

        /// <summary>
        /// Returns the distance between the two input points in meters.
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <returns>The System.Double.</returns>
        public static double GetGeoDistanceMeters(Point point1, Point point2)
        {
            var distanceKm = GetGeoDistance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude, GeoDistanceUnit.GeoDistanceMetricSystem);
            return distanceKm * 1000.0;
        }

        /// <summary>
        /// Returns the distance between the two input points in miles.
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <returns>The System.Double.</returns>
        public static double GetGeoDistanceMiles(Point point1, Point point2)
        {
            return GetGeoDistance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude, GeoDistanceUnit.GeoDistanceImperial);
        }

        /// <summary>
        /// Splits a comma-delimited latlon string into separate lat and lon values.
        /// </summary>
        /// <param name="ll">
        /// Comma-separated pair of doubles representing lat/lon. E.g. 47.7,-122.2 
        /// </param>
        /// <param name="lat">
        /// Latitude parsed from the input (assumed to be the first double) 
        /// </param>
        /// <param name="lon">
        /// Longitude parsed from the input (assumed to be the second double) 
        /// </param>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        public static bool ParseLatLon(string ll, out double lat, out double lon)
        {
            lat = 0.0;
            lon = 0.0;

            if (string.IsNullOrWhiteSpace(ll))
            {
                return false;

                // throw new ArgumentException("Cannot pass empty lat long", "ll");
            }

            var splits = ll.Split(',');
            if (splits.Count() != 2)
            {
                return false;

                // throw new ArgumentException(
                // "Error parsing input ll '" + ll + "'.  Expecting a comma-delimited number pair (e.g. '47.6,-122.2')");
            }

            if (!double.TryParse(splits[0], out lat))
            {
                return false;

                // throw new ArgumentException(
                // "Error processing double value " + splits[0]
                // + ".  Expecting a double-valued latitude parameter (e.g. 47.6)");
            }

            if (!double.TryParse(splits[1], out lon))
            {
                return false;

                // throw new ArgumentException(
                // "Error processing double value " + splits[1]
                // + ".  Expecting a double-valued longitude parameter (e.g. -122.2)");
            }

            return true;
        }

        /// <summary>
        /// Gets the geographic co ordinates and radius for a location by querying a GeoSource endpoint
        /// </summary>
        /// <param name="geoString">Location name</param>
        /// <param name="geoSource">Geosource to query</param>
        /// <returns>the geo point</returns>
        public static GeocodePoint GetGeoData(string geoString, GeoSource geoSource)
        {
            GeocodePoint geocodePoint = null;

            switch (geoSource)
            {
                case GeoSource.VirtualEarth:
                    geocodePoint = GetGeoDataFromVirtualEarth(geoString);
                    break;
                case GeoSource.LoMoLobby:
                    geocodePoint = GetGeoDataFromLobby(geoString);
                    break;
            }

            return geocodePoint;
        }

        /// <summary>
        /// Gets geo data from lat/lon
        /// </summary>
        /// <param name="lat">the lat</param>
        /// <param name="lon">the lon</param>
        /// <param name="market">the market</param>
        /// <returns>Value indicating success or failure</returns>
        public static bool GetGeoDataFromLatLon(double lat, double lon, out string market)
        {
            var success = false;
            try
            {
                var client = new WebClient();
                var str = client.DownloadString(string.Format(ReverseGeoCodeAPI, lat, lon));
                var xml = XElement.Parse(str);
                var addressNode = xml.Descendants(VENameSpace + "Address").FirstOrDefault();
                var postalCodeNode = addressNode != null ? addressNode.Descendants(VENameSpace + "PostalCode").FirstOrDefault() : null;
                var postalCode = postalCodeNode != null ? postalCodeNode.Value : null;
                var countryNode = addressNode != null ? addressNode.Descendants(VENameSpace + "CountryRegion").FirstOrDefault() : null;
                var country = countryNode != null ? countryNode.Value : null;
                if (country == "United States")
                {
                    country = "us";
                }

                market = country != null && postalCode != null ? country + ":zip:" + postalCode : null;
                success = true;
            }
            catch (Exception e)
            {
                market = e.Message;
            }

            return success;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Queries the LomoLobby for the Location data - Lat,Lon,BoundingBox
        /// </summary>
        /// <param name="geoString">Location Name</param>
        /// <returns>GeocodePoint that encapsulates lat,lon,boundingbox and radius</returns>
        private static GeocodePoint GetGeoDataFromVirtualEarth(string geoString)
        {
            if (string.IsNullOrEmpty(geoString))
            {
                return null;
            }

            var geoServiceUrl = string.Format(GeoCodeAPI, geoString);
            var client = new WebClient();
            var str = client.DownloadString(geoServiceUrl);
            var xml = XElement.Parse(str);

            var geocodePoint = new GeocodePoint { LocationName = geoString };
            var pointNode = xml.Descendants(VENameSpace + "Point").FirstOrDefault();
            var latNode = pointNode != null ? pointNode.Descendants(VENameSpace + "Latitude").FirstOrDefault() : null;
            var lonNode = pointNode != null ? pointNode.Descendants(VENameSpace + "Longitude").FirstOrDefault() : null;
            if (latNode != null && lonNode != null)
            {
                geocodePoint.Latitude = double.Parse(latNode.Value);
                geocodePoint.Longitude = double.Parse(lonNode.Value);
            }

            var boundingBoxNode = xml.Descendants(VENameSpace + "BoundingBox").FirstOrDefault();
            var southLatNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "SouthLatitude").FirstOrDefault() : null;
            var northLatNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "NorthLatitude").FirstOrDefault() : null;
            var westLonNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "WestLongitude").FirstOrDefault() : null;
            var eastLonNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "EastLongitude").FirstOrDefault() : null;
            if (southLatNode != null && northLatNode != null && westLonNode != null && eastLonNode != null)
            {
                geocodePoint.Radius = 0.5 * GetGeoDistanceMeters(
                                                double.Parse(southLatNode.Value),
                                                double.Parse(westLonNode.Value),
                                                double.Parse(northLatNode.Value),
                                                double.Parse(eastLonNode.Value));
                var boundingBox = new BoundingBox
                {
                    SouthLatitude = double.Parse(southLatNode.Value),
                    NorthLatitude = double.Parse(northLatNode.Value),
                    EastLongitude = double.Parse(eastLonNode.Value),
                    WestLongitude = double.Parse(westLonNode.Value)
                };
                geocodePoint.BoundingBox = boundingBox;
            }

            var addressNode = xml.Descendants(VENameSpace + "Address").FirstOrDefault();
            if (addressNode != null)
            {
                var adminDistrict = addressNode.Descendants(VENameSpace + "AdminDistrict").FirstOrDefault();
                var countryRegion = addressNode.Descendants(VENameSpace + "CountryRegion").FirstOrDefault();
                var locality = addressNode.Descendants(VENameSpace + "Locality").FirstOrDefault();
                var postalCode = addressNode.Descendants(VENameSpace + "PostalCode").FirstOrDefault();

                geocodePoint.Location = new GeoLocation()
                {
                    AdminDistrict = adminDistrict != null ? adminDistrict.Value : string.Empty,
                    CountryRegion = countryRegion != null ? countryRegion.Value : string.Empty,
                    Locality = locality != null ? locality.Value : string.Empty,
                    PostalCode = postalCode != null ? postalCode.Value : string.Empty
                };
            }

            return geocodePoint;
        }

        /// <summary>
        /// Queries the LomoLobby for the Location data - Lat,Lon,BoundingBox
        /// </summary>
        /// <param name="geoString">Search text having the location name</param>
        /// <returns>GeocodePoint that encapsulates lat,lon,boundingbox and radius</returns>
        private static GeocodePoint GetGeoDataFromLobby(string geoString)
        {
            string strSearchResults;
            GeocodePoint geocodePoint = null;
            using (var webClient = new WebClient())
            {
                string geoApiUrl = string.Format(LoMoLobbyApi, geoString);
                strSearchResults = webClient.DownloadString(geoApiUrl);
            }

            if (!string.IsNullOrEmpty(strSearchResults))
            {
                var lobbyResults = JsonConvert.DeserializeObject<dynamic>(strSearchResults);
                if (lobbyResults != null && lobbyResults.locations.Count > 0 && lobbyResults.locations[0].RelatedWordsFromQuery.Count > 0)
                {
                    double centerLatitude = 0.0;
                    double centerLongitude = 0.0;
                    double southLatitude = 0.0;
                    double northLatitude = 0.0;
                    double eastLongitude = 0.0;
                    double westLongitude = 0.0;

                    if (lobbyResults.locations[0].CenterLatitude != null)
                    {
                        centerLatitude = lobbyResults.locations[0].CenterLatitude.Value;
                    }

                    if (lobbyResults.locations[0].CenterLongtitude != null)
                    {
                        centerLongitude = lobbyResults.locations[0].CenterLongtitude.Value;
                    }
                    
                    if (lobbyResults.locations[0].SouthLatitude != null)
                    {
                        southLatitude = lobbyResults.locations[0].SouthLatitude.Value;
                    }
                    
                    if (lobbyResults.locations[0].NorthLatitude != null)
                    {
                        northLatitude = lobbyResults.locations[0].NorthLatitude.Value;
                    }
                    
                    if (lobbyResults.locations[0].EastLongitude != null)
                    {
                        eastLongitude = lobbyResults.locations[0].EastLongitude.Value;
                    }
                    
                    if (lobbyResults.locations[0].WestLongitude != null)
                    {
                        westLongitude = lobbyResults.locations[0].WestLongitude.Value;
                    }

                    if (centerLatitude != 0.0 && centerLongitude != 0.0 && southLatitude != 0.0 && northLatitude != 0.0 && eastLongitude != 0.0 && westLongitude != 0.0)
                    {
                        BoundingBox boundingBox = new BoundingBox
                        {
                            SouthLatitude = southLatitude,
                            NorthLatitude = northLatitude,
                            EastLongitude = eastLongitude,
                            WestLongitude = westLongitude
                        };
                        double radius = GetRadiusFromBoundingBox(boundingBox);
                        geocodePoint = new GeocodePoint
                        {
                            LocationName = lobbyResults.locations[0].RelatedWordsFromQuery[0].Value,
                            Latitude = centerLatitude,
                            Longitude = centerLongitude,
                            Radius = radius,
                            BoundingBox = boundingBox
                        };
                    }
                }
            }

            return geocodePoint;
        }

        /// <summary>
        /// This function converts decimal degrees to radians
        /// </summary>
        /// <param name="deg">
        /// decimal degrees 
        /// </param>
        /// <returns>
        /// Radian of the degree 
        /// </returns>
        private static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        /// <summary>
        /// This routine calculates the distance between two points (given the
        ///     latitude/longitude of those points).
        ///     Definitions:
        ///     South latitudes are negative, east longitudes are positive
        ///     Passed to function:
        ///     lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)
        ///     lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)
        ///     unit = the unit you desire for results
        ///     where: 'M' is statute miles
        ///     'K' is kilometers (default)
        ///     'N' is nautical miles
        ///     United States ZIP Code/ Canadian Postal Code databases with latitude
        ///     and longitude are available at http://www.zipcodeworld.com
        /// </summary>
        /// <param name="lat1">
        /// Latitude coordinate of position 1 
        /// </param>
        /// <param name="lon1">
        /// Longitude coordinate of position 1 
        /// </param>
        /// <param name="lat2">
        /// Latitude coordinate of position 2 
        /// </param>
        /// <param name="lon2">
        /// Longitude coordinate of position 2 
        /// </param>
        /// <param name="unit">
        /// Display unit 
        /// </param>
        /// <returns>
        /// The distance between two points. 
        /// </returns>
        private static double GetGeoDistance(double lat1, double lon1, double lat2, double lon2, GeoDistanceUnit unit)
        {
            double theta = lon1 - lon2;
            double dist = (Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2))) + (Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta)));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);

            switch (unit)
            {
                case GeoDistanceUnit.GeoDistanceImperial:
                    dist = dist * 60 * 1.1515;
                    break;

                case GeoDistanceUnit.GeoDistanceMetricSystem:
                    dist = dist * 60 * 1.1515 * 1.609344;
                    break;

                case GeoDistanceUnit.GeoDistanceNonSI:
                    dist = dist * 60 * 1.1515 * 0.8684;
                    break;
            }

            return dist;
        }

        /// <summary>
        /// This function converts radians to decimal degrees
        /// </summary>
        /// <param name="rad">
        /// input radians 
        /// </param>
        /// <returns>
        /// decimal degrees 
        /// </returns>
        private static double Rad2Deg(double rad)
        {
            return rad / Math.PI * 180.0;
        }

        #endregion
    }
}