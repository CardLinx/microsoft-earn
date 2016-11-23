//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System.Net;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Class to define FTP interaction for FDC Pts files
    /// </summary>
    public class FirstDataPtsFtpClient: FirstDataFtpClientBase
    {
        /// <summary>
        /// Constructor to create a new instance
        /// </summary>
        /// <param name="log">
        /// Handle to the Commerce Log
        /// </param>
        public FirstDataPtsFtpClient(CommerceLog log)
            : base(new NetworkCredential(UserName, Password), true, log)
        {
        }

        /// <summary>
        /// The Remote Directory this account refers to
        /// </summary>
        public override string RemoteDirectory
        {
            get
            {
                return GetPropertyValue(FirstDataFtpConstants.FirstDataPtsDirectoryPropertyName);
            }
        }

        /// <summary>
        /// Username
        /// </summary>
        private static string UserName = GetPropertyValue(FirstDataFtpConstants.FirstDataPtsFtpUserNamePropertyName);

        /// <summary>
        /// Password
        /// </summary>
        private static string Password = GetPropertyValue(FirstDataFtpConstants.FirstDataPtsFtpPasswordPropertyName);
    }
}