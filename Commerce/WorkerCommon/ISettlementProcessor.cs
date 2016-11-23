//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Threading.Tasks;

namespace Lomo.Commerce.WorkerCommon
{
    /// <summary>
    /// Processes settlement transactions.
    /// </summary>
    public interface ISettlementProcessor
    {
        /// <summary>
        /// Processes settlement transactions.
        /// </summary>
        Task Process();
    }
}