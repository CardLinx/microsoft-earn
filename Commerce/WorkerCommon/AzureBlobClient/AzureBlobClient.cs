//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Generic Azure Blob Client
    /// </summary>
    public abstract class AzureBlobClient
    {
        /// <summary>
        /// Default constructor to support mocking
        /// </summary>
        protected AzureBlobClient(){}

        /// <summary>
        /// Azure Blob Client Constructor
        /// </summary>
        /// <param name="connectionString">
        /// Storage Connection String
        /// </param>
        /// <param name="containerName">
        /// Container Name
        /// </param>
        protected AzureBlobClient(string connectionString, string containerName)
        {
            this.connectionString = connectionString;
            this.containerName = containerName;
            CreateContainer();
        }

        /// <summary>
        /// Upload Stream to BlobStore with a given name
        /// </summary>
        /// <param name="stream">
        /// Stream to upload
        /// </param>
        /// <param name="blobName">
        /// Name of the blob
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        protected async Task UploadBlobAsync(Stream stream, string blobName)
        {
            CloudBlockBlob blockBlob = Container.GetBlockBlobReference(blobName);
            await Task.Factory.FromAsync(
                blockBlob.BeginUploadFromStream(stream, null, null),
                blockBlob.EndUploadFromStream).ConfigureAwait(false);
        }

        /// <summary>
        /// Download Stream from BlobStore with a given name
        /// </summary>
        /// <param name="stream">
        /// Stream to download to
        /// </param>
        /// <param name="blobName">
        /// Name of the blob to download
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        /// <remarks>
        /// Internal for Testing. Should not be called directly.
        /// </remarks>
        public async Task DownloadBlobAsync(Stream stream, string blobName)
        {
            CloudBlockBlob blockBlob = Container.GetBlockBlobReference(blobName);
            await Task.Factory.FromAsync(
                blockBlob.BeginDownloadToStream(stream, null, null),
                blockBlob.EndDownloadToStream).ConfigureAwait(false);
        }

        /// <summary>
        /// Get losting of blobs in a given directory
        /// </summary>
        /// <param name="directoryName">
        /// Name of the directory
        /// </param>
        /// <returns>
        /// Collection of BlobItems
        /// </returns>
        protected ICollection<IListBlobItem> RetrieveFilesInDirectory(string directoryName)
        {
            CloudBlobDirectory directory = Container.GetDirectoryReference(directoryName);
            List<IListBlobItem> blobs = directory.ListBlobs().ToList();
            return blobs;
        }

        /// <summary>
        /// Move a blob from one directory to another
        /// </summary>
        /// <param name="oldName">
        /// Old Name
        /// </param>
        /// <param name="newName">
        /// New Name
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        protected async Task MoveBlobAsync(string oldName, string newName)
        {
            ICloudBlob oldBlob = Container.GetBlobReferenceFromServer(oldName);
            ICloudBlob newBlob = Container.GetBlockBlobReference(newName);
            if (oldBlob != null)
            {
                await Task<string>.Factory.FromAsync<Uri>(
                    newBlob.BeginStartCopyFromBlob,
                    newBlob.EndStartCopyFromBlob,
                    oldBlob.Uri,
                    null).ConfigureAwait(false);

                await Task.Factory.FromAsync(
                    oldBlob.BeginDelete,
                    oldBlob.EndDelete,
                    null).ConfigureAwait(false);
            }
        }

        

        /// <summary>
        /// Creates a blob container
        /// </summary>
        private void CreateContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            blobClient = storageAccount.CreateCloudBlobClient();
            Container = blobClient.GetContainerReference(containerName);
            Container.CreateIfNotExists();
        }

        /// <summary>
        /// Handle to blob container
        /// </summary>
        /// <remarks>
        /// Internal so that tests can directly use this 
        /// </remarks>
        internal CloudBlobContainer Container;

        /// <summary>
        /// Name of the container
        /// </summary>
        private string containerName;

        /// <summary>
        /// Connection String for storage account
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Handle to blob client
        /// </summary>
        private CloudBlobClient blobClient;

        
    }
}