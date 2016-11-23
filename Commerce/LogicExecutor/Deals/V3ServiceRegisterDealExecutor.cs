//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Contains logic necessary to execute a V3 register deal request.
    /// </summary>
    public class V3ServiceRegisterDealExecutor
    {
        /// <summary>
        /// Initializes a new instance of the V3RegisterDealExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context describing the deal to register.
        /// </param>
        public V3ServiceRegisterDealExecutor(CommerceContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Executes the Register deal API invocation.
        /// </summary>
        public void Execute()
        {
            SharedDealLogic sharedDealLogic = new SharedDealLogic(Context, CommerceOperationsFactory.DealOperations(Context));
            sharedDealLogic.Execute();
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}