//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Earn.Offers.Earn.Models
{
    public class EmailInfo
    {
        public List<string> To
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string FromDisplayName
        {
            get;
            set;
        }

        public string ReplyTo
        {
            get;
            set;
        }

        public string HtmlBody
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string TextBody
        {
            get;
            set;
        }
    }
}