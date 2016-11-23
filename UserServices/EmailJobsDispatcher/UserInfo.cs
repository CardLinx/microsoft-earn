//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace DealsEmailDispatcher
{
    using System;
    using System.Collections.Generic;

    public class UserInfo
    {
        public string UserEmail { get; set; }

        public string UserLocation { get; set; }

        public string Source { get; set; }

        public List<Guid> UserPreferences { get; set; }
    }
}