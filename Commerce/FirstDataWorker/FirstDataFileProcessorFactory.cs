//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.WorkerCommon;
    using Microsoft.Azure;

    /// <summary>
    /// Builds First Data file processor classes for test and production environments.
    /// </summary>
    public static class FirstDataFileProcessorFactory
    {
        /// <summary>
        /// Gets the object to use to perform user services client operations.
        /// </summary>
        /// <param name="extractFileName">
        /// Name of the extract file
        /// </param>
        /// <param name="extractFileStream">
        /// Extract file data stream
        /// </param>
        public static ISettlementFileProcessor FirstDataExtractProcessor(string extractFileName, Stream extractFileStream)
        {
            ISettlementFileProcessor result = new FirstDataExtractProcessor()
            {
                ExtractFileStream = extractFileStream,
                ExtractFileName = extractFileName
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockFirstDataExtractProcessor",
                                                                                                LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform user services client operations.
        /// </summary>
        /// <param name="acknowledgmentFileName">
        /// Name of the ack file
        /// </param>
        /// <param name="acknowledgmentFileStream">
        /// Ack file data stream
        /// </param>
        public static ISettlementFileProcessor FirstDataAcknowledgmentProcessor(string acknowledgmentFileName, Stream acknowledgmentFileStream)
        {
            ISettlementFileProcessor result = new FirstDataAcknowledgmentProcessor()
            {
                AcknowledgmentFileName = acknowledgmentFileName,
                AcknowledgmentFileStream = acknowledgmentFileStream
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockFirstDataAcknowledgmentProcessor",
                                                                                                LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform user services client operations.
        /// </summary>
        /// <param name="onPtsBuild">
        /// Action to be performed once we have pts file contents built in memory
        /// </param>
        public static ISettlementFileProcessor FirstDataPtsProcessor(Func<string, Task> onPtsBuild)
        {
            ISettlementFileProcessor result = new FirstDataPtsProcessor()
            {
                OnPtsBuild = onPtsBuild,
                TransactionPublisher = new FirstDataTransactionPublisher
                {
                    Queue = new AzureQueueClient(UserServicesStorageConnectionString, TransactionQueueName)
                }
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockFirstDataPtsProcessor",
                                                                                                LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform user services client operations.
        /// </summary>
        /// <param name="onRewardsPtsBuild">
        /// Action to be performed once we have pts file contents built in memory
        /// </param>
        public static RewardsPtsProcessor RewardsPtsProcessor(Func<string, Task> onRewardsPtsBuild)
        {
            RewardsPtsProcessor result = new RewardsPtsProcessor()
            {
                OnRewardsPtsBuild = onRewardsPtsBuild,
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<RewardsPtsProcessor>("MockRewardsPtsProcessor", LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the Types that exist within the specified mocks assembly.
        /// </summary>
        private static IEnumerable<Type> LateBoundMocksAssemblyTypes
        {
            get
            {
                if (lateBoundMocksAssemblyTypes == null)
                {
                    lateBoundMocksAssemblyTypes = LateBinding.GetLateBoundAssemblyTypes(MocksAssemblyName);
                }

                return lateBoundMocksAssemblyTypes;
            }
        }

        private static IEnumerable<Type> lateBoundMocksAssemblyTypes;

        /// <summary>
        /// The fully qualified name of the mocks assembly.
        /// </summary>
        private const string MocksAssemblyName = "Lomo.Commerce.Test.Mocks.dll";

        /// <summary>
        /// Connection string for sending transaction messages
        /// </summary>
//TODO: We have these read config reads scattered all over. Need to consolidate !
        private readonly static string UserServicesStorageConnectionString = CloudConfigurationManager.GetSetting("LoMo.UserServices.ConnectionString");

        /// <summary>
        /// Queue Name for transaction publisher
        /// </summary>
        private const string TransactionQueueName = "partner-transactions";
    }
}