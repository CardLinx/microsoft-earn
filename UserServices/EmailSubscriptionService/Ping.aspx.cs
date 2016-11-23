//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Configuration;
using System.Diagnostics;
using Users.Dal;

namespace LoMo.EmailSubscription.Service
{
    public partial class Ping : System.Web.UI.Page
    {
        private IUsersDal usersDal;

        public Ping()
        {
            var connectionString = ConfigurationManager.AppSettings["UsersDalConnectionString"];
            var storageAccountConnectionString = ConfigurationManager.AppSettings["StorageAccountConnectionString"];
            this.usersDal = new UsersDal(connectionString, null, false, new PriorityEmailJobsQueue<PriorityEmailCargo>(storageAccountConnectionString));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string strResponse = string.Empty;
            try
            {
                var emailSubscriptions = usersDal.GetEmailSubscriptionsByEmail("foo@foo.com",true,"WeeklyDeals");
                sw.Stop();
                strResponse = string.Format("Success in {0} ms", sw.ElapsedMilliseconds);
            }
            catch (Exception)
            {
                strResponse = "Error";
            }


            Response.Write(strResponse);
        }
        
    }
}