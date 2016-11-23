//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Commerce.RewardsNetworkWorker
{
    using System;

    using Common.Utils;

    using Lomo.Commerce.Logging;
    using WinSCP;

    /// <summary>
    /// Client for interacting with Rewards Network FTP.
    /// </summary>
    /// <remarks>
    /// This does not derive from FtpClientBase because RN FTP
    /// implements the SFTP protocol and the .NET FTP classes
    /// does not support that.
    /// If we have other SFTPs, then it would make sense to extract
    /// a base class from this.
    /// </remarks>
    public class RewardsNetworkFtpClient
    {
        readonly ISftpConfiguration config;

        readonly ICommerceLog log;

        public RewardsNetworkFtpClient(ISftpConfiguration config, ICommerceLog log)
        {
            this.config = config;
            this.log = log;
        }

        /// <summary>
        /// Upload the file to the FTP server.
        /// </summary>
        /// <param name="localPath">The path to the file to be uploaded.</param>
        /// <param name="serverPath">The path on the server where the file should be stored.</param>
        public void UploadFile(string localPath, string serverPath)
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = (Protocol)Enum.Parse(typeof(Protocol), this.config.Protocol),
                HostName = this.config.Address,
                UserName = this.config.UserName,
                Password = this.config.Password,
                SshHostKeyFingerprint = this.config.SshHostKeyFingerprint
            };

            this.log.Information(
                "Uploading file: '{0}' to server: '{1}', server path :'{2}', protocol: '{3}'",
                localPath,
                this.config.Address,
                serverPath,
                this.config.Protocol);

            bool uploadSuccess = Execute.WithRetry(
                new ExponentialBackoffRetryPolicy(), 
                i =>
                {
                    try
                    {
                        using (var session = new Session())
                        {
                            // Connect
                            session.Open(sessionOptions);

                            // Upload file
                            var transferOptions = new TransferOptions { TransferMode = TransferMode.Binary };

                            TransferOperationResult transferResult = session.PutFiles(
                                localPath,
                                serverPath,
                                false,
                                transferOptions);

                            // Throw on any error
                            transferResult.Check();
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal()) throw;

                        this.log.Error(
                            "An error occurred while uploading file '{0}' to server '{1}' at path '{2}' on attempt {3}",
                            ex,
                            localPath,
                            this.config.Address,
                            serverPath,
                            i);

                        return false;
                    }
                });

            this.log.Information(
                "Uploading file to server '{0}' " + (uploadSuccess ? "succeeded" : "failed"),
                this.config.Address);
        }

        /// <summary>
        /// Downloads a file from the FTP server.
        /// </summary>
        /// <param name="serverPath">The path to the file to be downloaded from the server.</param>
        /// <param name="localPath">The local path where the downloaded file should be saved.</param>
        public void DownloadFile(string serverPath, string localPath)
        {
            // Setup session options
            var sessionOptions = new SessionOptions
            {
                Protocol = (Protocol)Enum.Parse(typeof(Protocol), this.config.Protocol),
                HostName = this.config.Address,
                UserName = this.config.UserName,
                Password = this.config.Password,
                SshHostKeyFingerprint = this.config.SshHostKeyFingerprint
            };

            this.log.Information(
                "Downloading file: '{0}' from server: '{1}', local path :'{2}', protocol: '{3}'",
                serverPath,
                this.config.Address,
                localPath,
                this.config.Protocol);

            bool downloadSuccess = Execute.WithRetry(
                new ExponentialBackoffRetryPolicy(), 
                i =>
                {
                    using (var session = new Session())
                    {
                        try
                        {
                            // Connect
                            session.Open(sessionOptions);

                            // Download file
                            var transferOptions = new TransferOptions { TransferMode = TransferMode.Binary };

                            TransferOperationResult transferResult = session.GetFiles(
                                serverPath,
                                localPath,
                                false,
                                transferOptions);

                            // Throw on any error
                            transferResult.Check();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            if (ex.IsFatal()) throw;

                            this.log.Error(
                                "An error occurred while downloading file '{0}' from server '{1}' to path '{2}' on attempt {3}",
                                ex,
                                serverPath,
                                this.config.Address,
                                localPath,
                                i);

                            return false;
                        }
                    }
                });

            this.log.Information(
                "Downloading file from server '{0}' " + (downloadSuccess ? "succeeded" : "failed"),
                this.config.Address);
        }
    }
}