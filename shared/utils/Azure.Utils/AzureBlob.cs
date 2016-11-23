//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Utility class to upload/download data to azure blob
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace Azure.Utils
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Lomo.Logging;

    /// <summary>
    /// Utility class to upload/download data to azure blob
    /// </summary>
    public class AzureBlob
    {
        private readonly CloudStorageAccount cloudStorageAccount;

        private readonly CloudBlobClient blobClient;

        #region Constructor

        public AzureBlob(string account, IRetryPolicy retryPolicy = null)
        {
            this.cloudStorageAccount = CloudStorageAccount.Parse(account);
            blobClient = this.cloudStorageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions.RetryPolicy = retryPolicy ?? new ExponentialRetry();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Uploads the items in the list to the azure blob
        /// </summary>
        /// <param name="containerName">Blob container name</param>
        /// <param name="blobName">Name of the blob to upload to</param>
        /// <param name="dataList">data to upload</param>
        public void UploadListToBlob(string containerName, string blobName, IEnumerable<string> dataList)
        {
            try
            {
                CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);
                blobContainer.CreateIfNotExists();
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        foreach (var dataItem in dataList)
                        {
                            sw.WriteLine(dataItem);
                        }
                        sw.Flush();
                        ms.Position = 0;
                        blockBlob.UploadFromStream(ms);
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error("Could not upload blob " + blobName + " " + containerName + " " + e.Message);
            }
        }

        /// <summary>
        /// Uploads the text data to the azure blob
        /// </summary>
        /// <param name="containerName">Name of the blob container</param>
        /// <param name="blobName">Name of the blob</param>
        /// <param name="data">Text data to upload</param>
        public void UploadTextToBlob(string containerName, string blobName, string data)
        {
            try
            {
                CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);
                blobContainer.CreateIfNotExists();
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        sw.Write(data);
                        sw.Flush();
                        ms.Position = 0;
                        blockBlob.UploadFromStream(ms);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload blob " + blobName + " " + containerName + " " + e.Message);
            }
        }
        
        /// <summary>
        /// Uploads a data list as a block to the block blob
        /// </summary>
        /// <param name="containerName">Name of the blob container</param>
        /// <param name="blobName">Block blob name</param>
        /// <param name="dataList">List of data to be uploaded as a block to the blob</param>
        public void UploadBlockToBlob(string containerName, string blobName, IEnumerable<string> dataList)
        {
            try
            {
                CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);
                blobContainer.CreateIfNotExists();
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
                List<string> blockIds = new List<string>();
                if (blockBlob.Exists())
                {
                    blockIds.AddRange(blockBlob.DownloadBlockList().Select(id => id.Name));
                }

                var newId = Convert.ToBase64String(Encoding.Default.GetBytes(blockIds.Count.ToString("d6")));
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        foreach (var dataItem in dataList)
                        {
                            sw.WriteLine(dataItem);
                        }
                        sw.Flush();
                        ms.Position = 0;
                        blockBlob.PutBlock(newId, ms, null);
                    }
                }
                blockIds.Add(newId);
                blockBlob.PutBlockList(blockIds);
            }
            catch (Exception e)
            {
                Log.Error("Could not upload blob " + blobName + " " + containerName + " " + e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public bool BlobExists(string containerName, string blobName)
        {
            bool blobExists = false;
            try
            {
                CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
                blobExists = blockBlob.Exists();
            }
            catch (Exception e)
            {
                Log.Error("Error in checking if  " + blobName + " exists in " + containerName + " " + e.Message);
            }

            return blobExists;
        }

        /// <summary>
        /// Downloads the blob contents to a file
        /// </summary>
        /// <param name="blobContainer">Blob container name</param>
        /// <param name="blobName">Name of the blob</param>
        /// <param name="outputFile">Output file for the blob contents</param>
        public void DownloadBlobToFile(string blobContainer, string blobName, ref string outputFile)
        {
            try
            {
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
                CloudBlockBlob cloudBlob = container.GetBlockBlobReference(blobName);
                cloudBlob.DownloadToFile(outputFile, FileMode.Create);
            }
            catch (Exception e)
            {
                Log.Error("Error downloading blob {0} from {1} : {2}", blobName, blobContainer, e.Message);
            }
        }

        /// <summary>
        /// Downloads and returns the text data from the blob
        /// </summary>
        /// <param name="blobContainer">Blob container name</param>
        /// <param name="blobName">Blob name</param>
        /// <returns>text data from blob</returns>
        public string DownloadBlobToText(string blobContainer, string blobName)
        {
            string blobData = null;
            try
            {
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
                CloudBlockBlob cloudBlob = container.GetBlockBlobReference(blobName);
                blobData = cloudBlob.DownloadText();
            }
            catch (Exception e)
            {
                Log.Error("Error downloading blob {0} from {1} : {2}", blobName, blobContainer, e.Message);
            }

            return blobData;
        }

        public async Task<string> DownloadBlobToTextAync(string blobContainer, string blobName)
        {
            string blobData = null;
            try
            {
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
                CloudBlockBlob cloudBlob = container.GetBlockBlobReference(blobName);
                blobData = await cloudBlob.DownloadTextAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error("Error downloading blob {0} from {1} : {2}", blobName, blobContainer, e.Message);
            }

            return blobData;
        }

        #endregion
    }
}