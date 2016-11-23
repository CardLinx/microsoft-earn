//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Utils.Interface;
using Lomo.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Azure.Utils
{
    public class AzureBlob : IAzureBlob
    {
        private readonly CloudStorageAccount cloudStorageAccount;

        private readonly CloudBlobClient blobClient;

        public AzureBlob(string account, IRetryPolicy retryPolicy = null)
        {
            this.cloudStorageAccount = CloudStorageAccount.Parse(account);
            blobClient = this.cloudStorageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions.RetryPolicy = retryPolicy ?? new ExponentialRetry();
        }

        /// <summary>
        /// Uploads the text data to the azure blob
        /// </summary>
        /// <param name="containerName">Name of the blob container</param>
        /// <param name="blobName">Name of the blob</param>
        /// <param name="stream"></param>
        public async Task UploadBlobFromStreamAsync(string containerName, string blobName, Stream stream)
        {
            try
            {
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
                blobContainer.CreateIfNotExists();
                var blockBlob = blobContainer.GetBlockBlobReference(blobName);
                await blockBlob.UploadFromStreamAsync(stream).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error("Could not upload blob " + blobName + " " + containerName + " " + e.Message);
                throw;
            }
        }

        public async Task<MemoryStream> DownloadBlobToStreamAsync(string blobContainer, string blobName)
        {
            try
            {
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
                CloudBlockBlob cloudBlob = container.GetBlockBlobReference(blobName);
                MemoryStream ms = new MemoryStream();
                await cloudBlob.DownloadToStreamAsync(ms).ConfigureAwait(false);

                return ms;
            }
            catch (Exception e)
            {
                Log.Error("Error downloading blob {0} from {1} : {2}", blobName, blobContainer, e.Message);
                throw;
            }
        }
    }
}