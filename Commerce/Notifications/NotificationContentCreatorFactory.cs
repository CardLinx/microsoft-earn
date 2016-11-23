//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    using System;
    using System.Collections.Generic;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Context;
    using Microsoft.Azure;

    /// <summary>
    /// Factory to create instance of implementation of INotificationContentCreator
    /// </summary>
    public static class NotificationContentCreatorFactory
    {
        /// <summary>
        /// Get instance of content creator
        /// </summary>
        /// <param name="context">
        /// Commerce context
        /// </param>
        /// <returns>
        /// Instnace of content creator
        /// </returns>
        public static INotificationContentCreator NotificationContentCreator(CommerceContext context)
        {
            if (context== null)
            {
                throw new ArgumentNullException("context", "Parameter config cannot be null.");
            }

            if (notificationContentCreator == null)
            {
                if (context.Config.UseMockPartnerDependencies)
                {
                    //TODO: create mock
                    notificationContentCreator = LateBinding.BuildObjectFromLateBoundAssembly<INotificationContentCreator>("MockNotificationContentCreator",
                                                                                         LateBoundMocksAssemblyTypes);
                }
                else
                {
                    notificationContentCreator = new NotificationContentCreator()
                    {
                        TemplateServiceBaseAddress = CloudConfigurationManager.GetSetting("Lomo.Commerce.TemplateServiceUri"),
                        Logger = context.Log
                    }; 
                }
            }

            return notificationContentCreator;
        }
        private static INotificationContentCreator notificationContentCreator;

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