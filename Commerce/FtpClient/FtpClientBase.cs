//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FtpClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Base class for FTP clients.
    /// </summary>
    public abstract class FtpClientBase : IFtpClient
    {
        /// <summary>
        /// Creates a new instance of the FtpsClientBase class.
        /// </summary>
        /// <param name="credentials">
        /// The creditals to use in the FTP request.
        /// </param>
        /// <param name="certificates">
        /// The certificates to add to the FTP request, if any.
        /// </param>
        /// <param name="ssl">
        /// Specifies whether to connect using SSL.
        /// </param>
        /// <param name="keepAlive">
        /// Specifies whether to keep the connection to the FTP server alive after operation has completed.
        /// </param>
        /// <param name="usePassive">
        /// Specifies whether to send the use passive command to the server.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        protected FtpClientBase(ICredentials credentials,
                                X509CertificateCollection certificates,
                                bool ssl,
                                bool keepAlive,
                                bool usePassive,
                                CommerceLog log)
        {
            Credentials = credentials;
            Certificates = certificates;
            Ssl = ssl;
            KeepAlive = keepAlive;
            UsePassive = usePassive;
            Log = log;
        }

        /// <summary>
        /// Gets a list of all file names in a directory.
        /// </summary>
        /// <returns>
        /// An array of file names as strings
        /// </returns>
        public virtual async Task<string[]> DirectoryListAsync()
        {
            string[] result = null;

            List<string> fileList = new List<string>();

            // We've had tons of trouble with doing this async with MasterCard. Rework to be asynchronous again when we can figure out the problem.
            int tryCount = 0;
            int delay = 500;
            bool complete = false;
            while (complete == false && tryCount < 4)
            {
                tryCount++;
                WebResponse webResponse = null;
                Stream responseStream = null;
                StreamReader streamReader = null;
                try
                {
                    // Get the list of files in the FTP directory.
                    Log.Verbose("Sending request to get file list from FTP directory {0}.", FtpDirectory);
                    FtpWebRequest ftpWebRequest = ConstructFtpRequest(BuildFtpRequestUri(null));
                    ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    webResponse = ftpWebRequest.GetResponse();
                    responseStream = webResponse.GetResponseStream();
                    if (responseStream != null)
                    {
//TODO: First Data implementation weeded out anything that did not end in .TXT, but other servers return other info. For instance, MasterCard returns detailed
//      info for both ListDirectory and ListDirectoryDetails. Therefore, before FirstData can be migrated to this platform, it will have to adapt to interpret the
//      results on its own. See MasterCardFtpClientBase and its derived classes for examples.
                        streamReader = new StreamReader(responseStream);

                        string line = streamReader.ReadLine();
                        while (line != null)
                        {
                            // Add the returned name to the list.
                            if (String.IsNullOrWhiteSpace(line) == false)
                            {
                                fileList.Add(line);
                            }

                            line = streamReader.ReadLine();
                        }
                    }
                    
                    if (fileList.Count > 0)
                    {
                        result = fileList.ToArray();
                    }

                    // List all found files in the log.
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (string fileName in fileList)
                    {
                        stringBuilder.AppendLine(fileName);
                    }
                    Log.Verbose("The following {0} files are available in FTP directory {1}:\r\n{2}", fileList.Count, FtpDirectory, stringBuilder.ToString());
                   
                    complete = true;
                }
                catch (Exception ex)
                {
                    delay *= 2;
                    string message = String.Format("{0} encountered attempting to establish connection. Message: {1} ", ex.GetType(), ex.Message);
                    if (tryCount < 4)
                    {
                        Log.Warning(message + String.Format("Retry in {0} milliseconds.", delay));
                        Thread.Sleep(delay);
                    }
                    else
                    {
                        Log.Warning(message + "There will be no further retries.");
                        throw;
                    }
                }
                finally
                {
                    if (streamReader != null)
                    {
                        streamReader.Close();
                        streamReader = null;
                    }

                    if (responseStream != null)
                    {
                        responseStream.Close();
                        responseStream = null;
                    }

                    if (webResponse != null)
                    {
                        webResponse.Close();
                        webResponse = null;
                    }
                }
            }

            return result;

//            try
//            {
//                // Get the list of files in the FTP directory.
//                Log.Verbose("Sending request to get file list from FTP directory {0}.", FtpDirectory);
//                FtpWebRequest ftpWebRequest = ConstructFtpRequest(BuildFtpRequestUri(null));
//                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
//                using (WebResponse response = await ftpWebRequest.GetResponseAsync())
//                {
//                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
//                    {
////TODO: First Data implementation weeded out anything that did not end in .TXT, but other servers return other info. For instance, MasterCard returns detailed
////      info for both ListDirectory and ListDirectoryDetails. Therefore, before FirstData can be migrated to this platform, it will have to adapt to interpret the
////      results on its own. See MasterCardFtpClientBase and its derived classes for examples.
//                        string line = streamReader.ReadLine();
//                        while (line != null)
//                        {
//                            // Add the returned name to the list.
//                            if (String.IsNullOrWhiteSpace(line) == false)
//                            {
//                                fileList.Add(line);
//                            }

//                            line = streamReader.ReadLine();
//                        }
//                    }
//                }

//                if (fileList.Count > 0)
//                {
//                    result = fileList.ToArray();
//                }

//                // List all found files in the log.
//                StringBuilder stringBuilder = new StringBuilder();
//                foreach (string fileName in fileList)
//                {
//                    stringBuilder.AppendLine(fileName);
//                }
//                Log.Verbose("The following {0} files are available in FTP directory {1}:\r\n{2}", fileList.Count, FtpDirectory, stringBuilder.ToString());
//            }
//            catch(Exception exception)
//            {
//                Log.Error("Unhandled exception encountered during directory list operation.\r\nDirectory: {0}\r\nUri: {1}", exception,
//                          FtpDirectory, AddressAuthority);
//                throw;
//            }
            
//            return result;
        }

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
        public async Task UploadFileAsync(string fileName,
                                          Stream stream)
        {
            try
            {
                //// Upload the specified stream to the FTP directory under the specified file name.
                //Log.Verbose("Sending request to upload file {0} to FTP directory {1}.", fileName, FtpDirectory);

                //int tryCount = 0;
                //int delay = 500;
                //bool complete = false;
                //while (complete == false && tryCount < 4)
                //{
                //    tryCount++;
                //    try
                //    {
                //        FtpWebRequest ftpWebRequest = ConstructFtpRequest(BuildFtpRequestUri(fileName));
                //        ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                //        int bufferLength = 2048;
                //        byte[] buffer = new byte[bufferLength];

                //        using (Stream uploadStream = await ftpWebRequest.GetRequestStreamAsync().ConfigureAwait(false))
                //        {
                //            int contentLength = stream.Read(buffer, 0, bufferLength);
                //            while (contentLength != 0)
                //            {
                //                uploadStream.Write(buffer, 0, contentLength);
                //                contentLength = stream.Read(buffer, 0, bufferLength);
                //            }
                //        }

                //        complete = true;
                //    }
                //    catch (Exception ex)
                //    {
                //        delay *= 2;
                //        string message = String.Format("{0} encountered attempting to establish connection. ", ex.GetType());
                //        if (tryCount < 4)
                //        {
                //            Log.Warning(message + String.Format("Retry in {0} milliseconds.", delay));
                //            Thread.Sleep(delay);
                //        }
                //        else
                //        {
                //            Log.Warning(message + "There will be no further retries.");
                //            throw;
                //        }
                //    }
                //}

//TODO: We were losing some information in the abstractions provided by the using and async/await patterns and asynchronous flow but these are now under control.
//      We've re-instated the original flow with new retry mechanisms. The commented out code can be removed.
                int tryCount = 0;
                int delay = 500;
                bool complete = false;
                while (complete == false && tryCount < 4)
                {
                    tryCount++;
                    Stream uploadStream = null;
                    try
                    {
                        FtpWebRequest ftpWebRequest = ConstructFtpRequest(BuildFtpRequestUri(fileName));
                        ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                        int bufferLength = 2048;
                        byte[] buffer = new byte[bufferLength];

                        uploadStream = ftpWebRequest.GetRequestStream();
                        int contentLength = stream.Read(buffer, 0, bufferLength);
                        while (contentLength != 0)
                        {
                            uploadStream.Write(buffer, 0, contentLength);
                            contentLength = stream.Read(buffer, 0, bufferLength);
                        }

                        complete = true;
                    }
                    catch (SystemException ex)
                    {
                        delay *= 2;
                        string message = String.Format("{0} encountered attempting to establish connection. ", ex.GetType());
                        if (tryCount < 4)
                        {
                            Log.Warning(message + String.Format("Retry in {0} milliseconds.", delay));
                            Thread.Sleep(delay);
                        }
                        else
                        {
                            Log.Warning(message + "There will be no further retries.");
                            throw;
                        }
                    }
                    finally
                    {
                        if (uploadStream != null)
                        {
                            uploadStream.Close();
                            uploadStream.Dispose();
                            uploadStream = null;
                        }
                    }
                }

                Log.Verbose("Request to upload file {0} to directory {1} completed.", fileName, FtpDirectory);
            }
            catch (Exception exception)
            {
                Log.Error("Unhandled exception encountered during file upload operation.\r\nDirectory: {0}\r\nFile: {1}\r\nUri: {2}",
                          exception, FtpDirectory, fileName, AddressAuthority);
                throw;
            }
        }

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
        public async Task DownloadFileAsync(string fileName,
                                            Stream stream)
        {

            // We've had tons of trouble with doing this async with MasterCard. Rework to be asynchronous again when we can figure out the problem.
            int tryCount = 0;
            int delay = 500;
            bool complete = false;
            while (complete == false && tryCount < 4)
            {
                tryCount++;
                Stream responseStream = null;
                WebResponse webResponse = null;
                try
                {
                    FtpWebRequest ftpWebRequest = ConstructFtpRequest(BuildFtpRequestUri(fileName));
                    ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    webResponse = ftpWebRequest.GetResponse();
                    responseStream = webResponse.GetResponseStream();
                    if (responseStream != null)
                    {
                        responseStream.CopyTo(stream);
                    }

                    Log.Verbose("Request to download file {0} from directory {1} completed.", fileName, FtpDirectory);
                    complete = true;
                }
                catch (SystemException ex)
                {
                    delay *= 2;
                    string message = String.Format("{0} encountered attempting to establish connection. ", ex.GetType());
                    if (tryCount < 4)
                    {
                        Log.Warning(message + String.Format("Retry in {0} milliseconds.", delay));
                        Thread.Sleep(delay);
                    }
                    else
                    {
                        Log.Warning(message + "There will be no further retries.");
                        throw;
                    }
                }
                finally
                {
                    if (responseStream != null)
                    {
                        responseStream.Close();
                        responseStream.Dispose();
                        responseStream = null;
                    }

                    if (webResponse != null)
                    {
                        webResponse.Close();
                        webResponse.Dispose();
                        webResponse = null;
                    }
                }
            }

            //try
            //{
            //    // Download the specified file from the FTP directory into the specified stream.
            //    Log.Verbose("Sending request to download file {0} from directory {1}.", fileName, FtpDirectory);
            //    FtpWebRequest ftpWebRequest = ConstructFtpRequest(BuildFtpRequestUri(fileName));
            //    ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            //    using (WebResponse response = await ftpWebRequest.GetResponseAsync())
            //    {
            //        using (Stream responseStream = response.GetResponseStream())
            //        {
            //            if (responseStream != null)
            //            {
            //                responseStream.CopyTo(stream);
            //            }
            //        }
            //    }
            //    Log.Verbose("Request to download file {0} from directory {1} completed.", fileName, FtpDirectory);
            //}
            //catch (WebException exception)
            //{
            //    Log.Error("Web exception encountered during file download operation. Specified file may be a directory." +
            //              "\r\nDirectory: {0}\r\nFile: {1}\r\nUri: {2}", exception, FtpDirectory, fileName, AddressAuthority);
            //}
            //catch (Exception exception)
            //{
            //    Log.Error("Unhandled exception encountered during file download operation.\r\nDirectory: {0}\r\nFile: {1}\r\nUri: {2}",
            //              exception, FtpDirectory, fileName, AddressAuthority);
            //    throw;
            //}
           
        }

        /// <summary>
        /// Gets or set the base of the address to which the client will connect.
        /// </summary>
        protected abstract string AddressAuthority { get; }

        /// <summary>
        /// Gets the name of the directory involved in FTP transfers.
        /// </summary>
        protected abstract string FtpDirectory { get; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        protected CommerceLog Log { get; set; }

        /// <summary>
        /// Build the URI to use for the FTP request.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to transfer, if any. Defaults to null.
        /// </param>
        /// <returns>
        /// The URI to use for the FTP request.
        /// </returns>
        private Uri BuildFtpRequestUri(string fileName = null)
        {
            // Build the fully qualified URI string, ensuring proper number of slashes after each segment.
            StringBuilder stringBuilder = new StringBuilder("ftp://");
            stringBuilder.Append(AddressAuthority.Trim('/'));
//TODO: Need double slash after port?
            stringBuilder.Append("/");
            stringBuilder.Append(FtpDirectory.Trim('/'));
            if (String.IsNullOrWhiteSpace(fileName) == false)
            {
                stringBuilder.Append("/");
                stringBuilder.Append(fileName.Trim('/'));
            }

            return new Uri(stringBuilder.ToString());
        }

        /// <summary>
        /// Constructs an FTP request.
        /// </summary>
        /// <param name="uri">
        /// The URI to use for the FTP request.
        /// </param>
        /// <returns>
        /// The FtpWebRequest object through which an FTP request can be sent.
        /// </returns>
        private FtpWebRequest ConstructFtpRequest(Uri uri)
        {
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(uri);
            result.Timeout = 50000;

            if (Credentials != null)
            {
                result.Credentials = Credentials;
            }

            if (Certificates != null)
            {
                result.ClientCertificates = Certificates;
            }

            result.EnableSsl = Ssl;
            result.KeepAlive = KeepAlive;
            result.UsePassive = UsePassive;

            return result;
        }

        /// <summary>
        /// The creditals to use in the FTP request.
        /// </summary>
        private ICredentials Credentials;

        /// <summary>
        /// The certificates to add to the FTP request, if any.
        /// </summary>
        private X509CertificateCollection Certificates;

        /// <summary>
        /// Specifies whether to connect using SSL.
        /// </summary>
        private bool Ssl;

        /// <param name="keepAlive">
        /// Specifies whether to keep the connection to the FTP server alive after operation has completed.
        /// </param>
        private bool KeepAlive;

        /// <param name="keepAlive">
        /// Specifies whether to send the use passive command to the server.
        /// </param>
        private bool UsePassive;
    }
}