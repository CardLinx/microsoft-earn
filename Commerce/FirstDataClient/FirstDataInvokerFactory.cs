//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Builds Invoker classes for test and production environments.
    /// </summary>
    public static class FirstDataInvokerFactory
    {
        /// <summary>
        /// Gets the object to use to perform operations on users.
        /// </summary>
        /// <param name="performanceInformation">
        /// The object through which performance information can be added and obtained.
        /// </param>
        /// <param name="firstDataInvokerOverride">
        /// The specific FirstDataInvoker object to return, if not null.
        /// </param>
        public static IFirstDataInvoker BuildFirstDataInvoker(PerformanceInformation performanceInformation,
                                                              IFirstDataInvoker firstDataInvokerOverride)
        {
            IFirstDataInvoker result = new FirstDataInvoker();
            result.PerformanceInformation = performanceInformation;
            if (CommerceServiceConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IFirstDataInvoker>("MockFirstDataInvoker",
                                                                                         LateBoundMocksAssemblyTypes);
            }

            // If the override object is not null, return it instead.
            if (firstDataInvokerOverride != null)
            {
                result = firstDataInvokerOverride;
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