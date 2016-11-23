//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The PartnerTransactions interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.PartnerTransactions
{
    using System;
    using System.Collections.Generic;

    public interface IPartnerTransactions
    {
        /// <summary>
        /// Saves partner transactions to azure storage
        /// </summary>
        /// <param name="partnerTransactionsEntity">
        /// The partner transaction entity.
        /// </param>
        void SavePartnerTransactionEntity(PartnerTransactionsEntity partnerTransactionsEntity);

        /// <summary>
        /// Returns partner transactions from azure storage
        /// </summary>
        /// <returns>
        /// list of partner transaction entities
        /// </returns>
        IEnumerable<PartnerTransactionsEntity> GetPartnerTransactionEntities(Guid transactionId);
    }
}