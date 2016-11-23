//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Earn.Offers.Earn.Models
{
    public class AuthorizeUserResult
    {
        public bool Authorized
        {
            get;
            set;
        }

        public ActionResult Result
        {
            get;
            set;
        }
    }
}