//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.DataContracts.Extensions;
using System.Threading.Tasks;

namespace Lomo.Commerce.CardLink.Partners
{
    public interface IVisaClient
    {
        /// <summary>
        /// Issue credit to user during a burn transaction in lieu of earn credit that user has in his account
        /// </summary>
        /// <returns></returns>
        Task<ResultCode> SaveStatementCreditAsync();

    }
}