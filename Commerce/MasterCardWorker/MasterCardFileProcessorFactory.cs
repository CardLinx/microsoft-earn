//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.WorkerCommon;

    /// <summary>
    /// Builds MasterCard file processor classes for test and production environments.
    /// </summary>
    public static class MasterCardFileProcessorFactory
    {
        /// <summary>
        /// Gets the object to use to process MasterCard filtering files.
        /// </summary>
        /// <param name="uploadFilteringFile">
        /// The delegate to call to upload the file to its various destinations.
        /// </param>
        /// <returns>
        /// The ISettlementFileProcessor object to use.
        /// </returns>
        public static ISettlementFileProcessor MasterCardFilteringProcessor(Func<string, Task> uploadFilteringFile)
        {
            ISettlementFileProcessor result = new MasterCardFilteringProcessor()
            {
                UploadFilteringFile = uploadFilteringFile
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockMasterCardFilteringProcessor",
                                                                                                LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the object to use to process MasterCard clearing files.
        /// </summary>
        /// <param name="stream">
        /// The stream containing clearing file contents.
        /// </param>
        /// <param name="fileName">
        /// The name of the clearing file.
        /// </param>
        /// <returns>
        /// The ISettlementFileProcessor object to use.
        /// </returns>
        public static ISettlementFileProcessor MasterCardClearingProcessor(Stream stream,
                                                                           string fileName)
        {
            ISettlementFileProcessor result = new MasterCardClearingProcessor()
            {
                Stream = stream,
                FileName = fileName
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockMasterCardClearingProcessor", LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the object to use to process MasterCard rebate files.
        /// </summary>
        /// <param name="uploadRebateFile">
        /// The delegate to call to upload the file to its various destinations.
        /// </param>
        /// <returns>
        /// The ISettlementFileProcessor object to use.
        /// </returns>
        public static ISettlementFileProcessor MasterCardRebateProcessor(Func<string, Task> uploadRebateFile)
        {
            ISettlementFileProcessor result = new MasterCardRebateProcessor()
            {
                UploadRebateFile = uploadRebateFile
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockMasterCardRebateProcessor",
                                                                                                LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Gets the object to use to process MasterCard rebate confirmation files.
        /// </summary>
        /// <param name="stream">
        /// The stream containing rebate confirmation file contents.
        /// </param>
        /// <param name="fileName">
        /// The name of the rebate confirmation file.
        /// </param>
        /// <returns>
        /// The ISettlementFileProcessor object to use.
        /// </returns>
        public static ISettlementFileProcessor MasterCardRebateConfirmationProcessor(Stream stream,
                                                                                     string fileName)
        {
            ISettlementFileProcessor result = new MasterCardRebateConfirmationProcessor()
            {
                Stream = stream,
                FileName = fileName
            };

            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementFileProcessor>("MockMasterCardRebateConfirmationProcessor", LateBoundMocksAssemblyTypes);
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