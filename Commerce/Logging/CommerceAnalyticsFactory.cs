//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;

    /// <summary>
    /// Provides methods to create instances of commerce analytics classes or their mocks.
    /// </summary>
    public static class CommerceAnalyticsFactory
    {
        /// <summary>
        /// Gets the object to use to perform commerce analytics actions.
        /// </summary>
        /// <param name="commerceConfig">
        /// The CommerceConfig object to use to determine if mock partner dependencies are being used.
        /// </param>
        /// <returns>
        /// The IAnalyticsUserInfo instance to use.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter commerceConfig cannot be null.
        /// </exception>
        public static IAnalyticsUserInfo AnalyticsUserInfo(CommerceConfig commerceConfig)
        {
            if (commerceConfig == null)
            {
                throw new ArgumentNullException("commerceConfig", "Parameter commerceConfig cannot be null.");
            }

            if (analyticsUserInfo == null)
            {
                analyticsUserInfo = new AnalyticsUserInfo();

                if (commerceConfig.UseMockPartnerDependencies == true)
                {
                    analyticsUserInfo = LateBinding.BuildObjectFromLateBoundAssembly<IAnalyticsUserInfo>("MockAnalyticsUserInfo",
                                                                                                    LateBoundMocksAssemblyTypes);
                }
            }

            return analyticsUserInfo;
        }
        private static IAnalyticsUserInfo analyticsUserInfo;

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