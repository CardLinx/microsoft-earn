//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Azure.Utils;
using Azure.Utils.Interface;
using Lomo.Logging;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using OfferManagement.JobProcessor.Jobs;

namespace OfferManagement.JobProcessor
{
    public enum JobType
    {
        ProvisionMasterCardMid,
        ProvisionVisaMid,
        SyncOfferWithCommerce,
        ProvisionMasterCardMerchants,
        ProvisionRewardNetworkMerchants,
        ProvisionRewardNetworkVisaMid,
        ProvisionAmexMid
    }

    public static class JobManager
    {
        private static IAzureBlob _azureBlob;
        private static AzureScheduler _azureScheduler;
        private static SchedulerQueueInfo _schedulerQueueInfo;
        private static string _commerceEndpointUrl;
        private static string _commerceCertThumbprint;

        public static void Start()
        {
            InitializeAzureBlob();
            InitializeAzureScheduler();
            ReadCommerceSettings();
        }

        public static IScheduledJob GetJobByType(JobType jobType)
        {
            IScheduledJob scheduledJob;

            switch (jobType)
            {
                case JobType.ProvisionMasterCardMid:
                    scheduledJob = new ProvisionMasterCardMids(_azureBlob, _azureScheduler, _schedulerQueueInfo);
                    break;
                case JobType.ProvisionVisaMid:
                    scheduledJob = new ProvisionVisaMids();
                    break;
                case JobType.SyncOfferWithCommerce:
                    ICommerceService commerceService = new CommerceService(_commerceCertThumbprint, _commerceEndpointUrl);
                    scheduledJob = new SyncOfferWithCommerce(commerceService);
                    break;
                case JobType.ProvisionMasterCardMerchants:
                    scheduledJob = new ProvisionMerchantsFromMasterCard(_azureBlob);
                    break;
                case JobType.ProvisionRewardNetworkMerchants:
                    scheduledJob = new ProvisionRewardNetworkMerchants();
                    break;
                case JobType.ProvisionRewardNetworkVisaMid:
                    scheduledJob = new ProvisionRewardNetworkVisaMids(_azureBlob);
                    break;
                case JobType.ProvisionAmexMid:
                    scheduledJob = new ProvisionAmexMids(_azureBlob);
                    break;
                default:
                    throw new InvalidEnumArgumentException("jobtype is invalid");
            }

            return scheduledJob;
        }

        public static async void DeleteJob(string jobId)
        {
            await _azureScheduler.DeleteJob(jobId);
        }

        private static void InitializeAzureBlob()
        {
            Log.Info("Initializing AzureBlob");
            Log.Info("Reading blob storage connection string from config");
            string connectionString = CloudConfigurationManager.GetSetting("EarnStorageConnectionString");

            IRetryPolicy retryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(6), 4);
            _azureBlob = new AzureBlob(connectionString, retryPolicy);
            Log.Info("Initialized AzureBlob");
        }

        private static void InitializeAzureScheduler()
        {
            Log.Info("Initializing AzureScheduler");
            Log.Info("Reading subscription certificate thumbprint from config");
            string schedulerCertThumbPrint = CloudConfigurationManager.GetSetting("SubscriptionCertificateThumbprint");

            Log.Info("Getting certificate from store");
            X509Certificate2 subscriptionCertificate = Lomo.Core.Cryptography.Certificates.ByName(schedulerCertThumbPrint, StoreLocation.LocalMachine,
                X509FindType.FindByThumbprint, false);
            if (subscriptionCertificate == null)
            {
                throw new Exception($"Unable to get the certificate with thumprint {schedulerCertThumbPrint} from LocalMachine ");
            }

            Log.Info("Successfully got certificate from store");

            Log.Info("Reading SubscriptionId from config");
            string subscriptionId = CloudConfigurationManager.GetSetting("SubscriptionId");

            Log.Info("Reading SchedulerCloudServiceId from config");
            string cloudServiceId = CloudConfigurationManager.GetSetting("SchedulerCloudServiceId");

            Log.Info("Reading SchedulerJobCollectionName from config");
            string schedulerJobCollection = CloudConfigurationManager.GetSetting("SchedulerJobCollectionName");

            _azureScheduler = new AzureScheduler(subscriptionCertificate, subscriptionId, cloudServiceId, schedulerJobCollection);
            _schedulerQueueInfo = new SchedulerQueueInfo
            {
                AccountName = CloudConfigurationManager.GetSetting("StorageAccountName"),
                QueueName = CloudConfigurationManager.GetSetting("EarnJobsQueueName"),
                SasToken = CloudConfigurationManager.GetSetting("QueueSasToken")
            };
            Log.Info("Initialized AzureScheduler");
        }

        private static void ReadCommerceSettings()
        {
            Log.Info("Reading commerce certificate thumbprint from config");
            _commerceCertThumbprint = CloudConfigurationManager.GetSetting("CommerceCertificateThumbprint");

            Log.Info("Reading commerce endpoint url from config");
            _commerceEndpointUrl = CloudConfigurationManager.GetSetting("CommerceEndpointUrl");
        }
    }
}