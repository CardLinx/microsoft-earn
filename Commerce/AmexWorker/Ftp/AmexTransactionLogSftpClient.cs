//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Amex Transaction Log Sftp handling.
    /// </summary>
    public class AmexTransactionLogSftpClient : AmexSftpClientBase
    {
        public AmexTransactionLogSftpClient(CommerceLog log) 
            : base(UserName, Password, log)
        {
        }

        /// <summary>
        /// Username
        /// </summary>
        private static string UserName = GetPropertyValue(AmexSftpConstants.AmexTransactionLogSftpUserNamePropertyName);

        /// <summary>
        /// Password
        /// </summary>
        private static string Password = GetPropertyValue(AmexSftpConstants.AmexTransactionLogSftpPasswordPropertyName);
    }
}