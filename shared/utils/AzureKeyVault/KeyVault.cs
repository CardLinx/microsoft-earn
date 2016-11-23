//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace AzureKeyVault
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using System.Threading;
    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// This is a wrapper class for the Azure KeyVaultClient class.
    /// </summary>
    public class KeyVault
    {
        public string VaultName { get; }

        #region Private vars

        const string VaultUriTemplate = "https://{0}.vault.azure.net";

        readonly ClientAssertionCertificate certificate;

        readonly string activeDirectoryApplicationClientId;

        readonly string activeDirectoryApplicationKey;

        readonly KeyVaultClient keyVaultClient;

        readonly string vaultUri;
#endregion

#region Factory methods

        public static KeyVault CreateForKeyBasedAuthentication(
            string vaultName,
            string activeDirectoryApplicationClientId,
            string activeDirectoryApplicationKey)
        {
            return new KeyVault(vaultName, activeDirectoryApplicationClientId, activeDirectoryApplicationKey, null);
        }

        public static KeyVault CreateForCertificateBasedAuthentication(
            string vaultName,
            string activeDirectoryApplicationClientId,
            string certificateThumbprint)
        {
            var clientAssertionCertPfx = CertificateHelper.FindCertificateByThumbprint(certificateThumbprint);
            ClientAssertionCertificate certificate = new ClientAssertionCertificate(activeDirectoryApplicationClientId, clientAssertionCertPfx);
            return new KeyVault(vaultName, activeDirectoryApplicationClientId, null, certificate);
        }

        #endregion

#region Public Helper methods

        public static string GetVaultUriFromName(string vaultName)
        {
            return string.Format(VaultUriTemplate, vaultName);
        }
        #endregion

#region Constructor
        private KeyVault(
            string vaultName,
            string activeDirectoryApplicationClientId,
            string activeDirectoryApplicationKey,
            ClientAssertionCertificate certificate)
        {
            this.VaultName = vaultName;
            this.activeDirectoryApplicationClientId = activeDirectoryApplicationClientId;
            this.activeDirectoryApplicationKey = activeDirectoryApplicationKey;
            this.certificate = certificate;
            if (certificate == null)
            {
                this.keyVaultClient = new KeyVaultClient(this.GetAccessTokenUsingApplicationKey);
            }
            else
            {
                this.keyVaultClient = new KeyVaultClient(this.GetAccessTokenUsingCertificate);
            }

            this.vaultUri = GetVaultUriFromName(vaultName);
        }
#endregion

#region Wrapper Methods
        public Task<KeyOperationResult> EncryptAsync(string keyName, string keyVersion, string algorithm, byte[] plainText, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.EncryptAsync(
                this.vaultUri,
                keyName,
                keyVersion,
                algorithm,
                plainText,
                cancellationToken);
        }

        public Task<KeyOperationResult> EncryptAsync(string keyName, string algorithm, byte[] plainText, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.EncryptAsync(this.GetKeyIdentifier(keyName), algorithm, plainText, cancellationToken);
        }

        public Task<KeyOperationResult> DecryptAsync(string keyName, string algorithm, byte[] cipherText, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.DecryptAsync(this.GetKeyIdentifier(keyName), algorithm, cipherText, cancellationToken);
        }

        public Task<KeyOperationResult> SignAsync(string keyName, string keyVersion, string algorithm, byte[] digest, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.SignAsync(this.vaultUri, keyName, keyVersion, algorithm, digest, cancellationToken);
        }

        public Task<KeyOperationResult> SignAsync(string keyName, string algorithm, byte[] digest, CancellationToken cancellationToken = default(CancellationToken))
        {

            return this.keyVaultClient.SignAsync(this.GetKeyIdentifier(keyName), algorithm, digest, cancellationToken);
        }

        public Task<bool> VerifyAsync(string keyName, string algorithm, byte[] digest, byte[] signature, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.VerifyAsync(this.GetKeyIdentifier(keyName), algorithm, digest, signature, cancellationToken);
        }

        public Task<KeyOperationResult> WrapKeyAsync(string keyName, string keyVersion, string algorithm, byte[] key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.WrapKeyAsync(this.vaultUri, keyName, keyVersion, algorithm, key, cancellationToken);
        }

        public Task<KeyOperationResult> WrapKeyAsync(string keyName, string algorithm, byte[] key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.WrapKeyAsync(this.GetKeyIdentifier(keyName), algorithm, key, cancellationToken);
        }

        public Task<KeyOperationResult> UnwrapKeyAsync(string keyName, string algorithm, byte[] wrappedKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.UnwrapKeyAsync(this.GetKeyIdentifier(keyName), algorithm, wrappedKey, cancellationToken);
        }

        public Task<KeyBundle> CreateKeyAsync(string keyName, string keyType, int? keySize = null, string[] keyOps = null, KeyAttributes keyAttributes = null, Dictionary<string, string> tags = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.CreateKeyAsync(
                this.vaultUri,
                keyName,
                keyType,
                keySize,
                keyOps,
                keyAttributes,
                tags,
                cancellationToken);
        }

        public Task<KeyBundle> GetKeyAsync(string keyName, string keyVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetKeyAsync(this.vaultUri, keyName, keyVersion, cancellationToken);
        }

        public Task<KeyBundle> GetKeyAsync(string keyName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetKeyAsync(this.GetKeyIdentifier(keyName), cancellationToken);
        }

        public Task<ListKeysResponseMessage> GetKeysAsync(int? maxresults = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetKeysAsync(this.vaultUri, maxresults, cancellationToken);
        }

        public Task<ListKeysResponseMessage> GetKeysNextAsync(string nextLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetKeysNextAsync(nextLink, cancellationToken);
        }

        public Task<ListKeysResponseMessage> GetKeyVersionsAsync(string keyName, int? maxresults = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetKeyVersionsAsync(this.vaultUri, keyName, maxresults, cancellationToken);
        }

        public Task<ListKeysResponseMessage> GetKeyVersionsNextAsync(string nextLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetKeyVersionsNextAsync(nextLink, cancellationToken);
        }

        public Task<KeyBundle> DeleteKeyAsync(string keyName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.DeleteKeyAsync(this.vaultUri, keyName, cancellationToken);
        }

        public Task<KeyBundle> UpdateKeyAsync(string keyName, string[] keyOps = null, KeyAttributes attributes = null, Dictionary<string, string> tags = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.UpdateKeyAsync(
                this.vaultUri,
                keyName,
                keyOps,
                attributes,
                tags,
                cancellationToken);
        }

        public Task<KeyBundle> ImportKeyAsync(string keyName, KeyBundle keyBundle, bool? importToHardware = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.ImportKeyAsync(
                this.vaultUri,
                keyName,
                keyBundle,
                importToHardware,
                cancellationToken);
        }

        public Task<byte[]> BackupKeyAsync(string keyName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.BackupKeyAsync(this.vaultUri, keyName, cancellationToken);
        }

        public Task<KeyBundle> RestoreKeyAsync(byte[] keyBundleBackup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.RestoreKeyAsync(this.vaultUri, keyBundleBackup, cancellationToken);
        }

        public Task<Secret> GetSecretAsync(string secretName, string secretVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetSecretAsync(this.vaultUri, secretName, secretVersion, cancellationToken);
        }

        public Task<Secret> GetSecretAsync(string secretName, CancellationToken cancellationToken = default(CancellationToken))
        {

            return this.keyVaultClient.GetSecretAsync(this.GetSecretIdentifier(secretName), cancellationToken);
        }

        public Task<Secret> SetSecretAsync(string secretName, string value, Dictionary<string, string> tags = null, string contentType = null, SecretAttributes secretAttributes = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.SetSecretAsync(
                this.vaultUri,
                secretName,
                value,
                tags,
                contentType,
                secretAttributes,
                cancellationToken);
        }

        public Task<Secret> UpdateSecretAsync(string secretName, string contentType = null, SecretAttributes secretAttributes = null, Dictionary<string, string> tags = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.UpdateSecretAsync(
                this.vaultUri,
                secretName,
                contentType,
                secretAttributes,
                tags,
                cancellationToken);
        }

        public Task<Secret> DeleteSecretAsync(string secretName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.DeleteSecretAsync(this.vaultUri, secretName, cancellationToken);
        }

        public Task<ListSecretsResponseMessage> GetSecretsAsync(string vault, int? maxresults = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetSecretsAsync(this.vaultUri, maxresults, cancellationToken);
        }

        public Task<ListSecretsResponseMessage> GetSecretsNextAsync(string nextLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetSecretsNextAsync(nextLink, cancellationToken);
        }

        public Task<ListSecretsResponseMessage> GetSecretVersionsAsync(string vault, string secretName, int? maxresults = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetSecretVersionsAsync(this.vaultUri, secretName, maxresults, cancellationToken);
        }

        public Task<ListSecretsResponseMessage> GetSecretVersionsNextAsync(string nextLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.keyVaultClient.GetSecretVersionsNextAsync(nextLink, cancellationToken);
        }

#endregion

#region Private Helper methods
        string GetKeyIdentifier(string keyName, string version = null)
        {
            var keyId = new KeyIdentifier(this.vaultUri, keyName, version);
            return keyId.Identifier;
        }

        string GetSecretIdentifier(string secretName, string version = null)
        {
            var sid = new SecretIdentifier(this.vaultUri, secretName, version);
            return sid.Identifier;
        }

        async Task<string> GetAccessTokenUsingCertificate(string authority, string resource, string scope)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, this.certificate);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the access token");

            return result.AccessToken;
        }

        async Task<string> GetAccessTokenUsingApplicationKey(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(this.activeDirectoryApplicationClientId, this.activeDirectoryApplicationKey);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the access token");

            return result.AccessToken;
        }

#endregion
    }
}