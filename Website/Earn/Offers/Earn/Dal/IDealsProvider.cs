//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earn.Offers.Earn.Dal
{
    interface IDealsProvider
    {
        Task<List<Deal>> GetRewardNetworkDeals(string state);
    }
}