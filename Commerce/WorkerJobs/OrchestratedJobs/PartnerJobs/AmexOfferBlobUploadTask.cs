//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.IO;
    using System.Text;
    using Lomo.Commerce.AmexWorker;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;
    using Microsoft.Azure;

    /// <summary>
    /// Orchestrated task to upload offer registration record to blob store
    /// </summary>
    public class AmexOfferBlobUploadTask : IOrchestratedTask
    {
        /// <summary>
        /// Initializes a new instance of the AmexOfferBlobUploadTask class.
        /// </summary>
        /// <param name="offerRegistrationRecord">
        /// The offerRegistrationRecord is string representation of the data to upload
        /// </param>
        /// <param name="offerId">
        /// Amex Offer Id
        /// </param>
        /// <param name="merchantId">
        /// Amex Merchant Id
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        public AmexOfferBlobUploadTask(string offerRegistrationRecord, string merchantId, CommerceLog log)
        {
            OfferRegistrationRecord = offerRegistrationRecord;
            AmexMerchantId = merchantId;
            Log = log;
        }

        /// <summary>
        /// Initializes the AmexOfferBlobUploadTask instance.
        /// </summary>
        /// <param name="jobDetails">
        /// The details of the job being run.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler managing the jobs.
        /// </param>
        public void Initialize(ScheduledJobDetails jobDetails,
                               IScheduler scheduler)
        {
            JobDetails = jobDetails;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        /// The result of the execution of the task.
        /// </returns>
        public OrchestratedExecutionResult Execute()
        {
            OrchestratedExecutionResult result = OrchestratedExecutionResult.Success;
            try
            {
                // TODO: rename connection string, its no longer just FDC
                string connectionString = CloudConfigurationManager.GetSetting("Lomo.Commerce.Fdc.Blob.ConnectionString");

                OfferRegistrationRecordBlobClient client = new OfferRegistrationRecordBlobClient(connectionString, Log);

                string fileName = BlobName();
                byte[] contentBytes = Encoding.ASCII.GetBytes(OfferRegistrationRecord);
                MemoryStream ms = new MemoryStream(contentBytes);
                ms.Position = 0;
                client.UploadAsync(ms, fileName).Wait();
                JobDetails.Payload[AmexMerchantId] = AmexMerchantIdMarker;
            }
            catch (Exception exception)
            {
                Log.Warning("Unexpected error encountered during AmexOfferBlobUploadTask execution. Job will be retried. " +
                           "Exception:\r\n{0}", exception);
                result = OrchestratedExecutionResult.NonTerminalError;
            }

            return result;

        }

        /// <summary>
        /// Get Name of the blob file from offer id
        /// We can get same offer Id later, so it has to be unique
        /// </summary>
        /// <returns>
        /// string blob name
        /// </returns>
        private string BlobName()
        {
            return AmexMerchantId + "-" + General.GenerateShortGuid(Guid.NewGuid()) + ".txt";
        }

        /// <summary>
        /// String that marks an Merchant Id belonging to discount;
        /// </summary>
        public const string AmexMerchantIdMarker = "MerchantId";
        
        /// <summary>
        /// Gets or sets the AmexMerchantId.
        /// </summary>
        public string AmexMerchantId { get; set; }

        /// <summary>
        /// Gets or sets the OfferRegistrationRecord.
        /// </summary>
        private string OfferRegistrationRecord { get; set; }

        /// <summary>
        /// Gets or sets the details of the job being run.
        /// </summary>
        private ScheduledJobDetails JobDetails { get; set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }
    }
}