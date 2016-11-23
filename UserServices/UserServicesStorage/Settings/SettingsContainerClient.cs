//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The settings blob client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.Storage.Settings
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Lomo.Logging;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage;
    /// <summary>
    /// The settings blob client.
    /// </summary>
    public class SettingsContainerClient
    {
        #region Constants

        /// <summary>
        /// The deals selection rules blob name.
        /// </summary>
        private const string DealsSelectionRulesBlobName = "deals-selection-rules.xml";

        #endregion

        #region Fields

        /// <summary>
        /// The blob container.
        /// </summary>
        private readonly CloudBlobContainer blobContainer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsContainerClient"/> class.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <param name="settingsContainerName">
        /// The setting container name.
        /// </param>
        public SettingsContainerClient(string account, string settingsContainerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(account);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            this.blobContainer = blobClient.GetContainerReference(settingsContainerName);
            if (this.blobContainer.CreateIfNotExists())
            {
                this.blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get deals selection rules.
        /// </summary>
        /// <returns>
        /// The <see cref="DealsSelectionRules"/>.
        /// </returns>
        /// <exception cref="StorageException">Error downloading the rules file from the blob</exception>
        public DealsSelectionRules GetDealsSelectionRules()
        {
            Log.Verbose("Starting Download Deal Selection Rules from settings container");
            CloudBlockBlob dealsSelectionRulesBlob = this.blobContainer.GetBlockBlobReference(DealsSelectionRulesBlobName);
            try
            {
                string dealsSelectionRulesXml;
                using (var memoryStream = new MemoryStream())
                {
                    dealsSelectionRulesBlob.DownloadToStream(memoryStream);
                    memoryStream.Position = 0;
                    using (var sr = new StreamReader(memoryStream))
                    {
                        dealsSelectionRulesXml = sr.ReadToEnd();
                    }
                }

                var xmlSerializer = new XmlSerializer(typeof(DealsSelectionRules));
                var rules = (DealsSelectionRules)xmlSerializer.Deserialize(new StringReader(dealsSelectionRulesXml));
                Log.Verbose("Complete Download Deal Selection Rules from settings container");
                return rules;
            }
            catch (Exception e)
            {
                Log.Verbose("Error while downloading deals selection rules settings. Container Path: {0}; Blob Name: {1}; Error: {2}", this.blobContainer.Uri, DealsSelectionRulesBlobName, e);
                throw;
            }
        }

        #endregion
    }
}