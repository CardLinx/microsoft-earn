//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FtpClient
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for FTP clients.
    /// </summary>
    public interface IFtpClient
    {
        /// <summary>
        /// Gets a list of all file names in a directory.
        /// </summary>
        /// <returns>
        /// An array of file names as strings
        /// </returns>
        Task<string[]> DirectoryListAsync();

        /// <summary>
        /// Uploads the specified file to the server.
        /// </summary>
        /// Name of the file to create.
        /// <param name="fileName">
        /// </param>
        /// <param name="stream">
        /// Stream containing file contents.
        /// </param>
        /// <returns>
        /// Task to perform the requested operation.
        /// </returns>
        Task UploadFileAsync(string fileName,
                             Stream stream);

        /// <summary>
        /// Downloads the specified file from the server.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file to download.
        /// </param>
        /// <param name="stream">
        /// Memory Stream into which to read the download data.
        /// </param>
        /// <returns>
        /// Task to perform the requested operation.
        /// </returns>
        Task DownloadFileAsync(string fileName,
                               Stream stream);
    }
}