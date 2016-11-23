//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Data;

namespace Earn.Dashboard.Web.Models.PageModels
{
    public class HomePageModel
    {
        public HomePageModel(DataTable dataTable)
        {
            if (dataTable != null && dataTable.Columns.Contains("Name") && dataTable.Columns.Contains("Count"))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    switch (row["Name"].ToString())
                    {
                        case "New Users":
                            NewUsers = row["Count"].ToString();
                            break;
                        case "New Cards":
                            NewCards = row["Count"].ToString();
                            break;
                        case "Earn #":
                            Earns = row["Count"].ToString();
                            break;
                        case "Burn #":
                            Burns = row["Count"].ToString();
                            break;
                    }
                }
            }
            else
            {
                NewUsers = "...";
                NewCards = "...";
                Earns = "...";
                Burns = "...";
            }
        }


        public string NewUsers { get; set; }

        public string NewCards { get; set; }

        public string Earns { get; set; }

        public string Burns { get; set; }
    }
}