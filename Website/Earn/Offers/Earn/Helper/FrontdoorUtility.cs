//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Helper
{
    public class FrontdoorUtility
    {
        private static readonly char[] CommaSplitParam = { ',' };

        private static readonly char[] EqualsSplitParam = { '=' };
        
        public static UserLocation ParseReverseIp(string reverseIp)
        {
            if (string.IsNullOrWhiteSpace(reverseIp))
            {
                return null;
            }

            UserLocation location = new UserLocation
            {
                Location = reverseIp
            };

            string[] keyValues = reverseIp.Split(CommaSplitParam, StringSplitOptions.RemoveEmptyEntries);
            foreach (string[] keyValue in keyValues.Select(parameter => parameter.Split(EqualsSplitParam, 2, StringSplitOptions.RemoveEmptyEntries)).Where(keyValue => keyValue.Length == 2 && !string.IsNullOrWhiteSpace(keyValue[0])))
            {
                switch (keyValue[0].Trim().ToLowerInvariant())
                {
                    case "lat":
                        double lat;
                        double.TryParse(keyValue[1], out lat);
                        location.Latitude = lat;
                        break;
                    case "long":
                        double lon;
                        double.TryParse(keyValue[1], out lon);
                        location.Longitude = lon;
                        break;
                    case "country":
                        location.Country = keyValue[1];
                        break;
                    case "state":
                        location.State = keyValue[1];
                        break;
                    case "city":
                        location.City = keyValue[1];
                        break;
                    case "dma":
                        location.Dma = keyValue[1];
                        break;
                    case "zip":
                        location.ZipCode = keyValue[1];
                        break;
                    case "iso":
                        location.Iso = keyValue[1];
                        break;
                }
            }

            return location;
        }
    }
}