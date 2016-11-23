//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Class to interact with Amex Statement Credit File Blob store
    /// </summary>
    public class AmexStatementCreditResponseFileBlobClient : AzureBlobClient
    {
        /// <summary>
        /// No-arg default constructor used for Mocking
        /// as Mock extends this class
        /// </summary>
        /// <remarks>
        /// For tests, do not call directly
        /// </remarks>
        public AmexStatementCreditResponseFileBlobClient()
        {
        }

        /// <summary>
        /// Constructor to create Amex Statement Credit File Blob Client Instance
        /// </summary>
        /// <param name="connectionString">
        /// Azure Storage Connection String
        /// </param>
        public AmexStatementCreditResponseFileBlobClient(string connectionString, CommerceLog log)
            : base(connectionString, "amex-statementcreditresponsefiles")
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
            await UploadBlobAsync(stream, fileName).ConfigureAwait(false);
            Log.Verbose("Upload of file {0} to Amex Statement Credit File Blob Store to be processed complete", fileName);
        }

        /// <summary>
        /// Handle to Commerce log
        /// </summary>
        private CommerceLog Log;
    }
}