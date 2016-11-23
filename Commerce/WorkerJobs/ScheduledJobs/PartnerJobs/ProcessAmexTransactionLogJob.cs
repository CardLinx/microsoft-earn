//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.AmexWorker;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// Amex Transaction Log File Processing Job
    /// </summary>
    public class ProcessAmexTransactionLogJob : IScheduledJob
    {
        /// <summary>
        /// Process Amex Transaction Log File Job Execution
        /// </summary>
        /// <param name="details">
        /// Details of the job we are executing here. 
        /// </param>
        /// <param name="logger">
        /// Handle to the logger
        /// </param>
        public async Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            logger.Verbose("Starting execution of job \r\n Details {0}", details);
            string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");
            AmexTransactionLogSftpClient ftpClient = new AmexTransactionLogSftpClient(logger);
            AmexTransactionLogFileBlobClient blobClient = AmexBlobFactory.TransactionLogBlobClient(connectionString, logger);

            string[] files = await ftpClient.DirectoryListAsync("AXP_MSF_TLOG", "outbox");
            if (files != null)
            {
                foreach (string fileName in files)
                {
                    MemoryStream memStream = new MemoryStream();
                    await ftpClient.DownloadFileAsync(fileName, memStream, "outbox").ConfigureAwait(false);

                    // lets upload it to blob
                    memStream.Position = 0;
                    await blobClient.UploadAsync(memStream, fileName).ConfigureAwait(false);
                }
            }

            ICollection<string> listOfFiles = blobClient.RetrieveFilesToProcess();
            if (listOfFiles != null)
            {
                foreach (string fileName in listOfFiles)
                {
                    MemoryStream memStream = new MemoryStream();
                    memStream.Position = 0;
                    await blobClient.DownloadAsync(memStream, fileName).ConfigureAwait(false);

                    memStream.Position = 0;
                    TransactionLogFileProcessor transactionLogFileProcessor = new TransactionLogFileProcessor()
                    {
                        TransactionLogFileName = fileName,
                        TransactionLogFileStream = memStream
                    };
                    await transactionLogFileProcessor.Process().ConfigureAwait(false);

                    await blobClient.MarkAsProcessedAsync(fileName).ConfigureAwait(false);
                }
            }

            logger.Verbose("Execution of job {0} complete ", details.JobId);
        }
    }
}