//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Earn.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Earn.Offers.Earn.Dal
{
    public class DocumentDbDealsProvider : IDealsProvider
    {
        private DocumentDbDealsProvider()
        {
        }

        public static DocumentDbDealsProvider Instance = new DocumentDbDealsProvider();

        public async Task<List<Deal>> GetRewardNetworkDeals(string state)
        {
            List<Deal> deals = await DocumentDBRepository.Instance.Client.ExecuteStoredProcedureAsync<List<Deal>>(DocumentDBRepository.Instance.GetRewardNetworkDealsByState.SelfLink, state).ConfigureAwait(false);
            return deals;
        }
    }
}