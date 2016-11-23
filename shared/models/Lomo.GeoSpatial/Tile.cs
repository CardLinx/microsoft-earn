//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Simple tiling.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.GeoSpatial
{
    using System;
    using System.Runtime.Serialization;
    using ProtoBuf;

    /// <summary>
    /// Used for simple tiling
    /// </summary>
    [DataContract]
    public class Tile
    {
        /// <summary>
        /// Grid base. Difines how many tiles fit in one degree.
        /// </summary>
        private const int Base = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile" /> class.
        /// Just to make serializers happy.
        /// </summary>
        public Tile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile" /> class.
        /// </summary>
        /// <param name="point">the lat/lon</param>
        public Tile(Point point)
        {
            var latZoneId = GetZoneId(point.Latitude);
            var lonZoneId = GetZoneId(point.Longitude);
            SetPropertities(latZoneId, lonZoneId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile" /> class.
        /// </summary>
        /// <param name="latitude">the latitude</param>
        /// <param name="longitude">the longitude</param>
        public Tile(double latitude, double longitude)
        {
            var latZoneId = GetZoneId(latitude);
            var lonZoneId = GetZoneId(longitude);
            SetPropertities(latZoneId, lonZoneId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile" /> class.
        /// </summary>
        /// <param name="latZoneId">latitude zone id</param>
        /// <param name="lonZoneId">longitude zone id</param>
        public Tile(int latZoneId, int lonZoneId)
        {
            SetPropertities(latZoneId, lonZoneId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile" /> class.
        /// </summary>
        /// <param name="tileId">tile id</param>
        public Tile(int tileId)
        {
            SetPropertities(tileId);
        }

        /// <summary>
        /// Gets or sets tile Id
        /// </summary>
        [DataMember(EmitDefaultValue = true, Name = "tile_id")]
        [ProtoMember(10)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets latitude of tile center
        /// </summary>
        [DataMember(EmitDefaultValue = true, Name = "tile_center_lat")]
        [ProtoMember(20)]
        public double CenterLat { get; set; }

        /// <summary>
        /// Gets or sets longitude of tile center
        /// </summary>
        [DataMember(EmitDefaultValue = true, Name = "tile_center_lon")]
        [ProtoMember(30)]
        public double CenterLon { get; set; }

        /// <summary>
        /// Gets zone id
        /// </summary>
        /// <param name="degrees">lat or lon</param>
        /// <returns>zone id</returns>
        public static int GetZoneId(double degrees)
        {
            // TODO!!! This will need to change to use Math.Floor
            return (int)(degrees * Base);
        }

        /// <summary>
        /// Sets center properities
        /// </summary>
        /// <param name="latZoneId">latitude zone id</param>
        /// <param name="lonZoneId">longitude zone id</param>
        private void SetCenter(int latZoneId, int lonZoneId)
        {
            CenterLat = (latZoneId + 0.5) / Base;
            CenterLon = (lonZoneId + 0.5) / Base;
        }

        /// <summary>
        /// Sets tile properities
        /// </summary>
        /// <param name="latZoneId">latitude zone id</param>
        /// <param name="lonZoneId">longitude zone id</param>
        private void SetPropertities(int latZoneId, int lonZoneId)
        {
            Id = (latZoneId * 50000) + lonZoneId; // multiplier should exceed 180*64 which is < 20000
            SetCenter(latZoneId, lonZoneId);
        }

        /// <summary>
        /// Sets tile properities
        /// </summary>
        /// <param name="tileId">tile id</param>
        private void SetPropertities(int tileId)
        {
            Id = tileId;
            var latZoneId = (int)Math.Round((double)tileId / 50000);
            var lonZoneId = tileId - (latZoneId * 50000);
            SetCenter(latZoneId, lonZoneId);
        }
    }
}