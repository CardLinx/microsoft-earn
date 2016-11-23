//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Earn.Dashboard.CustomerService.DAL.db;
using Earn.DataContract.Commerce;

namespace Earn.Dashboard.CustomerService.DAL
{
    public class CustomerServiceDAL
    {
        public static async Task IssueCredits(IssueCreditsRequest request)
        {

        }

        public static async Task<List<EarnBurnHistory>> GetEarnBurnHistory(Guid userId)
        {
            CustomerServiceEntities dbContext = new CustomerServiceEntities();
            var query = from il in dbContext.QueryEarnBurnLineItems()
                        where il.GlobalId == userId
                        select new  EarnBurnHistory
                        {

                        };

            return await query.ToListAsync();

        }
    }
}