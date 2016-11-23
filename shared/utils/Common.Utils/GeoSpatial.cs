//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The geo spatial.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Common.Utils
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Xml.Linq;

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
        private const double MileToMeters = 1609.344;

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
            return distance / MileToMeters;
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
            return distance * MileToMeters;
        }

        /// <summary>
        /// Returns the distance between the two input points,
        ///     in meters.
        /// </summary>
        /// <param name="lat1">
        /// Point 1 Latitude 
        /// </param>
        /// <param name="lon1">
        /// Point 1 Longitude 
        /// </param>
        /// <param name="lat2">
        /// Point 2 Latitude 
        /// </param>
        /// <param name="lon2">
        /// Point 2 Longitude 
        /// </param>
        /// <returns>
        /// The System.Double. 
        /// </returns>
        public static double GetGeoDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var distanceKm = GetGeoDistance(lat1, lon1, lat2, lon2, GeoDistanceUnit.GeoDistanceMetricSystem);
            return distanceKm * 1000.0;
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
        /// Translates geo string to lat/lon, radius, and bounding box
        /// </summary>
        /// <param name="geoString">geo string</param>
        /// <param name="lat">the latitude of the region center</param>
        /// <param name="lon">the longitude of the region center</param>
        /// <param name="radius">the radius of the region</param>
        /// <param name="boundingBox">the bounding box of the region</param>
        /// <returns>Value indicating success or failure</returns>
        public static bool GetGeoDataFromString(string geoString, out double lat, out double lon, out double radius, out XElement boundingBox)
        {
            lat = 0;
            lon = 0;
            radius = 0;
            boundingBox = null;
            var success = false;
            try
            {
                var geoServiceUrl = string.Format(GeoCodeAPI, geoString);
                var client = new WebClient();
                var str = client.DownloadString(geoServiceUrl);
                var xml = XElement.Parse(str);

                var pointNode = xml.Descendants(VENameSpace + "Point").FirstOrDefault();
                var latNode = pointNode != null ? pointNode.Descendants(VENameSpace + "Latitude").FirstOrDefault() : null;
                var lonNode = pointNode != null ? pointNode.Descendants(VENameSpace + "Longitude").FirstOrDefault() : null;
                if (latNode != null && lonNode != null)
                {
                    lat = double.Parse(latNode.Value);
                    lon = double.Parse(lonNode.Value);
                }

                var boundingBoxNode = xml.Descendants(VENameSpace + "BoundingBox").FirstOrDefault();
                var southLatNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "SouthLatitude").FirstOrDefault() : null;
                var northLatNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "NorthLatitude").FirstOrDefault() : null;
                var westLonNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "WestLongitude").FirstOrDefault() : null;
                var eastLonNode = boundingBoxNode != null ? boundingBoxNode.Descendants(VENameSpace + "EastLongitude").FirstOrDefault() : null;
                if (southLatNode != null && northLatNode != null && westLonNode != null && eastLonNode != null)
                {
                    radius = 0.5 * GetGeoDistanceMeters(double.Parse(southLatNode.Value), double.Parse(westLonNode.Value), double.Parse(northLatNode.Value), double.Parse(eastLonNode.Value));
                    boundingBox = new XElement("BoundingBox", new XAttribute("SouthLatitude", southLatNode.Value), new XAttribute("NorthLatitude", northLatNode.Value), new XAttribute("WestLongitude", westLonNode.Value), new XAttribute("EastLongitude", eastLonNode.Value));
                }

                success = true;
            }
            catch (Exception)
            {
                // throw;
            }

            return success;
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