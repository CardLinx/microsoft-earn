//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Builds Invoker classes for test and production environments.
    /// </summary>
    public static class AmexInvokerFactory
    {
        /// <summary>
        /// Gets the object to use to perform operations on users.
        /// </summary>
        /// <param name="performanceInformation">
        /// The object through which performance information can be added and obtained.
        /// </param>
        /// <param name="amexInvokerOverride">
        /// The specific AmexInvoker object to return, if not null.
        /// </param>
        public static IAmexInvoker BuildAmexInvoker(PerformanceInformation performanceInformation,
            CommerceLog commerceLog,
            IAmexInvoker amexInvokerOverride)
        {
            IAmexInvoker result = new AmexInvoker();
            result.PerformanceInformation = performanceInformation;
            result.CommerceLog = commerceLog;

            if (CommerceServiceConfig.Instance.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IAmexInvoker>("MockAmexInvoker",
                    LateBoundMocksAssemblyTypes);
            }

            // If the override object is not null, return it instead.
            if (amexInvokerOverride != null)
            {
                result = amexInvokerOverride;
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