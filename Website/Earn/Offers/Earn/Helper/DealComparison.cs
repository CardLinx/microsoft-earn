//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using Earn.Offers.Earn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Helper
{
    public class DealBusinessComparer : IComparer<Deal>
    {
        private static IComparer<Deal> byZipcode = null;
        private static IComparer<Deal> byName = null;
        private static IComparer<Deal> byCity = null;
        private static IComparer<Deal> byCuisine = null;
        DealSortOrder sortOrder = DealSortOrder.BusinessName;

        private DealBusinessComparer(DealSortOrder sortOrder)
        {
            this.sortOrder = sortOrder;
        }

        public static IComparer<Deal> GetInstance(DealSortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case DealSortOrder.City:
                    if (byCity == null)
                    {
                        byCity = new DealBusinessComparer(DealSortOrder.City);
                    }

                    return byCity;

                case DealSortOrder.Zip:
                    if (byZipcode == null)
                    {
                        byZipcode = new DealBusinessComparer(DealSortOrder.Zip);
                    }

                    return byZipcode;

                case DealSortOrder.BusinessName:
                    if (byName == null)
                    {
                        byName = new DealBusinessComparer(DealSortOrder.BusinessName);
                    }

                    return byName;

                case DealSortOrder.Cuisine:
                    if (byCuisine == null)
                    {
                        byCuisine = new DealBusinessComparer(DealSortOrder.Cuisine);
                    }

                    return byCuisine;

                default:
                    throw new InvalidOperationException("Unknown Sort Order");
            }
        }

        public int Compare(Deal x, Deal y)
        {
            switch (this.sortOrder)
            {
                case DealSortOrder.BusinessName:
                    return x.Business.Name.CompareTo(y.Business.Name);
                case DealSortOrder.City:
                    return x.Business.Locations[0].City.CompareTo(y.Business.Locations[0].City);
                case DealSortOrder.Zip:
                    return x.Business.Locations[0].Zip.CompareTo(y.Business.Locations[0].Zip);
                case DealSortOrder.Cuisine:
                    return GetCuisine(x).CompareTo(GetCuisine(y));
                default:
                    throw new InvalidOperationException("Unknown Sort Order");
            }
        }

        private string GetCuisine(Deal x)
        {
            if (x == null || x.Business == null || x.Business.Locations == null || x.Business.Locations.Count == 0 || x.Business.Locations[0].Cuisine == null)
            {
                return string.Empty;
            }

            return x.Business.Locations[0].Cuisine;
        }
    }
}