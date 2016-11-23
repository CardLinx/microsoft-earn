//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Commerce.WorkerCommon;

namespace Lomo.Commerce.VisaWorker
{
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Visa specific Settlement Processor Factory
    /// </summary>
    public static class VisaSettlementProcessorFactory
    {
        /// <summary>
        /// Gets the object to use to process Visa rebater transactions.
        /// </summary>
        /// <returns>
        /// The ISettlementProcessor object to use.
        /// </returns>
        public static ISettlementProcessor VisaRebateProcessor()
        {
            ISettlementProcessor result = new VisaRebaterProcessor();
            
            if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISettlementProcessor>("MockVisaRebaterProcessor", LateBoundMocksAssemblyTypes);
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