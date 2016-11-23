//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Models;
using Earn.Offers.Earn.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Services
{
    public class AnalyticsService
    {
        /// <summary>
        /// Asynhronously saves the analytic record to database.
        /// </summary>
        /// <param name="analyticsData">Analytics data record.</param>
        /// <returns>Returns the status.</returns>
        public static async Task<bool> TrySaveAnalyticsData(AnalyticsModel analyticsData)
        {
            if (analyticsData == null)
            {
                return false;
            }

            bool status = await AnalyticsDatabaseDal.TrySaveAnalyticsRecordToDatabase(analyticsData);
            return status;
        }
    }
}