//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.MasterCardWorker;
    using Lomo.Commerce.VisaWorker;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;
    using Microsoft.WindowsAzure;

    /// <summary>
    /// Contains logic to process a MasterCard filtering file job.
    /// </summary>
    public class ProcessMasterCardFilteringJob : IScheduledJob
    {
        /// <summary>
        /// Process MasterCard filtering file job execution.
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
        public async Task Execute(ScheduledJobDetails details,
                                  CommerceLog log)
        {
            Log = log;

            Log.Verbose("Starting execution of job.\r\nDetails {0}", details);

            // Process filtering job for MasterCard.
            ISettlementFileProcessor masterCardProcessor = MasterCardFileProcessorFactory.MasterCardFilteringProcessor(UploadFilteringFile);
            await masterCardProcessor.Process().ConfigureAwait(false);

            Log.Verbose("Exeuction of job {0} complete ", details.JobId);
        }

        /// <summary>
        /// Uploads file to blob store and FTP site as needed.
        /// </summary>
        /// <param name="content">
        /// The content of the file to upload.
        /// </param>
        /// <returns>
        /// The task to upload the file where needed.
        /// </returns>
        public async Task UploadFilteringFile(string content)
        {
            string fileName = String.Concat("Filtering", DateTime.Now.ToString("yyyyMMddHmmss"), ".txt");
            MasterCardFilteringBlobClient blobClient = MasterCardBlobClientFactory.MasterCardFilteringBlobClient(Log);

            // Upload file to the blob store and MasterCard.
            byte[] contentBytes = Encoding.ASCII.GetBytes(content);
            using (MemoryStream memoryStream = new MemoryStream(contentBytes))
            {
                // Upload the file to the blob store.
                memoryStream.Position = 0;
                await blobClient.UploadAsync(memoryStream, fileName).ConfigureAwait(false);
                Log.Verbose("Uploaded file {0} to blob store.", fileName);

                // Upload the file to MasterCard.
                IFtpClient ftpClient = MasterCardFtpClientFactory.FilteringFtpClient(Log);
                memoryStream.Position = 0;
                await ftpClient.UploadFileAsync(fileName, memoryStream);
                Log.Verbose("Uploaded file {0} to MasterCard.", fileName);
            }

            // Mark the file copy in the blob store as having been uploaded to MasterCard.
            await blobClient.MarkAsCompleteAsync(fileName).ConfigureAwait(false);
            Log.Verbose("File {0} marked as uploaded.", fileName);
        }

        /// <summary>
        /// Gets or sets the log within which to log status of job processing.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}