//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Earn.Dashboard.Web.Service;
using Earn.DataContract.LomoUsers;

namespace Earn.Dashboard.Web.Controllers.Api
{
    public class SupportController : ApiController
    {

        [HttpGet]
        public async Task<List<Customer>> FindCustomers(string customerFilter)
        {
            var data = await CustomerService.FindCustomers(customerFilter);
            return data;
        }
    }
}