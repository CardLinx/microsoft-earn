//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.SftpClient
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Sftp Client contract 
    /// </summary>
    public interface ISftpClient
    {
        /// <summary>
        /// Get all file names in a directory
        /// </summary>
        /// <returns>
        /// Array of file names as strings
        /// </returns>
        Task<string[]> DirectoryListAsync(string fileNamePrefix, string folderName);

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
        Task UploadFileAsync(string fileName, Stream stream, string folderName);

        /// <summary>
        /// Download a file 
        /// </summary>
        /// <param name="fileName">
        /// Name of the file to download
        /// </param>
        /// <param name="stream">
        /// Memory Stream to read the download data into
        /// </param>
        /// <param name="folderName">Parent folder of the file </param>
        /// <returns>
        /// File Content as stream data
        /// </returns>
        Task DownloadFileAsync(string fileName, Stream stream, string folderName);
    }
}