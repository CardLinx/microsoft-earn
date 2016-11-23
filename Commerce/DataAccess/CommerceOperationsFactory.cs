//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using Lomo.AssemblyUtils;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Builds Operations classes for test and production environments.
    /// </summary>
    public static class CommerceOperationsFactory
    {
        /// <summary>
        /// Gets the object to use to perform operations on users.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IUserOperations instance to use.
        /// </returns>
        public static IUserOperations UserOperations(CommerceContext context)
        {
            ValidateContext(context);

            IUserOperations result = new UserOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IUserOperations>("MockUserOperations",
                                                                                       LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on cards.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The ICardOperations instance to use.
        /// </returns>
        public static ICardOperations CardOperations(CommerceContext context)
        {
            ValidateContext(context);

            ICardOperations result = new CardOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ICardOperations>("MockCardOperations",
                                                                                       LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on deals.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IDealOperations instance to use.
        /// </returns>
        public static IDealOperations DealOperations(CommerceContext context)
        {
            ValidateContext(context);

            IDealOperations result = new DealOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IDealOperations>("MockDealOperations",
                                                                                       LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on sequences.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The ISequenceOperations instance to use.
        /// </returns>
        public static ISequenceOperations SequenceOperations(CommerceContext context)
        {
            ValidateContext(context);

            ISequenceOperations result = new SequenceOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<ISequenceOperations>("MockSequenceOperations",
                                                                                       LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on claimed deals.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IClaimedDealOperations instance to use.
        /// </returns>
        public static IClaimedDealOperations ClaimedDealOperations(CommerceContext context)
        {
            ValidateContext(context);

            IClaimedDealOperations result = new ClaimedDealOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IClaimedDealOperations>("MockClaimedDealOperations",
                                                                                              LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on redeemed deals.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IRedeemedDealOperations instance to use.
        /// </returns>
        public static IRedeemedDealOperations RedeemedDealOperations(CommerceContext context)
        {
            ValidateContext(context);

            IRedeemedDealOperations result = new RedeemedDealOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IRedeemedDealOperations>("MockRedeemedDealOperations",
                                                                                               LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on authorizations.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IAuthorizationOperations instance to use.
        /// </returns>
        public static IAuthorizationOperations AuthorizationOperations(CommerceContext context)
        {
            ValidateContext(context);

            IAuthorizationOperations result = new AuthorizationOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IAuthorizationOperations>("MockAuthorizationOperations",
                                                                                               LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on history.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IRedemptionHistoryOperations instance to use.
        /// </returns>
        public static IRedemptionHistoryOperations RedemptionHistoryOperations(CommerceContext context)
        {
            ValidateContext(context);

            IRedemptionHistoryOperations result = new RedemptionHistoryOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IRedemptionHistoryOperations>("MockHistoryOperations",
                                                                                                    LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on referrals.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IReferralOperations instance to use.
        /// </returns>
        public static IReferralOperations ReferralOperations(CommerceContext context)
        {
            ValidateContext(context);

            IReferralOperations result = new ReferralOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IReferralOperations>("MockReferralOperations",
                                                                                           LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Gets the object to use to perform operations on rewards.
        /// </summary>
        /// <param name="context">
        /// The context in which operations will be performed.
        /// </param>
        /// <returns>
        /// The IRewardOperations instance to use.
        /// </returns>
        public static IRewardOperations RewardOperations(CommerceContext context)
        {
            ValidateContext(context);

            IRewardOperations result = new RewardOperations();

            if (context.Config.DataStoreMockLevel != CommerceDataStoreMockLevel.None)
            {
                result = LateBinding.BuildObjectFromLateBoundAssembly<IRewardOperations>("MockRewardOperations",
                                                                                         LateBoundMocksAssemblyTypes);
            }

            result.Context = context;

            return result;
        }

        /// <summary>
        /// Validate that context is provided
        /// </summary>
        /// <param name="context">
        /// Context cannot be null
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        internal static void ValidateContext(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null");
            }
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