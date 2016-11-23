//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    /// <summary>
    /// Constants related to Amex Sftp Calls
    /// </summary>
    public static class AmexSftpConstants
    {
        /// <summary>
        /// Property Name for Amex Sftp Uri
        /// </summary>
        public static readonly string AmexDataSftpUriPropertyName = "Lomo.Commerce.Amex.Sftp.Uri";

        /// <summary>
        /// Amex Offer Reg Request File User Name
        /// </summary>
        public static readonly string AmexOfferRegRequestSftpUserNamePropertyName = "Lomo.Commerce.Amex.Sftp.OfferRegRequest.Username";

        /// <summary>
        /// Amex Offer Reg Request File Password property
        /// </summary>
        public static readonly string AmexOfferRegRequestSftpPasswordPropertyName = "Lomo.Commerce.Amex.Sftp.OfferRegRequest.Password";

        /// <summary>
        /// Amex Offer Reg Response File User Name
        /// </summary>
        public static readonly string AmexOfferRegResponseSftpUserNamePropertyName = "Lomo.Commerce.Amex.Sftp.OfferRegResponse.Username";

        /// <summary>
        /// Amex Offer Reg Response File Password property
        /// </summary>
        public static readonly string AmexOfferRegResponseSftpPasswordPropertyName = "Lomo.Commerce.Amex.Sftp.OfferRegResponse.Password";

        /// <summary>
        /// Amex TransactionLog File User Name
        /// </summary>
        public static readonly string AmexTransactionLogSftpUserNamePropertyName = "Lomo.Commerce.Amex.Sftp.TransactionLog.Username";

        /// <summary>
        /// Amex TransactionLog File Password property
        /// </summary>
        public static readonly string AmexTransactionLogSftpPasswordPropertyName = "Lomo.Commerce.Amex.Sftp.TransactionLog.Password";

        /// <summary>
        /// Amex Statement Credit File User Name
        /// </summary>
        public static readonly string AmexStatementCreditSftpUserNamePropertyName = "Lomo.Commerce.Amex.Sftp.StatementCredit.Username";
        
        /// <summary>
        /// Amex Statement Credit Password property
        /// </summary>
        public static readonly string AmexStatementCreditSftpPasswordPropertyName = "Lomo.Commerce.Amex.Sftp.StatementCredit.Password";

        /// <summary>
        /// Property Name for Flag to Run File Processing Job or not
        /// </summary>
        public static readonly string RunAmexFileProcessingJobsPropertyName = "Lomo.Commerce.RunAmexFileProcessingJobs";

    }
}