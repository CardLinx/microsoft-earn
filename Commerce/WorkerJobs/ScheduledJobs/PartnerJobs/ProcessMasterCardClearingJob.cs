//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardWorker;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;

    /// <summary>
    /// Contains logic to process a MasterCard clearing file job.
    /// </summary>
    public class ProcessMasterCardClearingJob : IScheduledJob
    {
        /// <summary>
        /// Process MasterCard clearing file job execution.
        /// </summary>
        /// <param name="details">
        /// Details of the job to be executed.
        /// </param>
        /// <param name="log">
        /// Log within which to log status of job processing.
        /// </param>
        /// <returns>
        /// A task to execute the  job.
        /// </returns>
        /// <remarks>
        /// Once complete, this job will schedule a corresponding MasterCardProcessClearingJob.
        /// </remarks>
        public async Task Execute(ScheduledJobDetails details,
                                  CommerceLog log)
        {
            Log = log;
            Log.Verbose("Starting execution of job.\r\nDetails {0}", details);

            MasterCardClearingBlobClient blobClient = MasterCardBlobClientFactory.MasterCardClearingBlobClient(log);

            // Download files from MasterCard and upload them to the blob store.
            IFtpClient ftpClient = MasterCardFtpClientFactory.ClearingFtpClient(Log);
            string[] files = await ftpClient.DirectoryListAsync();
            if (files != null)
            {
                foreach (string fileName in files)
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        // Download the file from MasterCard.
                        await ftpClient.DownloadFileAsync(fileName, memStream).ConfigureAwait(false);

                        // Upload the file to the blob store.
                        memStream.Position = 0;
                        await blobClient.UploadAsync(memStream, fileName).ConfigureAwait(false);
                    }
                }
            }

            // Process all pending clearing files in the blob store.
            ICollection<string> fileNames = blobClient.RetrieveNamesOfPendingFiles();
            if (fileNames != null)
            {
                foreach (string fileName in fileNames)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        // Download the file from the blob store.
                        try
                        {
                            memoryStream.Position = 0;
                            await blobClient.DownloadAsync(memoryStream, fileName).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Unexpected Exception when downloading Clearing File from Blob store.", ex);
                            throw;
                        }

                        // Process the file.
                        try
                        {
                            memoryStream.Position = 0;
                            ISettlementFileProcessor clearingProcessor = MasterCardFileProcessorFactory.MasterCardClearingProcessor(memoryStream, fileName);
                            await clearingProcessor.Process().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Unexpected Exception when processing Clearing File.", ex);
                            throw;
                        }
                    }

                    // Mark the file as having been processed.
                    await blobClient.MarkAsCompleteAsync(fileName).ConfigureAwait(false);
                }
            }

            Log.Verbose("Execution of job {0} complete ", details.JobId);
        }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}