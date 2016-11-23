//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Linq;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using System.Collections.Generic;

    /// <summary>
    /// Contains methods to perform shared business logic for authorization objects.
    /// </summary>
    public class SharedAuthorizationLogic
    {
        /// <summary>
        /// Initializes a new instance of the SharedAuthorizationLogic class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        /// <param name="authorizationOperations">
        /// The object to use to perform operations on authorizations.
        /// </param>
        public SharedAuthorizationLogic(CommerceContext context,
                                        IAuthorizationOperations authorizationOperations)
        {
            Context = context;
            AuthorizationOperations = authorizationOperations;
        }

        /// <summary>
        /// Adds the Authorization to the data store and logs accordingly.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddAuthorization()
        {
            ResultCode result;

            // Add the redemption event info to the data store.
            Context.Log.Verbose("Attempting to add the authorization to the data store.");
            result = AuthorizationOperations.AddAuthorization();
            Context.Log.Verbose("ResultCode after adding the authorization to the data store: {0}", result);
            
//TODO: Shouldn't this be in the CardLink layer?
            // If the authorization was successfully created, complete populating redeemed deal into fields.
            if (result == ResultCode.Created)
            {
                RedeemedDealInfo redeeemedDealInfo = (RedeemedDealInfo)Context[Key.RedeemedDealInfo];
                int redemptionAmt = redeeemedDealInfo.DiscountAmount;
                double actualSavings = (double)redemptionAmt / 100;
                redeeemedDealInfo.DiscountText = String.Format("${0:F2}", actualSavings);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on authorizations.
        /// </summary>
        private IAuthorizationOperations AuthorizationOperations { get; set; }
    }
}