//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Class to interact with Amex Statement Credit File Blob store
    /// </summary>
    public class AmexStatementCreditFileBlobClient : AzureBlobClient
    {
        /// <summary>
        /// No-arg default constructor used for Mocking
        /// as Mock extends this class
        /// </summary>
        /// <remarks>
        /// For tests, do not call directly
        /// </remarks>
        public AmexStatementCreditFileBlobClient()
        {
        }

         /// <summary>
        /// Constructor to create Amex Statement Credit File Blob Client Instance
        /// </summary>
        /// <param name="connectionString">
        /// Azure Storage Connection String
        /// </param>
        public AmexStatementCreditFileBlobClient(string connectionString, CommerceLog log) 
            : base(connectionString, "amex-statementcreditfiles")
        {
            Log = log;
        }

        /// <summary>
        /// Upload Amex Statement Credit File to Blob Store
        /// </summary>
        /// <param name="stream">
        /// Record Data as Stream
        /// </param>
        /// <param name="fileName">
        /// Name of the File
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task UploadAsync(Stream stream, string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "fileName cannot be null");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream", "stream cannot be null");
            }

            Log.Verbose("Uploading file {0} to  Amex Statement Credit File Blob Store to be processed", fileName);
            fileName = ToBeProcessedDirectory + "/" + fileName;
            await UploadBlobAsync(stream, fileName).ConfigureAwait(false);
            Log.Verbose("Upload of file {0} to Amex Statement Credit File Blob Store to be processed complete", fileName);
        }

        /// <summary>
        /// Download Amex Statement Credit File from Blob Store
        /// </summary>
        /// <param name="stream">
        /// Stream to copy the data to
        /// </param>
        /// <param name="fileName">
        /// Name of the file to download
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task DownloadAsync(Stream stream, string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "fileName cannot be null");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream", "stream cannot be null");
            }

            Log.Verbose("Downloading file {0} from Amex Statement Credit File Blob Store to be processed", fileName);
            fileName = ToBeProcessedDirectory + "/" + fileName;
            await DownloadBlobAsync(stream, fileName).ConfigureAwait(false);
            Log.Verbose("Download file {0} from Amex Statement Credit File Blob Store complete", fileName);
        }

        /// <summary>
        /// Mark a file as processed
        /// </summary>
        /// <param name="fileName">
        /// Name of the file
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task MarkAsProcessedAsync(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "fileName cannot be null");
            }

            Log.Verbose("Marking file {0} in Amex Statement Credit File Blob Store as Processed", fileName);
            string oldFileName = ToBeProcessedDirectory + "/" + fileName;
            string newFileName = fileName;
            await MoveBlobAsync(oldFileName, newFileName).ConfigureAwait(false);
            Log.Verbose("Marking file {0} in Amex Statement Credit File Blob Store as Processed complete", fileName);
        }

        /// <summary>
        /// Gets list of files to process.
        /// </summary>
        /// <returns>
        /// List of file names
        /// </returns>
        public virtual ICollection<string> RetrieveFilesToProcess()
        {
            ICollection<IListBlobItem> blobItems = RetrieveFilesInDirectory(ToBeProcessedDirectory);
            ICollection<string> files = new Collection<string>();
            if (blobItems != null)
            {
                foreach (IListBlobItem item in blobItems)
                {
                    CloudBlockBlob blob = item as CloudBlockBlob;
                    if (blob != null)
                    {
                        // get the actual file name without directory
                        string[] tokens = blob.Name.Split('/');
                        string name = tokens[tokens.Length - 1];
                        files.Add(name);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Direcotry we put unprocessed files into - virtual
        /// </summary>
        private readonly string ToBeProcessedDirectory = "ToBeProcessed";

        /// <summary>
        /// Handle to Commerce log
        /// </summary>
        private CommerceLog Log;
    }
}