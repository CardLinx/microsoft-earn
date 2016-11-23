//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The messages blob client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Email
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The messages blob client.
    /// </summary>
    public class EmailBlobClient
    {
        #region Data Members

        /// <summary>
        /// The html content type.
        /// </summary>
        private const string HtmlContentType = "text/html";

        /// <summary>
        /// The text content type.
        /// </summary>
        private const string TextContentType = "text/plain";

        /// <summary>
        /// The pickup base directory name.
        /// </summary>
        private const string PickupBaseDirectoryName = "email_blob_pickup";

        /// <summary>
        /// The pickup directory full base path.
        /// </summary>
        private readonly string pickupDirectoryFullBasePath;

        /// <summary>
        /// The blob container.
        /// </summary>
        private readonly CloudBlobContainer blobContainer;

        /// <summary>
        /// The smtp client.
        /// </summary>
        private readonly SmtpClient smtpClient = new SmtpClient();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailBlobClient"/> class.
        /// </summary>
        /// <param name="accountName">
        /// The account name.
        /// </param>
        /// <param name="containerName">
        /// The container name.
        /// </param>
        public EmailBlobClient(string accountName, string containerName)
        {
            // This code assumes that if we are running in the cloud a custom temp folder is assigned to the temp environment variable
            this.pickupDirectoryFullBasePath = Path.Combine(Path.GetTempPath(), PickupBaseDirectoryName);
            var cloudStorageAccount = CloudStorageAccount.Parse(accountName);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            this.blobContainer = blobClient.GetContainerReference(containerName);
            if (this.blobContainer.CreateIfNotExists())
            {
                this.blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            this.smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
        }

        #endregion

        /// <summary>
        /// Send outbound message
        /// </summary>
        /// <param name="correlationId">
        /// The correlation Id.
        /// </param>
        /// <param name="emailInformation">
        /// The email Information.
        /// </param>
        public void Send(Guid correlationId, EmailInformation emailInformation)
        {
            MailMessage mailMessage =
                new MailMessage { From = new MailAddress(emailInformation.From, emailInformation.FromDisplayName), Subject = emailInformation.Subject };
            
            string emailTempDir = Path.Combine(this.pickupDirectoryFullBasePath, Guid.NewGuid().ToString());
            this.smtpClient.PickupDirectoryLocation = emailTempDir;
            try
            {
                Directory.CreateDirectory(emailTempDir);
                foreach (string toAdress in emailInformation.To)
                {
                    mailMessage.To.Add(toAdress);
                }

                if (!string.IsNullOrEmpty(emailInformation.HtmlBody) && !string.IsNullOrEmpty(emailInformation.TextBody))
                {
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailInformation.HtmlBody, new ContentType(HtmlContentType)));
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailInformation.TextBody, new ContentType(TextContentType)));
                }
                else if (!string.IsNullOrEmpty(emailInformation.HtmlBody))
                {
                    mailMessage.Body = emailInformation.HtmlBody;
                    mailMessage.IsBodyHtml = true;
                }
                else if (!string.IsNullOrEmpty(emailInformation.TextBody))
                {
                    mailMessage.Body = emailInformation.TextBody;
                    mailMessage.IsBodyHtml = false;
                }

                this.smtpClient.Send(mailMessage);
                string emailFilePath = Directory.GetFiles(emailTempDir).First();
                byte[] emailFileContent = File.ReadAllBytes(emailFilePath);
                var viewBlob = this.blobContainer.GetBlockBlobReference(string.Format("{0}-view.eml", correlationId));
                viewBlob.Properties.ContentType = "message/rfc822";
                var downloadBlob = this.blobContainer.GetBlockBlobReference(string.Format("{0}-download.eml", correlationId));
                using (MemoryStream ms = new MemoryStream(emailFileContent))
                {
                    ms.Position = 0;
                    viewBlob.UploadFromStream(ms);
                    ms.Position = 0;
                    downloadBlob.UploadFromStream(ms);
                }
            }
            finally
            {
                Directory.Delete(emailTempDir, true);
            }
        }
    }
}