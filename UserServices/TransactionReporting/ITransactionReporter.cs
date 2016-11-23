//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Reports the partner deals transactions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TransactionReporting
{
    using LoMo.UserServices.DealsMailing;

    /// <summary>
    /// Reports the partner deals transactions
    /// </summary>
    /// <typeparam name="T">PartnerTransactionReportingCargo</typeparam>
    public interface ITransactionReporter<in T> where T : PartnerTransactionReportingCargo
    {
        /// <summary>
        /// Initializes the Transaction reporter
        /// </summary>
        void Initialize();

        /// <summary>
        /// Invokes the partner endpoint to report the transactions
        /// </summary>
        /// <param name="transactionReportingCargo">Partner TransactionReporting cargo</param>
        void Report(T transactionReportingCargo);
    }
}