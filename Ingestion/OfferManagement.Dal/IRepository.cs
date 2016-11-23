//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository
    /// </summary>
    public interface IRepository
    {
        Task<bool> CreateAsync<T>(IEnumerable<T> items);        

        Task<bool> UpdateAsync<T>(IEnumerable<T> items);

        Task<bool> DeleteAsync(IEnumerable<string> ids);
    }
}