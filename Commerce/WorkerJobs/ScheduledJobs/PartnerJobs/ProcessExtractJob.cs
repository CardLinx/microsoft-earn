//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{

    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.FirstDataWorker;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// Job to Process FDC Extract files.
    /// </summary>
    public class ProcessExtractJob : IScheduledJob
    {
        /// <summary>
        /// 1. Process the FDC Extract file.
        /// 2. Schedule the Process Pts Job
        /// </summary>
        /// <param name="details">
        /// Details of the job we are executing here. 
        /// </param>
        /// <param name="logger">
        /// Handle to the logger
        /// </param>
        public async Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            logger.Verbose("Starting execution of job \r\n " +
                           "Details {0}", details);

            string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");
            IFtpClient ftpClient = FirstDataFtpClientFactory.FirstDataExtractFtpClient(logger);

            FirstDataExtractBlobClient blobClient = FirstDataBlobClientFactory.FirstDataExtractBlobClient(connectionString, logger);

            string[] files = await ftpClient.DirectoryListAsync();
            if (files != null)
            {
                foreach (string fileName in files)
                {
                    MemoryStream memStream = new MemoryStream();
                    await ftpClient.DownloadFileAsync(fileName, memStream).ConfigureAwait(false);

                    // lets upload it to blob
                    memStream.Position = 0;
                    await blobClient.UploadAsync(memStream, fileName).ConfigureAwait(false);
                }
            }

            // Now try to run all the pending files in the blob
            ICollection<string> listOfFiles = blobClient.RetrieveFilesToProcess();
            if (listOfFiles != null)
            {
                foreach (string fileName in listOfFiles)
                {
                    MemoryStream memStream = new MemoryStream();
                    memStream.Position = 0;
                    await blobClient.DownloadAsync(memStream, fileName).ConfigureAwait(false);

                    memStream.Position = 0;
                    ISettlementFileProcessor extractProcessor = FirstDataFileProcessorFactory.FirstDataExtractProcessor(fileName, memStream);
                    await extractProcessor.Process().ConfigureAwait(false);

                    await blobClient.MarkAsProcessedAsync(fileName).ConfigureAwait(false);
                }
            }

            logger.Verbose("Execution of job {0} complete ", details.JobId);

        }
    }
}