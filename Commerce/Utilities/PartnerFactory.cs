//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Utilities
{
    using System;
    using System.Collections.Generic;
    using AnalyticsClient;
    using Azure.Utils;
    using Lomo.AssemblyUtils;
    using Lomo.Authorization;
    using Lomo.Commerce.Configuration;
    using Lomo.Scheduler;
    using LoMo.UserServices.DataContract;
    using Microsoft.Azure;
    using Users.Dal;

    /// <summary>
    /// Provides methods to create instances of partner classes or their mocks.
    /// </summary>
    public static class PartnerFactory
    {
        /// <summary>
        /// Gets the object to use to perform operations on users.
        /// </summary>
        /// <param name="commerceConfig">
        /// The CommerceConfig object to use to determine if mock partner dependencies are being used.
        /// </param>
        public static IUsersDal UsersDal(CommerceConfig commerceConfig)
        {
            if (commerceConfig == null)
            {
                throw new ArgumentNullException("commerceConfig", "Parameter commerceConfig cannot be null.");
            }

            if (usersDal == null)
            {
                usersDal = new UsersDal();

                if (commerceConfig.UseMockPartnerDependencies == true)
                {
                    usersDal = LateBinding.BuildObjectFromLateBoundAssembly<IUsersDal>("MockUsersDal",
                                                                                       LateBoundMocksAssemblyTypes);
                }
            }

            return usersDal;
        }
        private static IUsersDal usersDal;

        /// <summary>
        /// Gets the object to use to perform user services client operations.
        /// </summary>
        /// <param name="commerceConfig">
        /// The CommerceConfig object to use to determine if mock partner dependencies are being used.
        /// </param>
        /// <param name="userServicesUri">
        /// The Uri at which the User Services can be found.
        /// </param>
        /// <returns>
        /// The IUserServicesClient instance to use.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter commerceConfig cannot be null.
        /// </exception>
        public static IUserServicesClient UserServicesClient(Uri userServicesUri, CommerceConfig commerceConfig)
        {
            if (commerceConfig == null)
            {
                throw new ArgumentNullException("commerceConfig", "Parameter commerceConfig cannot be null.");
            }

            IUserServicesClient result = new UserServiceClient(userServicesUri);

            if (commerceConfig.UseMockPartnerDependencies == true)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IUserServicesClient>("MockUserServicesClient",
                                                                                           LateBoundMocksAssemblyTypes);
            }

            return result;
        }

        /// <summary>
        /// Get the object to add analytics data
        /// </summary>
        /// <param name="commerceConfig">
        /// The CommerceConfig object to use to determine if mock partner dependencies are being used.
        /// </param>
        /// <returns>
        /// The IAnalytics client instance to use.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter commerceConfig cannot be null.
        /// </exception>
        public static IAnalyticsClient AnalyticsClient(CommerceConfig commerceConfig)
        {
            if (commerceConfig == null)
            {
                throw new ArgumentNullException("commerceConfig", "Parameter commerceConfig cannot be null.");
            }

            if (analyticsClient == null)
            {
                analyticsClient = new AnalyticsClient();

                if (commerceConfig.UseMockPartnerDependencies == true)
                {
                    analyticsClient = LateBinding.BuildObjectFromLateBoundAssembly<IAnalyticsClient>("MockAnalyticsClient",
                                                                                                    LateBoundMocksAssemblyTypes);
                }
            }

            return analyticsClient;
        }
        private static IAnalyticsClient analyticsClient;

        /// <summary>
        /// Get the object to request simple web tokens.
        /// </summary>
        /// <returns>
        /// The ISimpleWebTokenRequestor instance to use.
        /// </returns>
        public static ISimpleWebTokenRequestor SimpleWebTokenRequestor()
        {
            if (simpleWebTokenRequestor == null)
            {
                simpleWebTokenRequestor = new SimpleWebTokenRequestor();
                if (CommerceWorkerConfig.Instance.UseMockPartnerDependencies == true)
                {
                    simpleWebTokenRequestor =
                            LateBinding.BuildObjectFromLateBoundAssembly<ISimpleWebTokenRequestor>("MockSimpleWebTokenRequestor",
                                                                                                   LateBoundMocksAssemblyTypes);
                }
            }

            return simpleWebTokenRequestor;
        }
        private static ISimpleWebTokenRequestor simpleWebTokenRequestor;

        /// <summary>
        /// Gets the object for azure table to add certain warnings
        /// </summary>
        /// <param name="config">
        /// The configuration to use to get settings.
        /// </param>
        /// <returns>
        /// The IAzureTable instance to use.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter config cannot be null.
        /// </exception>
        public static IAzureTable AzureTable(CommerceConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config", "Parameter config cannot be null.");
            }

            if (azureTable == null)
            {
                azureTable = new AzureTable();

                if (config.UseMockPartnerDependencies == true)
                {
                    azureTable = LateBinding.BuildObjectFromLateBoundAssembly<IAzureTable>("MockAzureTable",
                                                                                       LateBoundMocksAssemblyTypes);
                }
            }

            return azureTable;
        }
        private static IAzureTable azureTable;

        /// <summary>
        /// Gets the Mock Scheduler
        /// </summary>
        /// <param name="queueName">
        /// Queue Name
        /// </param>
        /// <param name="tableName">
        /// Table Name
        /// </param>
        /// <returns>
        /// The IScheduler instance to use.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter config cannot be null.
        /// </exception>
        public static IScheduler Scheduler(string queueName,
                                           string tableName,
                                           CommerceConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config", "Parameter config cannot be null.");
            }

            if (scheduler == null)
            {
                scheduler =
                  SchedulerFactory.GetScheduler(CloudConfigurationManager.GetSetting("Lomo.Commerce.Scheduler.ConnectionString"),
                                                queueName, tableName);
                if (config.UseMockPartnerDependencies == true)
                {
                    scheduler = LateBinding.BuildObjectFromLateBoundAssembly<IScheduler>("MockScheduler",
                                                                                         LateBoundMocksAssemblyTypes);
                }
            }

            return scheduler;
        }
        private static IScheduler scheduler;

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