//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Builds Invoker classes for test and production environments.
    /// </summary>
    public static class MasterCardInvokerFactory
    {
        /// <summary>
        /// Gets the object to use to perform operations on users.
        /// </summary>
        /// <param name="performanceInformation">
        /// The object through which performance information can be added and obtained.
        /// </param>
        /// <param name="masterCardInvokerOverride">
        /// The specific MasterCardInvoker object to return, if not null.
        /// </param>
        public static IMasterCardInvoker BuildMasterCardInvoker(PerformanceInformation performanceInformation,
                                                                IMasterCardInvoker masterCardInvokerOverride)
        {
            IMasterCardInvoker result = new MasterCardInvoker();
            result.PerformanceInformation = performanceInformation;
            if (UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IMasterCardInvoker>("MockMasterCardInvoker",
                                                                                         LateBoundMocksAssemblyTypes);
            }

            // If the override object is not null, return it instead.
            if (masterCardInvokerOverride != null)
            {
                result = masterCardInvokerOverride;
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

        /// <summary>
        /// Gets a value that indicates whether the factory should build a mock object.
        /// </summary>
        private static bool UseMockPartnerDependencies
        {
            get
            {
                if (useMockPartnerDependenciesSet == false)
                {
                    CommerceConfig config = CommerceServiceConfig.Instance;
                    if (config == null)
                    {
                        config = CommerceWorkerConfig.Instance;
                    }

                    useMockPartnerDependencies = config.UseMockPartnerDependencies;
                    useMockPartnerDependenciesSet = true;
                }

                return useMockPartnerDependencies;
            }
        }
        private static bool useMockPartnerDependencies;
        private static bool useMockPartnerDependenciesSet;

        /// <summary>
        /// The types found within the late bound mocks assembly.
        /// </summary>
        private static IEnumerable<Type> lateBoundMocksAssemblyTypes;

        /// <summary>
        /// The fully qualified name of the mocks assembly.
        /// </summary>
        private const string MocksAssemblyName = "Lomo.Commerce.Test.Mocks.dll";
    }
}