//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Earn.Dashboard.LomoUsersDAL.db;
using Earn.DataContract.LomoUsers;

namespace Earn.Dashboard.LomoUsersDAL
{
    public class LomoUserDal
    {
        public static async Task<List<Customer>> FetchUsersByFilter(CustomerFilter filter)
        {
            LomoUsersEntities dbContext = new LomoUsersEntities();
            var query = from u in dbContext.Users
                        join ulTmp in dbContext.UsersLocations on u.Id equals ulTmp.UserId into userLocations
                        from ul in userLocations.DefaultIfEmpty()
                        select new Customer
                        {
                            GlobalId = u.Id,
                            Name = u.Name,
                            Email = u.Email,
                            MSID = u.MsId,
                            PhoneNumber = u.PhoneNumber,
                            State = ul != null ? ul.State : string.Empty,
                            City = ul != null ? ul.City : string.Empty,
                            ZipCode = ul != null ? ul.ZipCode : string.Empty
                        };

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                query = query.Where(x => x.Email == filter.Email);
            }

            if (filter.UserId != null)
            {
                query = query.Where(x => x.GlobalId == filter.UserId);
            }

            if (!string.IsNullOrWhiteSpace(filter.MSIDorPUID))
            {
                query = query.Where(x => x.MSID == filter.MSIDorPUID);
            }

            return await query.ToListAsync();
        }

        public static async Task<List<Customer>> FetchUsersByIds(List<Guid> ids)
        {
            LomoUsersEntities dbContext = new LomoUsersEntities();
            var query = from u in dbContext.Users
                        join ulTmp in dbContext.UsersLocations on u.Id equals ulTmp.UserId into userLocations
                        from ul in userLocations.DefaultIfEmpty()
                        where ids.Contains(u.Id)
                        select new Customer
                        {
                            GlobalId = u.Id,
                            Name = u.Name,
                            Email = u.Email,
                            MSID = u.MsId,
                            PhoneNumber = u.PhoneNumber,
                            State = ul != null ? ul.State : string.Empty,
                            City = ul != null ? ul.City : string.Empty,
                            ZipCode = ul != null ? ul.ZipCode : string.Empty
                        };

            return await query.ToListAsync();
        }

    }
}