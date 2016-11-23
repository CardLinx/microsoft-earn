//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Configuration;

namespace Earn.Dashboard.Web.Utils
{
    public static class Config
    {
        public const string ThumbnailPhotoCookie = "thp";

        public const string ThumbnailPhoto = "ThumbnailPhoto";

        public const string Role = "Role";

        public const string IsAuthorized = "IsAuthorized";

        public const string DailyStatistics = "DailyStatistics";

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
            public const string Support = "Support";
            public const string Visitor = "Visitor";
        }

        public static bool IsProduction
        {
            get { return ConfigurationManager.AppSettings["IsProduction"] == "true"; }
        }

        public static readonly Dictionary<string, string> SecurityGroups = new Dictionary<string, string>
        {
            { Roles.Admin, "8844fef1-d514-488a-8134-1fde0686f0d7"}, // earnit-admin
            { Roles.User, "0db5e92b-1718-4b6e-bb43-3742aed33071"}, // earnit
            { Roles.Support, "f73b2c49-d55b-45d2-b86c-8706f33fdd98"}, // earnit-support
            { Roles.Visitor, null} // earnit
        };
    }
}