//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace AcsToken
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Rebex.Net;

    /// <summary>
    /// Default Sftp Client Implementation 
    /// </summary>
    public class SftpClient
    {
        /// <summary>
        /// Creates instance of the default implementation
        /// </summary>
        /// <param name="username">
        /// username of the connection
        /// </param>
        /// <param name="password">
        /// password of the connection
        /// </param>
        /// <param name="uri">
        /// Uri of Sftp server
        /// </param>
        public SftpClient(string username, string password, string uri)
        {
            Username = username;
            Password = password;
            SftpUri = uri;
        }

        /// <summary>
        /// Get all file names in a directory
        /// </summary>
        /// <returns>
        /// Array of file names as strings
        /// </returns>
        public async Task<string[]> DirectoryListAsync(string fileNamePrefix, string folderName)
        {
            return await Task.Run(
                () =>
                {
                    List<string> fileNames = new List<string>();
                    using (Sftp client = new Sftp())
                    {
                        client.Connect(SftpUri);
                        client.Login(Username, Password);
                        client.ChangeDirectory(folderName);
                        SftpItemCollection list = client.GetList();
                        foreach (SftpItem item in list)
                        {
                            if (item.Name.ToUpper().StartsWith(fileNamePrefix.ToUpper()))
                            {
                                fileNames.Add(item.Name);
                            }
                        }
                    }

                    return fileNames.ToArray();
                }
                );
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
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task UploadFileAsync(string fileName, Stream stream, string folderName)
        {
            await Task.Run(
                () =>
                {
                    using (Sftp client = new Sftp())
                    {
                        client.Connect(SftpUri);
                        client.Login(Username, Password);
                        client.ChangeDirectory(folderName);
                        client.PutFile(stream, fileName);
                    }
                }
            );
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
        /// <returns>
        /// File Content as stream data
        /// </returns>
        public async Task DownloadFileAsync(string fileName, Stream stream, string folderName)
        {
            await Task.Run(
                () =>
                {
                    using (Sftp client = new Sftp())
                    {
                        client.Connect(SftpUri);
                        client.Login(Username, Password);
                        client.ChangeDirectory(folderName);
                        client.GetFile(fileName, stream);
                    }
                }
            );
        }

        /// <summary>
        /// Sftp username
        /// </summary>
        private string Username;

        /// <summary>
        /// Sftp password
        /// </summary>
        private string Password;

        /// <summary>
        /// Sftp uri
        /// </summary>
        private string SftpUri;
    }
}