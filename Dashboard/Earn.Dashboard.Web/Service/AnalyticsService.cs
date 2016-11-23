//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Earn.Dashboard.Web.Service
{
    public static class AnalyticsService
    {
        public static DataTable DailyStatistics(DateTime date)
        {
            string query = string.Format("SELECT * FROM DailyStatistics('{0}'); ", date.ToShortDateString());            
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DailyStatistics"].ConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
            {
                sqlDataAdapter.Fill(dataTable);
            }

            return dataTable;
        }
    }
}