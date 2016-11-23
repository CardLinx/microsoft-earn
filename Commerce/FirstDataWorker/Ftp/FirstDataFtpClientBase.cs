//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;
    using Microsoft.Azure;

    /// <summary>
    /// Base class for FDC FTP Clients
    /// </summary>
    public abstract class FirstDataFtpClientBase : IFtpClient
    {
        /// <summary>
        /// Creates an instance 
        /// </summary>
        /// <param name="credentials">
        /// FTP Credentials
        /// </param>
        /// <param name="ssl">
        /// Should use SSL or not
        /// </param>
        /// <param name="log">
        /// Handle to Commerce Logger
        /// </param>
        protected FirstDataFtpClientBase(ICredentials credentials, bool ssl, CommerceLog log)
        {
            Credentials = credentials;
            Ssl = ssl;
            Log = log;
        }

        /// <summary>
        /// Get all file names in a directory
        /// </summary>
        /// <returns>
        /// Array of file names as strings
        /// </returns>
        public async Task<string[]> DirectoryListAsync()
        {
            try
            {
                Log.Verbose("Request to get file list from directory {0}", RemoteDirectory);
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftpRequest = ConstructFtpRequest(null);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

                using (WebResponse response = await ftpRequest.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            // We only care about log files in .txt extensions
                            if (line.EndsWith(".txt", true, CultureInfo.InvariantCulture))
                            {
                                result.Append(line);
                                result.Append(";");
                            }
                            line = reader.ReadLine();
                        }
                        if (!string.IsNullOrEmpty(result.ToString()))
                        {
                            result.Remove(result.ToString().LastIndexOf(';'), 1);
                            string[] fileList = result.ToString().Split(';');
                            Log.Information("Following files are available in {0} directory", RemoteDirectory);
                            foreach (string fileName in fileList)
                            {
                                Log.Information("File Name : {0}", fileName);
                            }
                            return result.ToString().Split(';');
                        }

                        Log.Information("No files are available in {0} directory", RemoteDirectory);
                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(
                    "Error during directory list operation. " +
                    "Directory : {0} , Uri : {1}",
                    exception,
                    RemoteDirectory,
                    FtpUri);

                throw;
            }
            
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
        public async Task UploadFileAsync(string fileName, Stream stream)
        {
            try
            {
                Log.Verbose("Request to Upload file {0} to directory {1}", fileName, RemoteDirectory);
                FtpWebRequest ftpRequest = ConstructFtpRequest(fileName);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];

                using (Stream uploadStream = await ftpRequest.GetRequestStreamAsync())
                {
                    int contentLength = stream.Read(buffer, 0, bufferLength);

                    while (contentLength != 0)
                    {
                        uploadStream.Write(buffer, 0, contentLength);
                        contentLength = stream.Read(buffer, 0, bufferLength);
                    }
                }
                Log.Verbose("Request to Upload file {0} to directory {1} completed", fileName, RemoteDirectory);
            }
            catch (Exception exception)
            {
                // log and throw
                Log.Error(
                    "Error during Upload operation. " +
                    "Directory : {0} , File {1}, Uri : {2}",
                    exception,
                    RemoteDirectory,
                    fileName,
                    FtpUri);

                throw;
            }
            
        }

        /// <summary>
        /// Download a file 
        /// </summary>
        /// <param name="fileName">
        /// Name of the file to download
        /// </param>
        /// <param name="stream">
        /// Memory Stream to download file to
        /// </param>
        /// <returns>
        /// File Content as stream data
        /// </returns>
        public async Task DownloadFileAsync(string fileName, Stream stream)
        {
            Log.Verbose("Request to Download file {0} from directory {1}", fileName, RemoteDirectory);
            
            // MemoryStream memStream = new MemoryStream();
            FtpWebRequest ftpRequest = ConstructFtpRequest(fileName);
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            try
            {
                using (WebResponse response = await ftpRequest.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            responseStream.CopyTo(stream);
                        }
                    }
                }

                Log.Verbose("Request to Download file {0} from directory {1} complete", fileName, RemoteDirectory);
            }
            catch (Exception exception)
            {
                // log and throw
                Log.Error(
                    "Error during Download operation. " +
                    "Directory : {0} , File {1}, Uri : {2}",
                    exception,
                    RemoteDirectory,
                    fileName,
                    FtpUri);

                throw;
            }
           
        }

        /// <summary>
        /// Handle to the Remote Directory
        /// </summary>
        public abstract string RemoteDirectory
        {
            get;
        }

        /// <summary>
        /// Converts uri like http://contoso.com to http://contoso.com/
        /// </summary>
        /// <param name="uri">
        /// uri to format
        /// </param>
        /// <returns>
        /// formatted uri
        /// </returns>
        internal static string PrependSlashBeforeFileName(string uri)
        {
            if (!uri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return uri + "/";
            }

            return uri;
        }

        /// <summary>
        /// Get Property Value for FTP Ops given a name
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
        /// Construct FTP request 
        /// </summary>
        /// <param name="suffix">
        /// Optional suffix -> file Name to upload/download
        /// </param>
        /// <returns>
        /// Newly contstructed FTP Web Request 
        /// </returns>
        private FtpWebRequest ConstructFtpRequest(string suffix)
        {
            string uri = FtpUri + RemoteDirectory;
            if (!string.IsNullOrEmpty(suffix))
            {
                // put / before file name
                uri = PrependSlashBeforeFileName(uri);
                uri += suffix; 
            }

            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            ftpRequest.UsePassive = true;
            ftpRequest.EnableSsl = Ssl;
            if (Credentials != null)
            {
                ftpRequest.Credentials = Credentials; 
            }
            return ftpRequest;
        }
        
        /// <summary>
        /// FTP Server URI
        /// </summary>
        internal string FtpUri = GetPropertyValue(FirstDataFtpConstants.FirstDataFtpUriPropertyName);

        /// <summary>
        /// Commerce Log Handler
        /// </summary>
        private CommerceLog Log;

        /// <summary>
        /// Use ssl or not
        /// </summary>
        private bool Ssl;

        /// <summary>
        /// FTP Credentials
        /// </summary>
        private ICredentials Credentials;
    }
}