//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.db;
using Earn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Helper
{
    public class DatabaseAdapter
    {
        /// <summary>
        /// Transforms the object to the database schema object that can be persisted.
        /// </summary>
        /// <returns>Returns the transformed database schema object.</returns>
        public static analytic GetDatabaseModel(AnalyticsModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            analytic record = new analytic();

            record.NewUser = model.NewUser;
            record.IsAuthenticated = model.IsAuthenticated;
            record.AuthenticatedUserId = model.AuthenticatedUserId;
            record.SessionId = model.SessionId;
            record.BrowserId = model.BrowserId;

            record.PageUrl = model.PageUrl;
            record.PageTitle = model.PageTitle;
            record.ServerTimeStamp = model.ServerTimeStamp.DateTime;

            record.cmp_name = model.CampaignName;
            record.cmp_ref = model.CampaignReferrer;
            record.cmp_source = model.CampaignSource;

            record.IPAddress = model.IPAddress;
            record.UserAgent = model.UserAgent;
            record.DeviceType = model.DeviceType;

            record.EventId = model.EventId;
            record.EventType = model.EventType;
            record.EventInfo = model.EventInfo;

            record.FlightId = model.FlightId;
            return record;
        }
    }
}