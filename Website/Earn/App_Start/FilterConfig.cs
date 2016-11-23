//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.Offers.Earn.Attributes;
using System.Web;
using System.Web.Mvc;

namespace Earn
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute(), 3);
            filters.Add(new RootDomainRedirectFilterAttribute(), 1);
           // filters.Add(new CorpnetFilterAttribute(), 2);
        }
    }
}