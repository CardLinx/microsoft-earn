//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;
    using Microsoft.Azure;

    /// <summary>
    /// An abstract base class for MasterCard FTP clients.
    /// </summary>
    public abstract class MasterCardFtpClientBase : FtpClientBase
    {
        /// <summary>
        /// Initializes static members of the MasterCardFtpClientBase class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
         Justification = "It is not possible to perform this kind of initialization inline.")]
        static MasterCardFtpClientBase()
        {
            // Popualte the Certificates collection.
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2 certificate = store.Certificates.Find(X509FindType.FindBySubjectName,
                                                                   CloudConfigurationManager.GetSetting(MasterCardFtpConstants.UserName),
                                                                   true).OfType<X509Certificate2>().FirstOrDefault();
            store.Close();
            Certificates.Add(certificate);
        }

        /// <summary>
        /// Initializes a new instance of the MasterCardFtpClientBase class.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        protected MasterCardFtpClientBase(CommerceLog log)
            : base(Credentials, Certificates, true, false, true, log)
        {
        }

        /// <summary>
        /// Gets a list of all file names in a directory.
        /// </summary>
        /// <returns>
        /// An array of file names as strings
        /// </returns>
        public override async Task<string[]> DirectoryListAsync()
        {
            string[] result = null;

            // Get the raw directory contents and then filter out incorrect file types, directories, etc..
            string[] rawContents = await base.DirectoryListAsync();
            if (rawContents != null)
            {
                List<string> filteredContents = new List<string>();
                foreach (string item in rawContents)
                {
                    if (item.ToLowerInvariant().Contains(RelevantFileNameSubstring) == true)
                    {
                        filteredContents.Add(item.Substring(item.LastIndexOf(" ") + 1));
                    }
                }

                if (filteredContents.Count > 0)
                {
                    result = filteredContents.ToArray();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or set the base of the address to which the client will connect.
        /// </summary>
        protected override string AddressAuthority
        {
            get
            {
                return CloudConfigurationManager.GetSetting(MasterCardFtpConstants.AddressAuthority);
            }
        }

        /// <summary>
        /// Gets the substring that must be present for a file to be considered relevant during a directory list operation.
        /// </summary>
        protected abstract string RelevantFileNameSubstring { get; }

        /// <summary>
        /// The credentials to use for the FTP connection.
        /// </summary>
        private static ICredentials Credentials =
            new NetworkCredential(CloudConfigurationManager.GetSetting(MasterCardFtpConstants.UserName),
                                  CloudConfigurationManager.GetSetting(MasterCardFtpConstants.Password));

        /// <summary>
        /// The certificates to use for the FTP connection.
        /// </summary>
        private static X509CertificateCollection Certificates = new X509CertificateCollection();
    }
}