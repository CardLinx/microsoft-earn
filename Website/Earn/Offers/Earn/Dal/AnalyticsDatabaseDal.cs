//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.db;
using Earn.Models;
using Earn.Offers.Earn.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Dal
{
    public class AnalyticsDatabaseDal
    {
        /// <summary>
        /// Saves the analytics event record to the database.
        /// </summary>
        /// <param name="analyticsModel">The record to persist to database.</param>
        /// <returns>The status of the request.</returns>
        public static async Task<bool> TrySaveAnalyticsRecordToDatabase(AnalyticsModel analyticsModel)
        {
            try
            {
                using (earnanalyticsEntities entities = new earnanalyticsEntities())
                {
                    entities.analytics.Add(DatabaseAdapter.GetDatabaseModel(analyticsModel));
                    await entities.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}