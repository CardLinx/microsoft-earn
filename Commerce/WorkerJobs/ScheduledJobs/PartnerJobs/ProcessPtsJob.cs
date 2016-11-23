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
    using Lomo.Commerce.FirstDataWorker;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.VisaWorker;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// Pts File Processing Job
    /// </summary>
    public class ProcessPtsJob : IScheduledJob
    {
        /// <summary>
        /// Process Pts File Job Execution
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

            Logger.Verbose("Starting execution of job \r\n " +
                          "Details {0}", details);

            // Process PTS Job for FDC
            ISettlementFileProcessor ptsProcessor = FirstDataFileProcessorFactory.FirstDataPtsProcessor(OnPtsBuild);
            await ptsProcessor.Process().ConfigureAwait(false);
            
            Logger.Verbose("Exeuction of job {0} complete ", details.JobId);
        }

        /// <summary>
        /// Async Delegate to do processing after we construct the content of file.
        /// 1. Upload the content to blob store, upload pending
        /// 2. Ftp to FDC
        /// 3. Mark as uploaded
        /// </summary>
        /// <param name="content">
        /// Content of the file
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task OnPtsBuild(string content)
        {
            IFtpClient ftpClient = FirstDataFtpClientFactory.FirstDataPtsFtpClient(Logger);

            string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");
            FirstDataPtsBlobClient blobClient = FirstDataBlobClientFactory.FirstDataPtsBlobClient(connectionString, Logger);

            // FDC requires EST.
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime estNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, easternZone);
            string fileName = estNow.ToString("yyyyMMddHmmss") + ".GPTD5628.TXT";

            //upload file to blob store
            byte[] contentBytes = Encoding.ASCII.GetBytes(content);
            MemoryStream ms = new MemoryStream(contentBytes);
            ms.Position = 0;
            await blobClient.UploadAsync(ms, fileName);
            Logger.Verbose("Uploaded file {0} to blob stored to be Ftped to FDC", fileName);

            // ftp it only if there are transactions
            if (!IsFileEmpty(ms))
            {
                ms.Position = 0;
                await ftpClient.UploadFileAsync(fileName, ms);
                Logger.Verbose("File {0} uploaded to FDC", fileName);
            }
            
            // mark done
            await blobClient.MarkAsUploadedAsync(fileName);
            Logger.Verbose("File {0} marked as uploaded", fileName);
        }

        /// <summary>
        /// Async Delegate to do processing after we construct the content of file.
        /// 1. Upload the content to blob store, upload pending
        /// 2. Ftp to FDC
        /// 3. Mark as uploaded
        /// </summary>
        /// <param name="content">
        /// Content of the file
        /// </param>
        /// <returns>
        /// Async Task Wrapper
        /// </returns>
        public async Task OnRewardsPtsBuild(string content)
        {
            IFtpClient ftpClient = FirstDataFtpClientFactory.FirstDataPtsFtpClient(Logger);

            string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");
            RewardsPtsBlobClient blobClient = FirstDataBlobClientFactory.RewardsPtsBlobClient(connectionString, Logger);

            // FDC requires EST.
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime estNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, easternZone);
            string fileName = estNow.ToString("yyyyMMddHmmss") + ".GPTD5628.TXT";

            //upload file to blob store
            byte[] contentBytes = Encoding.ASCII.GetBytes(content);
            MemoryStream ms = new MemoryStream(contentBytes);
            ms.Position = 0;
            await blobClient.UploadAsync(ms, fileName);
            Logger.Verbose("Uploaded file {0} to blob stored to be Ftped to FDC", fileName);

            // ftp it only if there are transactions
            if (!IsFileEmpty(ms))
            {
                ms.Position = 0;
                await ftpClient.UploadFileAsync(fileName, ms);
                Logger.Verbose("File {0} uploaded to FDC", fileName);

                ms.Position = 0;
                string fileNameOfCopy = DateTime.Now.ToString("yyyyMMddHmmss") + ".MCSFTPTS.TXT";
                await ftpClient.UploadFileAsync(fileNameOfCopy, ms);
            }

            // mark done
            await blobClient.MarkAsUploadedAsync(fileName);
            Logger.Verbose("File {0} marked as uploaded", fileName);
        }

        /// <summary>
        /// Check whether file is empty.
        /// </summary>
        /// <param name="ms">
        /// Stream contents of the file
        /// </param>
        /// <returns>
        /// True/False
        /// </returns>
        private bool IsFileEmpty(MemoryStream ms)
        {
            // build what empty file looks like
            StringBuilder builder = new StringBuilder();
            builder.Append(TotalRecordPtsBuilder.Build(0, 1));
            builder.Append("\n");
            string emptyContents = builder.ToString();

            ms.Position = 0;
            StreamReader reader =  new StreamReader(ms);
            string incomingContents = reader.ReadToEnd();
            if (incomingContents == emptyContents)
            {
                return true;
            }

            return false;
        }

        private CommerceLog Logger { get; set; }
    }
}