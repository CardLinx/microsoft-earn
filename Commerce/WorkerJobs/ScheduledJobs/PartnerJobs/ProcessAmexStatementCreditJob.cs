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
    using Lomo.Commerce.AmexWorker;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// Amex Statement Credit File Processing Job
    /// </summary>
    public class ProcessAmexStatementCreditJob : IScheduledJob
    {
        /// <summary>
        /// Process Amex Statement Credi File Job Execution
        /// </summary>
        /// <param name="details">
        /// Details of the job we are executing here. 
        /// </param>
        /// <param name="logger">
        /// Handle to the logger
        /// </param>
        public async Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            Logger = logger;

            Logger.Verbose("Starting execution of job \r\n Details {0}", details);

            // Process Statement Credit Acknowledgment files
            await ProcessStatementCreditResponse();

            // Process Statement Credit files
            StatementCreditFileBuilder builder = new StatementCreditFileBuilder();
            await builder.Build(OnStmtCreditFileBuild).ConfigureAwait(false);

            Logger.Verbose("Execution of job {0} complete ", details.JobId);
        }

        /// <summary>
        /// Async Delegate to do processing after we construct the content of file.
        /// 1. Upload the content to blob store, upload pending
        /// 2. Ftp to Amex
        /// 3. Mark as uploaded
        /// </summary>
        /// <param name="content">
        /// Content of the file
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task OnStmtCreditFileBuild(string content)
        {
            string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");

            AmexStatementCreditSftpClient ftpClient = new AmexStatementCreditSftpClient(Logger);
            AmexStatementCreditFileBlobClient blobClient = AmexBlobFactory.StatementCreditFileBlobClient(connectionString, Logger);

            string fileName = "MSF_AXP_stmt_crdt_" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".txt";

            if (content != null)
            {
                //upload file to blob store
                byte[] contentBytes = Encoding.ASCII.GetBytes(content);
                MemoryStream ms = new MemoryStream(contentBytes);
                ms.Position = 0;
                await blobClient.UploadAsync(ms, fileName).ConfigureAwait(false);

                // ftp it
                ms.Position = 0;
                await ftpClient.UploadFileAsync(fileName, ms, "inbox");
                Logger.Verbose("File {0} uploaded to Amex", fileName);

                // mark done
                await blobClient.MarkAsProcessedAsync(fileName);
                Logger.Verbose("File {0} marked as uploaded", fileName);
            }
        }

        private async Task ProcessStatementCreditResponse()
        {
            Logger.Verbose("Processing Amex Statement Credit Response files");
            string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");
            AmexStatementCreditResponseSftpClient ftpClient = new AmexStatementCreditResponseSftpClient(Logger);
            AmexStatementCreditResponseFileBlobClient blobClient = AmexBlobFactory.StatementCreditResponseFileBlobClient(connectionString, Logger);

            Logger.Verbose("Reading all statement credit acknowledgment files");
            string[] responseFileNames = ftpClient.DirectoryListAsync("AXP_MSF_STMT_CRDT_ACK", "outbox").Result;

            if (responseFileNames == null || responseFileNames.Length == 0)
            {
                Logger.Verbose("No statement credit acknowledgment files to process.");
            }
            else
            {
                Logger.Verbose("Found {0} statement credit acknowledgment files\n {1} \n", responseFileNames.Length, string.Join("\n", responseFileNames));
                foreach (string fileName in responseFileNames)
                {
                    MemoryStream stream = new MemoryStream();
                    await ftpClient.DownloadFileAsync(fileName, stream, "outbox").ConfigureAwait(false);

                    stream.Position = 0;
                    await blobClient.UploadAsync(stream, fileName).ConfigureAwait(false);
                }

                // Need to check for failures in the acknowledgment files and update the transactions accordingly.
                // This code is not implemented as its not the priority at the moment and due to Earn Sunset.
            }

            Logger.Verbose("Finished processing Amex Statement Credit Response files");
        }

        private CommerceLog Logger { get; set; }
    }
}