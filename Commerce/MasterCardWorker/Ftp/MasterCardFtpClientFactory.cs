//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.FtpClient;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Factory class to create instances of MasterCard FTP clients.
    /// </summary>
    public static class MasterCardFtpClientFactory
    {
        /// <summary>
        /// Creates an instance of the filtering file FTP client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        /// <returns>
        /// An instance of the filtering file FTP client.
        /// </returns>
        public static IFtpClient FilteringFtpClient(CommerceLog log)
        {
            IFtpClient result = new FilteringFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockMasterCardFtpClient", LateBoundMocksAssemblyTypes);
            }
            
            return result;
        }

        /// <summary>
        /// Creates an instance of the clearing file FTP client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        /// <returns>
        /// An instance of the clearing file FTP client.
        /// </returns>
        public static IFtpClient ClearingFtpClient(CommerceLog log)
        {
            IFtpClient result = new ClearingFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockMasterCardFtpClient", LateBoundMocksAssemblyTypes);
            }
            
            return result;
        }

        /// <summary>
        /// Creates an instance of the rebate file FTP client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        /// <returns>
        /// An instance of the rebate file FTP client.
        /// </returns>
        public static IFtpClient RebateFtpClient(CommerceLog log)
        {
            IFtpClient result = new RebateFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockMasterCardFtpClient", LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of the rebate confirmation file FTP client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        /// <returns>
        /// An instance of the rebate confirmation file FTP client.
        /// </returns>
        public static IFtpClient RebateConfirmationFtpClient(CommerceLog log)
        {
            IFtpClient result = new RebateConfirmationFtpClient(log);
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFtpClient>("MockMasterCardFtpClient", LateBoundMocksAssemblyTypes);
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