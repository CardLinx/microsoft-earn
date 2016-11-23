//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace DealsEmailDispatcher
{
    public class SendGridResponse
    {
        public string email { get; set; }

        public string status { get; set; }

        public string reason { get; set; }

        public string created { get; set; }
    }
}