//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.SftpClient;
    using Microsoft.Azure;

    /// <summary>
    /// Base client which provides SFTP wrapper for Amex specific functionality
    /// </summary>
    /// <remarks>
    /// This just wraps the calls to underlying sftp binary, as that is not testable. To we mock everything beyond this layer.
    /// </remarks>
    public abstract class AmexSftpClientBase
    {
        /// <summary>
        /// Creates an instance 
        /// </summary>
        /// <param name="userName">
        /// Username for the connection
        /// </param>
        /// <param name="password">
        /// Password for the connection
        /// </param>
        /// <param name="log">
        /// Handle to Commerce Logger
        /// </param>
        protected AmexSftpClientBase(string userName, string password, CommerceLog log)
        {
            Client = SftpClientFactory.SftpClient(userName, password, SftpUri, CommerceWorkerConfig.Instance);
            Log = log;
        }

        /// <summary>
        /// Get all file names in a directory
        /// </summary>
        /// <returns>
        /// Array of file names as strings
        /// </returns>
        public virtual async Task<string[]> DirectoryListAsync(string fileNamePrefix, string folderName)
        {
            Log.Verbose("Request to get file list");
            return await Client.DirectoryListAsync(fileNamePrefix, folderName);
        }

        /// <summary>
        /// Upload a file to the server
        /// </summary>
        /// Name of the file to Create
        /// <param name="fileName">
        /// </param>
        /// <param name="stream">
        /// File content as stream data
        /// </param>
        /// <param name="folderName">Parent folder of the file</param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task UploadFileAsync(string fileName, Stream stream, string folderName)
        {
           
            Log.Verbose("Request to Upload file {0}", fileName);
            await Client.UploadFileAsync(fileName, stream, folderName);
            
        }

        /// <summary>
        /// Download a file 
        /// </summary>
        /// <param name="fileName">
        /// Name of the file to download
        /// </param>
        /// <param name="stream">
        /// Memory Stream to read the download data into
        /// </param>
        /// <param name="folderName">Parent folder of the file</param>
        /// <returns>
        /// File Content as stream data
        /// </returns>
        public async Task DownloadFileAsync(string fileName, Stream stream, string folderName)
        {

            Log.Verbose("Request to Download file {0}", fileName);
            await Client.DownloadFileAsync(fileName, stream, folderName);
        }

        /// <summary>
        /// Sftp Server URI
        /// </summary>
        internal string SftpUri = GetPropertyValue(AmexSftpConstants.AmexDataSftpUriPropertyName);

        /// <summary>
        /// Get Property Value for SFTP Ops given a name
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property
        /// </param>
        /// <returns>
        /// Value of the property
        /// </returns>
        protected static string GetPropertyValue(string propertyName)
        {
            return CloudConfigurationManager.GetSetting(propertyName);
        }

        /// <summary>
        /// Commerce Log Handler
        /// </summary>
        private CommerceLog Log;

        /// <summary>
        /// Sftp Client
        /// </summary>
        private ISftpClient Client;


    }
}