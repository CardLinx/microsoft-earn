//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Worker
{
    using System;
    using System.IO;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;

    /// <summary>
    /// Distributed Locking mechanism using Azure Blob Lease
    /// </summary>
    public class ConcurrencyMonitor : IDisposable
    {
        /// <summary>
        /// Creates a new instance for the monitor
        /// </summary>
        public ConcurrencyMonitor(string leaseConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(leaseConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("leasecontainer");
            container.CreateIfNotExists();
            blob = container.GetBlockBlobReference("leaseblob");
            if(!Exists(blob))
            {
                using (Stream str = new MemoryStream(new byte[]{0}))
                {
                    blob.UploadFromStream(str);
                }
            }
        }

        /// <summary>
        /// Invoke the action after acquiring lease
        /// 1. Will only invoke action if lease is acquired
        /// 2. If lease is already acquired, this is as good as No-Op
        /// 3. Its is a Non-Blocking call, in the sense unlike C# locks, the thread will not wait to acquire lock
        /// </summary>
        /// <param name="action">
        /// Action to Invoke
        /// </param>
        public virtual void InvokeWithLease(Action action)
        {
            try
            {
                LeaseId = blob.AcquireLease(TimeSpan.FromSeconds(60), null);
                if (action != null)
                {
                    action.Invoke();
                }
            }
            catch (Exception)
            {
                // acquire lease can throw an exception if its already acquired
            }
        }

        /// <summary>
        /// Dispose off the object -> release lease
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementation of dispose
        /// </summary>
        /// <param name="disposing">
        /// Should dispose or not
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (LeaseId != null)
                    {
                        blob.ReleaseLease(AccessCondition.GenerateLeaseCondition(LeaseId));
                    }
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Checks if blob exists or not
        /// </summary>
        /// <param name="blob">
        /// Reference to the blob
        /// </param>
        /// <returns>
        /// True/False indicating whether blob exists
        /// </returns>
        private static bool Exists(CloudBlockBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// lease blob
        /// </summary>
        private CloudBlockBlob blob;
        
        /// <summary>
        /// Disposed flag
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Lease Id
        /// </summary>
        private string LeaseId;
    }
}