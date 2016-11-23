//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The location validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UserServices.FrondEnd
{
    using System.Text.RegularExpressions;

    using LoMo.UserServices.DataContract;

    /// <summary>
    /// The location validator.
    /// </summary>
    public class LocationValidator
    {
        /// <summary>
        /// The postal pattern regex
        /// </summary>
        private static readonly Regex ValidPostalPattern = new Regex(@"^\d{5}$");

        /// <summary>
        /// Validate the location. </summary> <param name="location">
        /// The location. </param>
        /// <param name="reason"> in case of validation failure will include the reason</param>
        ///  <returns> is valid location argument value
        /// </returns>
        public static bool Validate(Location location, out string reason)
        {
            if (location == null)
            {
                reason = "location information is emtpy";
                return false;
            }

            if (location.LocationType != LocationType.Postal && location.LocationType != LocationType.National)
            {
                reason = string.Format("Location type must be Postal or national. The actual value is: {0}", location.LocationType);
                return false;
            }

            if (location.CountryCode == null || location.CountryCode.ToLowerInvariant() != "us")
            {
                reason = string.Format("CountryCode code must be us. The actual value is: {0}", location.CountryCode);
                return false;
            }

            if (location.LocationType == LocationType.Postal && (location.LocationName == null || !ValidPostalPattern.IsMatch(location.LocationName)))
            {
                reason = string.Format("Not valid us zip code. The provided value is: {0}", location.LocationName);
                return false;
            }

            reason = string.Empty;
            return true;
        }
    }
}