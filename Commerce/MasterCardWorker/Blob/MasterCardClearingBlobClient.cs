//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
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
    /// Contains methods to upload clearing files to the blob store and to mark them as having been uploaded to MasterCard.
    /// </summary>
    public class MasterCardClearingBlobClient : MasterCardBlobClient
    {
        /// <summary>
        /// Initializes a new instance of the MasterCardBlobClient class.
        /// </summary>
        /// <remarks>
        /// This constructor is meant for test use only.
        /// </remarks>
        public MasterCardClearingBlobClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MasterCardBlobClient class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        public MasterCardClearingBlobClient(CommerceLog log)
            : base(ClearingFolderName, ClearingFileTypeDescription, log)
        {
        }

        /// <summary>
        /// Gets the list of files to process.
        /// </summary>
        /// <returns>
        /// The list of files to be processed.
        /// </returns>
        public override ICollection<string> RetrieveNamesOfPendingFiles()
        {
            ICollection<string> result = new Collection<string>();

            ICollection<IListBlobItem> blobItems = RetrieveFilesInDirectory(Decoration);
            if (blobItems != null)
            {
                foreach (IListBlobItem blobItem in blobItems)
                {
                    CloudBlockBlob blob = (CloudBlockBlob)blobItem;
                    if (blob != null)
                    {
                        // Remove the directory path from the filename.
                        string[] tokens = blob.Name.Split('/');
                        string name = tokens[tokens.Length - 1];
                        result.Add(name);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the decoration prepended to the name of files that have yet to be uploaded to MasterCard.
        /// </summary>
        protected override string UnprocessedDecoration
        {
            get
            {
                return Decoration;
            }
        }

        /// <summary>
        /// The name of the folder in which clearing files will be uploaded.
        /// </summary>
        private const string ClearingFolderName = "mastercard-clearingfiles";

        /// <summary>
        /// The description for MasterCard clearing files.
        /// </summary>
        private const string ClearingFileTypeDescription = "MasterCard Clearing";

        /// <summary>
        /// The decoration prepended to the name of files that have yet to be uploaded to MasterCard.
        /// </summary>
        private const string Decoration = "ToBeProcessed";
    }
}