//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace AzureKeyVault
{
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Contains helper functions for deals with X509 certs.
    /// </summary>
    public static class CertificateHelper
    {
        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false); // Don't validate certs, since the test root isn't installed.

                if (col == null || col.Count == 0)
                    return null;

                return col[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}