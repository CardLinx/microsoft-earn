//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;
    using Microsoft.Azure;

    /// <summary>
    /// Contains methods to upload files to the blob store and to mark them as having been uploaded to MasterCard.
    /// </summary>
    public abstract class MasterCardBlobClient : AzureBlobClient
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardBlobClient class.
        /// </summary>
        /// <remarks>
        /// This constructor is meant for test use only.
        /// </remarks>
        protected MasterCardBlobClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MasterCardBlobClient class.
        /// </summary>
        /// <param name="folderName">
        /// The name of the folder in which files will be uploaded.
        /// </param>
        /// <param name="fileTypeDescription">
        /// The description for the type of file this object is addressing.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        protected MasterCardBlobClient(string folderName,
                                    string fileTypeDescription,
                                    CommerceLog log)
            : base(ConnectionString, folderName)
        {
            FileTypeDescription = fileTypeDescription;
            Log = log;
        }

        /// <summary>
        /// Downloads the contents of the specified file into the specified stream.
        /// </summary>
        /// <param name="stream">
        /// Stream into which to load file contents.
        /// </param>
        /// <param name="fileName">
        /// The name the file whose contents to download.
        /// </param>
        /// <returns>
        /// A Task that will perform the download operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter fileName cannot be null.
        /// -OR-
        /// Parameter stream cannot be null.
        /// </exception>
        public async virtual Task DownloadAsync(Stream stream,
                                                string fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Parameter stream cannot be null.");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "Parameter fileName cannot be null.");
            }

            Log.Verbose("Downloading file {0} from {1} blob store.", fileName, FileTypeDescription);
            fileName = String.Concat(UnprocessedDecoration, "/", fileName);
            await DownloadBlobAsync(stream, fileName).ConfigureAwait(false);
            Log.Verbose("Download file {0} from {1} Blob Store complete.", fileName, FileTypeDescription);
        }

        /// <summary>
        /// Uploads the contents of the specified stream to the blob store under a decorated version of the specified file name.
        /// </summary>
        /// <param name="stream">
        /// Stream containing file contents.
        /// </param>
        /// <param name="fileName">
        /// The name of the file under which to store stream contents.
        /// </param>
        /// <returns>
        /// A Task that will perform the upload operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter fileName cannot be null.
        /// -OR-
        /// Parameter stream cannot be null.
        /// </exception>
        public async virtual Task UploadAsync(Stream stream,
                                              string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "Parameter fileName cannot be null.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Parameter stream cannot be null.");
            }

            Log.Verbose("Uploading file {0} to {1} blob store.", fileName, FileTypeDescription);
            fileName = String.Concat(UnprocessedDecoration, "/", fileName);
            await UploadBlobAsync(stream, fileName).ConfigureAwait(false);
            Log.Verbose("Upload of file {0} to {1} blob store complete.", fileName, FileTypeDescription);
        }

        /// <summary>
        /// Marks the file with the specified name as having been uploaded to MasterCard.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file to mark.
        /// </param>
        /// <returns>
        /// A Task that will perform the mark operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter fileName cannot be null.
        /// </exception>
        public async virtual Task MarkAsCompleteAsync(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName", "Parameter fileName cannot be null.");
            }

            Log.Verbose("Marking file {0} in {1} blob store as Uploaded.", fileName, FileTypeDescription);
            string oldFileName = String.Concat(UnprocessedDecoration, "/", fileName);
            string newFileName = fileName;
            await MoveBlobAsync(oldFileName, newFileName).ConfigureAwait(false);
            Log.Verbose("Marking file {0} in {1} blob store as Uploaded complete.", fileName, FileTypeDescription);
        }

        /// <summary>
        /// Gets the names of pending files of the type being processed.
        /// </summary>
        /// <returns>
        /// The list of the names of pending files.
        /// </returns>
        public virtual ICollection<string> RetrieveNamesOfPendingFiles()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the decoration prepended to the name of files that have yet to be uploaded to MasterCard.
        /// </summary>
        protected abstract string UnprocessedDecoration { get; }

        /// <summary>
        /// The description for the type of file this object is addressing.
        /// </summary>
        public string FileTypeDescription { get; set; }

        /// <summary>
        /// The CommerceLog object within which to place log entries.
        /// </summary>
        private CommerceLog Log;

        /// <summary>
        /// The connection string to the MasterCard blob store.
        /// </summary>
        private static string ConnectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.MasterCard.Blob.ConnectionString");
    }
}