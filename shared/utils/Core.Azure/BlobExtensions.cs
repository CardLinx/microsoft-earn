//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Azure
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;

	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Blob;

	using Extensions;

	public static class BlobExtensions
	{
		public static void AppendBlock(this CloudBlockBlob blob, Stream stream, int maxRetries = 0, int sleepMilliseconds = 0)
		{
			int retryCount = 0;
			string leaseID = null;
			var random = new Random();
			var blockID = Guid.NewGuid().ToString("N").ToUpperInvariant();
			while (true)
				try
				{
					leaseID = blob.AcquireLease(TimeSpan.FromMinutes(1), null);
					var accessCondition = new AccessCondition { LeaseId = leaseID };
					blob.PutBlock(blockID, stream, null, accessCondition);
					var blockList = blob.
						DownloadBlockList(BlockListingFilter.Committed, accessCondition).
						Select(x => x.Name).
						Concat(new [] { blockID });
					blob.PutBlockList(blockList, accessCondition);
					blob.ReleaseLease(accessCondition);
					break;
				}
				catch (Exception)
				{
					if (leaseID != null)
						blob.ReleaseLease(new AccessCondition { LeaseId = leaseID });
					if (++retryCount < maxRetries)
						Thread.Sleep(random.Next(sleepMilliseconds / 2, sleepMilliseconds + sleepMilliseconds / 2));
					else
						throw;
				}
		}

		public static void AppendBlock(this CloudBlockBlob blob, byte[] block, int maxRetries = 0, int sleepMilliseconds = 0)
		{
			using (var stream = new MemoryStream(block))
			{
				blob.AppendBlock(stream, maxRetries, sleepMilliseconds);
			}
		}

		/// <param name="source">Blob to move</param>
		/// <param name="path">Full path from the root, including new blob name</param>
		/// <returns>New blob</returns>
		public static CloudBlockBlob MoveTo(this CloudBlockBlob source, string path)
		{
			var result = source.Container.GetBlockBlobReference(path);
			result.StartCopyFromBlob(source);
			source.Delete();
			return result;
		}

		/// <returns>Freshly created blob</returns>
		public static CloudBlockBlob CreateBlob(this CloudBlobContainer container, string path, string content)
		{
			var blob = container.GetBlockBlobReference(path);
			blob.UploadText(content);
			return blob;
		}

		public static long GetBlobSize(string accountName, string accessKey, string containerName, string path)
		{
			var connectionString = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}".
				ExpandWith(accountName, accessKey);
			var account = CloudStorageAccount.Parse(connectionString);
			var blobClient = account.CreateCloudBlobClient();
			var container = blobClient.GetContainerReference(containerName);
			var blob = container.GetBlockBlobReference(path);
			if (!blob.Exists())
				throw new Exception("Blob [{0}] does not exist".ExpandWith(path));
			blob.FetchAttributes();
			return blob.Properties.Length;
		}
	}
}