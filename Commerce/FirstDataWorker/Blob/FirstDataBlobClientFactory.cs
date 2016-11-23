//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Logging;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;

    /// <summary>
    /// Factory to create FDC blob store clients
    /// </summary>
    public static class FirstDataBlobClientFactory
    {
        /// <summary>
        ///  Gets the FDC Extract Blob Client
        /// </summary>
        /// <param name="connectionString">
        /// Storage connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instace of FDC Extract Blob Client
        /// </returns>
        public static FirstDataExtractBlobClient FirstDataExtractBlobClient(string connectionString, CommerceLog log)
        {
            FirstDataExtractBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<FirstDataExtractBlobClient>("MockFirstDataExtractBlobClient",
                                                                                                LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new FirstDataExtractBlobClient(connectionString, log);
            }

            return result;
        }

        /// <summary>
        ///  Gets the FDC Ack Blob Client
        /// </summary>
        /// <param name="connectionString">
        /// Storage connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instace of FDC Ack Blob Client
        /// </returns>
        public static FirstDataAcknowledgmentBlobClient FirstDataAcknowledgmentBlobClient(string connectionString, CommerceLog log)
        {
            FirstDataAcknowledgmentBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<FirstDataAcknowledgmentBlobClient>("MockFirstDataAcknowledgmentBlobClient",
                                                                                                LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new FirstDataAcknowledgmentBlobClient(connectionString, log);
            }

            return result;
        }

        /// <summary>
        ///  Gets the FDC Pts Blob Client
        /// </summary>
        /// <param name="connectionString">
        /// Storage connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instace of FDC Pts Blob Client
        /// </returns>
        public static FirstDataPtsBlobClient FirstDataPtsBlobClient(string connectionString, CommerceLog log)
        {
            FirstDataPtsBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<FirstDataPtsBlobClient>("MockFirstDataPtsBlobClient",
                                                                                                LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new FirstDataPtsBlobClient(connectionString, log);
            }

            return result;
        }

        /// <summary>
        ///  Gets the Rewards Pts Blob Client
        /// </summary>
        /// <param name="connectionString">
        /// Storage connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instace of Rewards Pts Blob Client
        /// </returns>
        public static RewardsPtsBlobClient RewardsPtsBlobClient(string connectionString, CommerceLog log)
        {
            RewardsPtsBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<RewardsPtsBlobClient>("MockRewardsPtsBlobClient", LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new RewardsPtsBlobClient(connectionString, log);
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
    }
}