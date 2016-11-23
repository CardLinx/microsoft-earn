//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The template store client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.Templating
{
    using System.Configuration;
    using System.IO;
    using System.Text;
    using Lomo.Logging;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The template store client.
    /// </summary>
    public class TemplateBlobStoreClient : ITemplateStoreClient
    {
        #region Consts

        /// <summary>
        /// The templates container name.
        /// </summary>
        internal const string TemplatesContainerName = "templates";

        /// <summary>
        /// The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.Templating.ConnectionString";

        #endregion

        #region Data Members

        /// <summary>
        /// The blob client.
        /// </summary>
        private readonly CloudBlobClient blobClient;

        /// <summary>
        /// Indicate whether the templates container already exists.
        /// </summary>
        private bool templatesContainerExists = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBlobStoreClient"/> class.
        /// </summary>
        public TemplateBlobStoreClient()
            : this(GetStorageAccountFromConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBlobStoreClient"/> class.
        /// </summary>
        /// <param name="account">
        /// Azure Storage account name.
        /// </param>
        public TemplateBlobStoreClient(string account)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(account);
            this.blobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        #endregion

        #region ITemplateStoreClient Imp

        /// <summary>
        /// <see cref="ITemplateStoreClient.DownloadTemplate"/>
        /// </summary>
        public string DownloadTemplate(string category, string name, Locale locale)
        {
            if (locale != null)
            {
                name = string.Format("{0}-{1}", locale.Id, name);
            }

            name = name.ToLowerInvariant();

            Log.Verbose("Start Downloading Template. Category:{0}; Name:{1}; Locale: {2}", category, name, locale);
            string blobReferencePath = string.Format("{0}/{1}", category, name);
            CloudBlockBlob templateBlob = this.blobClient.GetContainerReference(TemplatesContainerName).GetBlockBlobReference(blobReferencePath);
            try
            {
                string text;
                using (var memoryStream = new MemoryStream())
                {
                    templateBlob.DownloadToStream(memoryStream);
                    memoryStream.Position = 0;
                    using (var sr = new StreamReader(memoryStream))
                    {
                        text = sr.ReadToEnd();
                    }
                }

                Log.Verbose("Complete Downloading Template. Category:{0}; Name:{1}; Locale: {2}", category, name, locale);
                return text;
            }
            catch (StorageException exception)
            {
                Log.Verbose("Error while downloading Template. Category:{0}; Name:{1}; Locale: {2}; Exception: {3}", category, name, locale, exception);
                throw new TemplateStoreClientException(string.Format("Error while Downloading template. Category: {0}; Name: {1}; Locale: {2}", category, name, locale), exception);
            }
        }

        /// <summary>
        /// <see cref="ITemplateStoreClient.UploadTemplate"/>
        /// </summary>
        public void UploadTemplate(string category, string name, Locale locale, string template)
        {
            category = category.ToLowerInvariant();
            if (locale != null)
            {
                name = string.Format("{0}-{1}", locale.Id, name);
            }

            name = name.ToLowerInvariant();
            try
            {
                Log.Verbose("Start Uploading Template. Category:{0}; Name:{1}; Locale: {2}", category, name, locale);
                CloudBlobContainer blobContainer = this.blobClient.GetContainerReference(TemplatesContainerName);
                if (!this.templatesContainerExists)
                {
                    blobContainer.CreateIfNotExists();
                    blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                    this.templatesContainerExists = true;
                }

                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(string.Format("{0}/{1}", category, name));
                blob.Properties.ContentType = "text/xml";
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms))
                    {
                        sw.Write(template);
                        sw.Flush();
                        ms.Position = 0;
                        blob.UploadFromStream(ms);
                    }
                }
                
                Log.Verbose("Complete Uploading Template. Category:{0}; Name:{1}; Locale: {2}", category, name, locale);
            }
            catch (StorageException exception)
            {
                Log.Verbose("Error while uploading Template. Category:{0}; Name:{1}; Locale: {2}; Exception: {3}", category, name, locale, exception);
                throw new TemplateStoreClientException(string.Format("Error while Uploading template. Category: {0}; Name: {1}; Locale: {2}", category, name, locale), exception);
            }
        }

        /// <summary>
        /// <see cref="ITemplateStoreClient.DeleteTemplate"/>
        /// </summary>
        public void DeleteTemplate(string category, string name, Locale locale)
        {
            category = category.ToLowerInvariant();
            if (locale != null)
            {
                name = string.Format("{0}-{1}", locale.Id, name);
            }
            
            name = name.ToLowerInvariant();

            Log.Verbose("Start Deleting Template. Category:{0}; Name:{1}; Locale: {2}", category, name, locale);
            string blobReferencePath = string.Format("{0}/{1}", category, name);
            CloudBlockBlob templateBlob = this.blobClient.GetContainerReference(TemplatesContainerName).GetBlockBlobReference(blobReferencePath);
            
            try
            {
                templateBlob.Delete();
                Log.Verbose("Complete Deleting Template. Category:{0}; Name:{1}; Locale: {2}", category, name, locale);
            }
            catch (StorageException exception)
            {
                Log.Verbose("Error while deleting Template. Category:{0}; Name:{1}; Locale: {2}; Exception: {3}", category, name, locale, exception);
                throw new TemplateStoreClientException(string.Format("Error while deleting template. Category: {0}; Name: {1}; Locale: {2}", category, name, locale), exception);
            }
        }

        /// <summary>
        /// <see cref="ITemplateStoreClient.DeleteCategoryTemplates"/>
        /// </summary>
        public void DeleteCategoryTemplates(string category)
        {
            category = category.ToLowerInvariant();
            Log.Verbose("Start Deleting Category Templates. Category:{0}; ", category);
            CloudBlobContainer cloudBlobContainer = this.blobClient.GetContainerReference(TemplatesContainerName);
            CloudBlobDirectory cloudBlobDirectory = cloudBlobContainer.GetDirectoryReference(category);
            try
            {
                this.DeleteDirectory(cloudBlobDirectory);
                Log.Verbose("Complete Deleting Category Templates. Category:{0}; ", category);
            }
            catch (StorageException exception)
            {
                Log.Verbose("Error while deleting Category Templates. Category:{0}; Exception: {1}", category, exception);
                throw new TemplateStoreClientException(string.Format("Error while deleting category templates. Category: {0}; ", category), exception);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The get storage account from config.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetStorageAccountFromConfig()
        {
            return CloudConfigurationManager.GetSetting(StorageSetting);
        }

        /// <summary>
        /// The virtual azure blob directory.
        /// </summary>
        /// <param name="directory">
        /// The blob directory.
        /// </param>
        private void DeleteDirectory(CloudBlobDirectory directory)
        {
            foreach (var subItem in directory.ListBlobs())
            {
                if (subItem is CloudBlobDirectory)
                {
                    this.DeleteDirectory(subItem as CloudBlobDirectory);
                }
                else
                {
                    ((CloudBlockBlob)subItem).Delete();
                }
            }
        }

        #endregion
    }
}