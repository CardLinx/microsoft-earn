//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.IO;
using System.Threading.Tasks;

namespace Azure.Utils.Interface
{
    public interface IAzureBlob
    {
        Task UploadBlobFromStreamAsync(string containerName, string blobName, Stream stream);

        Task<MemoryStream> DownloadBlobToStreamAsync(string blobContainer, string blobName);
    }
}