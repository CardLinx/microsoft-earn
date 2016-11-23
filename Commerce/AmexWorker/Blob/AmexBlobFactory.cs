//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexWorker
{
    using System;
    using System.Collections.Generic;
    using Lomo.Commerce.Logging;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;

    /// <summary>
    /// Factory to create Amex blob store clients
    /// </summary>
    public static class AmexBlobFactory
    {
        /// <summary>
        /// Offer Registration Record Blob Client Factory Method
        /// </summary>
        /// <param name="connectionString">
        /// Storage account connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of OfferRegistrationRecordBlobClient
        /// </returns>
        public static OfferRegistrationRecordBlobClient OfferRegistrationRecordBlobClient(string connectionString, CommerceLog log)
        {
            OfferRegistrationRecordBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<OfferRegistrationRecordBlobClient>("MockAmexOfferRegistrationRecordBlobClient",
                                                                                                LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new OfferRegistrationRecordBlobClient(connectionString, log);
            }

            return result;
        }

        /// <summary>
        /// Offer Registration Response File Blob Client Factory Method
        /// </summary>
        /// <param name="connectionString">
        /// Storage account connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of OfferRegistrationResponseFileBlobClient
        /// </returns>
        public static OfferRegistrationResponseFileBlobClient OfferRegistrationResponseFileBlobClient(string connectionString, CommerceLog log)
        {
            OfferRegistrationResponseFileBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result =
                    LateBinding.BuildObjectFromLateBoundAssembly<OfferRegistrationResponseFileBlobClient>(
                        "MockAmexOfferRegistrationResponseFileBlobClient",
                        LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new OfferRegistrationResponseFileBlobClient(connectionString, log);
            }
            return result;
        }

        /// <summary>
        /// Amex Transaction Log File Blob Client Factory Method
        /// </summary>
        /// <param name="connectionString">
        /// Storage account connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of AmexTransactionLogFileBlobClient
        /// </returns>
        public static AmexTransactionLogFileBlobClient TransactionLogBlobClient(string connectionString, CommerceLog log)
        {
            AmexTransactionLogFileBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result =
                    LateBinding.BuildObjectFromLateBoundAssembly<AmexTransactionLogFileBlobClient>(
                        "MockAmexTransactionLogFileBlobClient",
                        LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new AmexTransactionLogFileBlobClient(connectionString, log);
            }
            return result;
        }

        /// <summary>
        /// Amex Statement Credit File Blob Client Factory Method
        /// </summary>
        /// <param name="connectionString">
        /// Storage account connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of AmexStatementCreditFileBlobClient
        /// </returns>
        public static AmexStatementCreditFileBlobClient StatementCreditFileBlobClient(string connectionString, CommerceLog log)
        {
            AmexStatementCreditFileBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result =
                    LateBinding.BuildObjectFromLateBoundAssembly<AmexStatementCreditFileBlobClient>(
                        "MockAmexStatementCreditFileBlobClient",
                        LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new AmexStatementCreditFileBlobClient(connectionString, log);
            }
            return result;
        }

        /// <summary>
        /// Amex Statement Credit File Response Blob Client Factory Method
        /// </summary>
        /// <param name="connectionString">
        /// Storage account connection string
        /// </param>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of AmexStatementCreditResponseFileBlobClient
        /// </returns>
        public static AmexStatementCreditResponseFileBlobClient StatementCreditResponseFileBlobClient(string connectionString, CommerceLog log)
        {
            AmexStatementCreditResponseFileBlobClient result = null;
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result =
                    LateBinding.BuildObjectFromLateBoundAssembly<AmexStatementCreditResponseFileBlobClient>(
                        "MockAmexStatementCreditResponseFileBlobClient",
                        LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new AmexStatementCreditResponseFileBlobClient(connectionString, log);
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