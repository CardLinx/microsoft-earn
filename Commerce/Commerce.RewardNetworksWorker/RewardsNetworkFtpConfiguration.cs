//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker
{
    using Microsoft.Azure;

    /// <summary>
    /// Configuration for SFTP clients.
    /// </summary>
    public interface ISftpConfiguration
    {
        /// <summary>
        /// The username to connect to the FTP server.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// The password to connect to the FTP server.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// The FTP server address.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// The SSH hostkey fingerprint.
        /// </summary>
        string SshHostKeyFingerprint { get; }

        /// <summary>
        /// The FTP protocol to use (FTP/SFTP etc.)
        /// </summary>
        string Protocol { get; }
    }

    /// <summary>
    /// Configuration for RewardsNetwork FTP client.
    /// </summary>
    /// <remarks>
    /// This class requires Azure assemblies to work properly.
    /// This will prevent unit testing, hence the interface.
    /// </remarks>
    public class RewardsNetworkFtpConfiguration : ISftpConfiguration
    {
        /// <summary>
        /// The username to connect to the FTP server.
        /// </summary>
        public string UserName 
        {
            get
            {
                return CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpUserName);
            }
        }

        /// <summary>
        /// The password to connect to the FTP server.
        /// </summary>
        public string Password
        {
            get
            {
                return CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpPassword);
            }
        }

        /// <summary>
        /// The FTP server address.
        /// </summary>
        public string Address
        {
            get
            {
                return CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpAddress);
            }
        }

        /// <summary>
        /// The SSH hostkey fingerprint for the Rewards Network FTP server.
        /// </summary>
        public string SshHostKeyFingerprint
        {
            get
            {
                return CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpSshHostKeyFingerprint);
            }
        }

        /// <summary>
        /// The FTP protocol to use (FTP/SFTP etc.)
        /// </summary>
        public string Protocol
        {
            get
            {
                return CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpProtocol);
            }
        }

        /// <summary>
        /// Directory for uploading transaction reports to the Rewards Network FTP.
        /// </summary>
        public string TransactionReportDirectory
        {
            get
            {
                return CloudConfigurationManager.GetSetting(RewardsNetworkConstants.FtpTransactionReportDirectory);
            }
        }
    }
}