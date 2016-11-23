//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using Lomo.Commerce.Logging;
    using Microsoft.Azure;

    /// <summary>
    /// Contains methods to upload filtering files via FTP.
    /// </summary>
    public class FilteringFtpClient : MasterCardFtpClientBase
    {
        /// <summary>
        /// Initializes a new instance of the FilteringFtpClient class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public FilteringFtpClient(CommerceLog log)
            : base(log)
        {
        }

        /// <summary>
        /// Gets the name of the directory involved in FTP transfers.
        /// </summary>
        protected override string FtpDirectory
        {
            get
            {
                return CloudConfigurationManager.GetSetting(MasterCardFtpConstants.FilteringDirectory);
            }
        }

        /// <summary>
        /// Gets the substring that must be present for a file to be considered relevant during a directory list operation.
        /// </summary>
        /// <remarks>
        /// This property is not relevant to this client.
        /// </remarks>
        protected override string RelevantFileNameSubstring
        {
            get
            {
                return null;
            }
        }
    }
}