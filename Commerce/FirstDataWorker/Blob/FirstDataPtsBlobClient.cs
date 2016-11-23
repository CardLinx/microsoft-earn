//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Class to interact with Pts File Blob Store
    /// </summary>
    public class FirstDataPtsBlobClient : AzureBlobClient
    {
         /// <summary>
        /// No-arg default constructor used for Mocking
        /// as Mock extends this class
        /// </summary>
        /// <remarks>
        /// For tests, do not call directly
        /// </remarks>
        public FirstDataPtsBlobClient()
        {
        }

        /// <summary>
        /// Constructor to create Extract Pts Client Instance
        /// </summary>
        /// <param name="connectionString">
        /// Azure Storage Connection String
        /// </param>
        public FirstDataPtsBlobClient(string connectionString, CommerceLog log) 
            : base(connectionString, "fdc-ptsfiles")
        {
            Log = log;
        }

        /// <summary>
        /// Upload Pts File to Blob Store
        /// to archive it.
        /// </summary>
        /// <param name="stream">
        /// File Data as Stream
        /// </param>
        /// <param name="fileName">
        /// Name of the file
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

            Log.Verbose("Uploading file {0} to Pts Blob Store", fileName);
            fileName = ToBeUploaded + "/" + fileName;
            await UploadBlobAsync(stream, fileName).ConfigureAwait(false);
            Log.Verbose("Upload of file {0} to Pts Blob Store complete", fileName);
        }

        /// <summary>
        /// Mark a file as uploaded
        /// </summary>
        /// <param name="fileName">
        /// Name of the file
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async virtual Task MarkAsUploadedAsync(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "fileName cannot be null");
            }

            Log.Verbose("Marking file {0} in Pts Blob Store as Uploaded", fileName);
            string oldFileName = ToBeUploaded + "/" + fileName;
            string newFileName = fileName;
            await MoveBlobAsync(oldFileName, newFileName).ConfigureAwait(false);
            Log.Verbose("Marking file {0} in Pts Blob Store as Uploaded complete", fileName);
        }

        /// <summary>
        /// Direcotry we put unprocessed files into - virtual
        /// </summary>
        private readonly string ToBeUploaded = "ToBeUploaded";

        /// <summary>
        /// Handle to Commerce log
        /// </summary>
        private CommerceLog Log;
    }
}