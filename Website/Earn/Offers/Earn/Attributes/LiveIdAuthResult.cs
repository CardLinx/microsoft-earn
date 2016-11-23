//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Attributes
{
    public class LiveIdAuthResult
    {
        public bool IsUserAuthenticated
        {
            get;
            set;
        }

        public string SignInHtmlLink
        {
            get;
            set;
        }

        public string SignOutHtmlLink
        {
            get;
            set;
        }

        public string RpsHeaders
        {
            get;
            set;
        }

        public string Puid
        {
            get;
            set;
        }

        public string Anid
        {
            get;
            set;
        }

        public string ProfileName
        {
            get;
            set;
        }
    }
}