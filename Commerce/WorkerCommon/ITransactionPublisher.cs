//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System.Threading.Tasks;
    using Lomo.Commerce.DataContracts;

    /// <summary>
    /// Transaction Publisher Interface
    /// </summary>
    public interface ITransactionPublisher
    {
        /// <summary>
        /// Publish Fdc Transaction Downstream
        /// </summary>
        /// <param name="transactionDetail">
        /// Transaction Details
        /// </param>
        Task PublishTransactionAsync(TransactionDetail transactionDetail);
    }
}