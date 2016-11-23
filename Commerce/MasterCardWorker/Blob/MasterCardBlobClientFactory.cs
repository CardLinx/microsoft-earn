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
    using Lomo.Commerce.Logging;
    using Microsoft.WindowsAzure;

    /// <summary>
    /// Factory to create MasterCard blob store clients.
    /// </summary>
    public static class MasterCardBlobClientFactory
    {
        /// <summary>
        /// Creates an instance of a MasterCard Filtering blob client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        /// <returns>
        /// And instace of the MasterCard Filtering blob client.
        /// </returns>
        public static MasterCardFilteringBlobClient MasterCardFilteringBlobClient(CommerceLog log)
        {
            MasterCardFilteringBlobClient result = null;

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<MasterCardFilteringBlobClient>("MockMasterCardFilteringBlobClient", LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new MasterCardFilteringBlobClient(log);
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of a MasterCard Clearing blob client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        /// <returns>
        /// And instace of the MasterCard Clearing blob client.
        /// </returns>
        public static MasterCardClearingBlobClient MasterCardClearingBlobClient(CommerceLog log)
        {
            MasterCardClearingBlobClient result = null;

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<MasterCardClearingBlobClient>("MockMasterCardClearingBlobClient", LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new MasterCardClearingBlobClient(log);
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of a MasterCard Rebate blob client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        /// <returns>
        /// And instace of the MasterCard Rebate blob client.
        /// </returns>
        public static MasterCardRebateBlobClient MasterCardRebateBlobClient(CommerceLog log)
        {
            MasterCardRebateBlobClient result = null;

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<MasterCardRebateBlobClient>("MockMasterCardRebateBlobClient", LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new MasterCardRebateBlobClient(log);
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of a MasterCard Rebate Confirmation blob client.
        /// </summary>
        /// <param name="log">
        /// The CommerceLog object within which to place log entries.
        /// </param>
        /// <returns>
        /// And instace of the MasterCard Rebate Confirmation blob client.
        /// </returns>
        public static MasterCardRebateConfirmationBlobClient MasterCardRebateConfirmationBlobClient(CommerceLog log)
        {
            MasterCardRebateConfirmationBlobClient result = null;

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<MasterCardRebateConfirmationBlobClient>("MockMasterCardRebateConfirmationBlobClient",
                                                                                                              LateBoundMocksAssemblyTypes);
            }
            else
            {
                result = new MasterCardRebateConfirmationBlobClient(log);
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