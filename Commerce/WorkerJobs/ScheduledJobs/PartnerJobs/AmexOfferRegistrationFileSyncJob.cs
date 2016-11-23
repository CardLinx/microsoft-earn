//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.Commerce.AmexWorker;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.Logic;
    using Lomo.Commerce.Logging;
    using Lomo.Scheduler;
    using Lomo.Commerce.Utilities;
    using Microsoft.Azure;

    /// <summary>
    /// A recurring job that uploads offer registration request file to Amex SFTP Servers 
    /// and checks for the response as well.
    /// </summary>
    public class AmexOfferRegistrationFileSyncJob : IScheduledJob
    {
        public AmexOfferRegistrationFileSyncJob()
        {
            Context = new CommerceContext("Amex Offer Registration File Sync Job", CommerceWorkerConfig.Instance);
            Scheduler = PartnerFactory.Scheduler(CommerceWorkerConfig.Instance.SchedulerQueueName,
                                                 CommerceWorkerConfig.Instance.SchedulerTableName,
                                                 CommerceWorkerConfig.Instance);
        }

        /// <summary>
        /// Execute the file sync job
        /// </summary>
        /// <param name="details">
        /// Details of the job we are executing here. 
        /// </param>
        /// <param name="logger">
        /// Handle to the logger
        /// </param>
        /// <remarks>
        /// This job:
        /// 1. If response is pending, it waits for response to be available
        /// 2. When response becomes available, the job processes it and puts flag that response is processed
        /// 3. If response is not pending, it checks to see if there are records to upload and uploads them
        /// 4. After file is sent to Amex, job puts flag that response is being waited on
        /// </remarks>
        public async Task Execute(ScheduledJobDetails details, CommerceLog logger)
        {
            Logger = logger;
            JobDetails = details;
            Init();

            Logger.Exhaustive("Starting execution of job \r\n Details {0}", details);

            ////if (IsWaitingForResponse())
            ////{
            ////    // check the response 
            ////    string[] responseFileNames = await RetrieveResponseFileNamesAsync().ConfigureAwait(false);
            ////    if (responseFileNames == null)
            ////    {
            ////        // we have to wait for response. 
            ////        return;
            ////    }

            ////    await UploadResponseFilesToBlobStoreAsync(responseFileNames).ConfigureAwait(false);

            ////    // update job to mark, no response is expected now
            ////    UpdateResponsePendingIndicatorInJob("false");
            ////}
            ////else
            ////{
            ////    // process response files if present in blob store 
            ////    await ProcessResponseFilesAsync().ConfigureAwait(false);

            ////    // if we reach here, we have either processed the response or no response is being waited on
            ////    // so create new request file if needed
            ////    string requestFile = await CreateNewRequestFileIfNeededAsync().ConfigureAwait(false);

            ////    if (requestFile != null)
            ////    {
            ////        await SendRequestFileAsync(requestFile).ConfigureAwait(false);

            ////        // successfully sent file -> update job to wait for response
            ////        UpdateResponsePendingIndicatorInJob("true");

            ////    }
            ////}

            // Process if any response files are pending
            string[] responseFileNames = await RetrieveResponseFileNamesAsync().ConfigureAwait(false);
            if (responseFileNames != null && responseFileNames.Length > 0)
            {
                await UploadResponseFilesToBlobStoreAsync(responseFileNames).ConfigureAwait(false);
            }

            // Process if any requests files are pending
            string requestFile = await CreateNewRequestFileIfNeededAsync().ConfigureAwait(false);
            if (requestFile != null)
            {
                await SendRequestFileAsync(requestFile).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// fetch the response file
        /// </summary>
        /// <returns></returns>
        internal virtual async Task<string[]> RetrieveResponseFileNamesAsync()
        {
            string[] fileNames = await SftpResponseFileClient.DirectoryListAsync("MSF_AXP_MER_REG_RESP_", "outbox").ConfigureAwait(false);
            return fileNames;
        }

        /// <summary>
        /// Create new instance of processor the response file
        /// </summary>
        /// <returns></returns>
        internal virtual RegistrationResponseFileProcessor ResponseFileProcessor(string responseFileName, Stream stream)
        {
            return new RegistrationResponseFileProcessor()
            {
                Context = Context,
                ResponseFileName = responseFileName,
                ResponseFileStream = stream,
                Scheduler = Scheduler
            };
        }

        /// <summary>
        /// Initialize state
        /// </summary>
        private void Init()
        {
            SftpRequestFileClient = new AmexOfferRegistrationRequestSftpClient(Logger);
            SftpResponseFileClient = new AmexOfferRegistrationResponseSftpClient(Logger);
            ResponseFileBlobClient = AmexBlobFactory.OfferRegistrationResponseFileBlobClient(ConnectionString, Logger);
            DetailRecordsBloblClient = AmexBlobFactory.OfferRegistrationRecordBlobClient(ConnectionString, Logger);
        }

        /// <summary>
        /// Update job payload to indicate whether job is waititng on response or not
        /// </summary>
        private void UpdateResponsePendingIndicatorInJob(string flag)
        {
            JobDetails.Payload["responsePending"] = flag;
        }

        /// <summary>
        /// send registration request to amex
        /// </summary>
        private async Task SendRequestFileAsync(string file)
        {
            // MSF_AXP_mer_reg_ yymmdd_hhmmss.txt
            string dateformatted = DateTime.UtcNow.ToString("yyMMdd_hhmmss");
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(file);
                writer.Flush();
                stream.Position = 0;
                await SftpRequestFileClient.UploadFileAsync("MSF_AXP_mer_reg_" + dateformatted + ".txt", stream, "inbox").ConfigureAwait(false);
            }
        }

        /// <summary>
        /// create request file
        /// </summary>
        /// <returns>
        /// Async task wrapper
        /// </returns>
        private async Task<string> CreateNewRequestFileIfNeededAsync()
        {
            ICollection<string> recordNames = DetailRecordsBloblClient.RetrieveFilesToProcess();
            Collection<string> records = new Collection<string>();
            string fileToUpload = null;
            if (recordNames != null)
            {
                foreach (string recordName in recordNames)
                {
                    MemoryStream memStream = new MemoryStream();
                    memStream.Position = 0;
                    await DetailRecordsBloblClient.DownloadAsync(memStream, recordName).ConfigureAwait(false); ;

                    memStream.Position = 0;
                    using (StreamReader streamReader = new StreamReader(memStream))
                    {
                        string record = streamReader.ReadToEnd();
                        records.Add(record);
                    }

                    await DetailRecordsBloblClient.MarkAsProcessedAsync(recordName).ConfigureAwait(false); ;
                }
            }

            if (records.Count > 0)
            {
                Context[Key.SequenceName] = "AmexOfferRegistrationSequence";
                SharedSequenceLogic sequenceLogic = new SharedSequenceLogic(Context, CommerceOperationsFactory.SequenceOperations(Context));
                int sequenceNumber = sequenceLogic.RetrieveNextValueInSequence();
                fileToUpload = OfferRegistrationFileBuilder.Build(records, sequenceNumber, DateTime.UtcNow);
            }
           
            return fileToUpload;
        }

        /// <summary>
        /// process response
        /// </summary>
        private async Task UploadResponseFilesToBlobStoreAsync(string [] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                MemoryStream stream = new MemoryStream();
                await SftpResponseFileClient.DownloadFileAsync(fileName, stream, "outbox").ConfigureAwait(false);

                stream.Position = 0;
                await ResponseFileBlobClient.UploadAsync(stream, fileName).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Process the response file
        /// </summary>
        /// <returns>
        /// Task wrapper
        /// </returns>
        private async Task ProcessResponseFilesAsync()
        {
            // Now try to run all the pending files in the blob
            ICollection<string> listOfFiles = ResponseFileBlobClient.RetrieveFilesToProcess();
            if (listOfFiles != null)
            {
                foreach (string fileName in listOfFiles)
                {
                    MemoryStream memStream = new MemoryStream();
                    memStream.Position = 0;
                    await ResponseFileBlobClient.DownloadAsync(memStream, fileName);

                    memStream.Position = 0;
                    RegistrationResponseFileProcessor processor = ResponseFileProcessor(fileName, memStream);
                    bool fileSubmissionSuccess = await processor.ProcessAsync().ConfigureAwait(false);
                    await HandleResponse(fileSubmissionSuccess, fileName).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Handle the submission of response file
        /// </summary>
        /// <param name="submissionSuccessful">
        /// Submission status
        /// </param>
        /// <param name="fileName">
        /// Name of the file
        /// </param>
        internal async Task HandleResponse(bool submissionSuccessful, string fileName)
        {
            if (submissionSuccessful)
            {
                await ResponseFileBlobClient.MarkAsProcessedAsync(fileName).ConfigureAwait(false);
            }
            else
            {
                // decrease sequence number to we can submit same number again
                DecrementSequence();
            }
        }

        /// <summary>
        /// Decrease sequence number, in case file submission failed
        /// </summary>
        internal void DecrementSequence()
        {
            Context[Key.SequenceName] = "AmexOfferRegistrationSequence";
            SharedSequenceLogic sequenceLogic = new SharedSequenceLogic(Context, CommerceOperationsFactory.SequenceOperations(Context));
            sequenceLogic.DecrementSequence();
        }

        /// <summary>
        /// Checks whether we are waiting for a response from earlier upload
        /// </summary>
        /// <returns>
        /// True/False
        /// </returns>
        private bool IsWaitingForResponse()
        {
            bool responsePending;
            bool.TryParse(JobDetails.Payload["responsePending"], out responsePending);
            Logger.Exhaustive("Waiting for Response ? {0}", responsePending);
            return responsePending;
        }

        /// <summary>
        /// Gets or sets the job details
        /// </summary>
        private ScheduledJobDetails JobDetails { get; set; }

        /// <summary>
        /// Gets or sets the Logger
        /// </summary>
        private CommerceLog Logger { get; set; }

        /// <summary>
        /// Gets or sets tje Sftp request client
        /// </summary>
        private AmexOfferRegistrationRequestSftpClient SftpRequestFileClient { get; set; }
        
        /// <summary>
        /// Gets or sets the Sftp response client
        /// </summary>
        internal AmexOfferRegistrationResponseSftpClient SftpResponseFileClient { get; set; }

        /// <summary>
        /// Gets or sets the response file blob client
        /// </summary>
        private OfferRegistrationResponseFileBlobClient ResponseFileBlobClient { get; set; }

        /// <summary>
        /// Gets or sets the commerce context
        /// </summary>
        private CommerceContext Context{get; set; }

        /// <summary>
        /// Gets or sets the detail record blob client
        /// </summary>
        private OfferRegistrationRecordBlobClient DetailRecordsBloblClient { get; set; }

        /// <summary>
        /// Gets or set the blob store connection string
        /// </summary>
        private string ConnectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");

        /// <summary>
        /// Gets or set the Scheduler
        /// </summary>
        private IScheduler Scheduler { get; set; }
    }
}