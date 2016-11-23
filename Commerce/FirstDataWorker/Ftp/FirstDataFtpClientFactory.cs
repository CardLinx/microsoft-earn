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
    using Lomo.Commerce.FtpClient;

    /// <summary>
    /// Factory to create instances for FDC FTP clients
    /// </summary>
    public static class FirstDataFtpClientFactory
    {
        /// <summary>
        /// Creates instance of FDC Extract FTP Client
        /// </summary>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of ftp client
        /// </returns>
        public static IFtpClient FirstDataExtractFtpClient(CommerceLog log)
        {
            IFtpClient result = new FirstDataExtractFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockFirstDataFtpClient",
                                                                                                LateBoundMocksAssemblyTypes);
            }
            
            return result;
        }

        /// <summary>
        /// Creates instance of FDC Ack FTP Client
        /// </summary>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of ftp client
        /// </returns>
        public static IFtpClient FirstDataAcknowledgmentFtpClient(CommerceLog log)
        {
            IFtpClient result = new FirstDataAcknowledgmentFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockFirstDataFtpClient",
                                                                                                LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Creates instance of FDC Pts FTP Client
        /// </summary>
        /// <param name="log">
        /// Commerce Logger
        /// </param>
        /// <returns>
        /// Instance of ftp client
        /// </returns>
        public static IFtpClient FirstDataPtsFtpClient(CommerceLog log)
        {
            IFtpClient result = new FirstDataPtsFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockFirstDataFtpClient",
                                                                                                LateBoundMocksAssemblyTypes);
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