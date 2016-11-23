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
    /// Class to interact with Amex Merchant Offer Registration Record Blob Store
    /// </summary>
    public class OfferRegistrationRecordBlobClient : AzureBlobClient
    {
        /// <summary>
        /// No-arg default constructor used for Mocking
        /// as Mock extends this class
        /// </summary>
        /// <remarks>
        /// For tests, do not call directly
        /// </remarks>
        public OfferRegistrationRecordBlobClient()
        {
        }

        /// <summary>
        /// Constructor to create Merchant Offer Registration Record Blob Client Instance
        /// </summary>
        /// <param name="connectionString">
        /// Azure Storage Connection String
        /// </param>
        public OfferRegistrationRecordBlobClient(string connectionString, CommerceLog log) 
            : base(connectionString, "amex-offer-registrationrecords")
        {
            Log = log;
        }

        /// <summary>
        /// Upload Merchant Offer Registration Record to Blob Store
        /// </summary>
        /// <param name="stream">
        /// Record Data as Stream
        /// </param>
        /// <param name="recordName">
        /// Name of the Record
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task UploadAsync(Stream stream, string recordName)
        {
            if (recordName == null)
            {
                throw new ArgumentNullException("recordName", "recordName cannot be null");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream", "stream cannot be null");
            }

            Log.Verbose("Uploading record {0} to Amex Merchant Offer Registration Record Blob Store to be processed", recordName);
            recordName = ToBeProcessedDirectory + "/" + recordName;
            await UploadBlobAsync(stream, recordName).ConfigureAwait(false);
            Log.Verbose("Upload of file {0} to Merchant Offer Registration Blob Store to be processed complete", recordName);
        }

        /// <summary>
        /// Download Merchant Offer Registration record from Blob Store
        /// </summary>
        /// <param name="stream">
        /// Stream to copy the data to
        /// </param>
        /// <param name="recordName">
        /// Name of the record to download
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task DownloadAsync(Stream stream, string recordName)
        {
            if (recordName == null)
            {
                throw new ArgumentNullException("recordName", "recordName cannot be null");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream", "stream cannot be null");
            }

            Log.Verbose("Downloading record {0} from Merchant Offer Registration Blob Store to be processed", recordName);
            recordName = ToBeProcessedDirectory + "/" + recordName;
            await DownloadBlobAsync(stream, recordName).ConfigureAwait(false);
            Log.Verbose("Download record {0} from Merchant Offer Registration Blob Store complete", recordName);
        }

        /// <summary>
        /// Mark a Merchant Offer Registration record as processed
        /// </summary>
        /// <param name="recordName">
        /// Name of the record
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task MarkAsProcessedAsync(string recordName)
        {
            if (recordName == null)
            {
                throw new ArgumentNullException("recordName", "recordName cannot be null");
            }

            Log.Verbose("Marking record {0} in Merchant Offer Registration Blob Store as Processed", recordName);
            string oldFileName = ToBeProcessedDirectory + "/" + recordName;
            string newFileName = recordName;
            await MoveBlobAsync(oldFileName, newFileName).ConfigureAwait(false);
            Log.Verbose("Marking record {0} in Merchant Offer Registration Blob Store as Processed complete", recordName);
        }

        /// <summary>
        /// Gets list of records to process.
        /// </summary>
        /// <returns>
        /// List of record names
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