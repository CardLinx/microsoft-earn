//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.VisaClient
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Builds Invoker classes for test and production environments.
    /// </summary>
    public static class VisaInvokerFactory
    {
        /// <summary>
        /// Gets the object to use to perform operations on users.
        /// </summary>
        /// <param name="performanceInformation">
        /// The object through which performance information can be added and obtained.
        /// </param>
        /// <param name="visaInvokerOverride">
        /// The specific VisaInvoker object to return, if not null.
        /// </param>
        /// <param name="config">
        /// The config object.
        /// </param>

        public static IVisaInvoker BuildVisaInvoker(PerformanceInformation performanceInformation,
                                                              IVisaInvoker visaInvokerOverride, CommerceConfig config)
        {
            IVisaInvoker result = new VisaInvoker();
            result.PerformanceInformation = performanceInformation;

            if (config.UseMockPartnerDependencies)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IVisaInvoker>("MockVisaInvoker",
                                                                                         LateBoundMocksAssemblyTypes);
            }

            // If the override object is not null, return it instead.
            if (visaInvokerOverride != null)
            {
                result = visaInvokerOverride;
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
                if (_lateBoundMocksAssemblyTypes == null)
                {
                    _lateBoundMocksAssemblyTypes = LateBinding.GetLateBoundAssemblyTypes(MocksAssemblyName);
                }

                return _lateBoundMocksAssemblyTypes;
            }
        }

        private static IEnumerable<Type> _lateBoundMocksAssemblyTypes;

        /// <summary>
        /// The fully qualified name of the mocks assembly.
        /// </summary>
        private const string MocksAssemblyName = "Lomo.Commerce.Test.Mocks.dll";
    }
}