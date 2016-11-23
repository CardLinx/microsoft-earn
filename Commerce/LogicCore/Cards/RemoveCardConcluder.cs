//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to conclude the execution of a remove card request.
    /// </summary>
    public class RemoveCardConcluder
    {
        /// <summary>
        /// Initializes a new instance of the RemoveCardConcluder class.
        /// </summary>
        /// <param name="context">
        /// The context of the current API call.
        /// </param>
        public RemoveCardConcluder(CommerceContext context)
        {
            Context = context;
            CardOperations = CommerceOperationsFactory.CardOperations(Context);
        }

        /// <summary>
        /// Concludes execution of the Remove card call after previous work has been completed.
        /// </summary>
        /// <param name="resultCode">
        /// The ResultCode to set within the call response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public void Conclude(ResultCode resultCode)
        {
            try
            {
                // Set the Response ResultCode as needed.
                Context.Log.Verbose("ResultCode when Conclude process begins: {0}.", resultCode);

                // If process succeeded, update internal data storage.
                if (resultCode == ResultCode.Success)
                {
                    // Remove the card.
                    resultCode = RemoveCard();
                }

                ((RemoveCardResponse)Context[Key.Response]).ResultSummary.SetResultCode(resultCode);
                RestResponder.BuildAsynchronousResponse(Context);
            }
            catch (Exception ex)
            {
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Gets or sets the context of the current API call.
        /// </summary>
        CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for Card operations.
        /// </summary>
        internal ICardOperations CardOperations { get; set; }

        /// <summary>
        /// Removes the specified Card from the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        private ResultCode RemoveCard()
        {
            ResultCode result;

            Context.Log.Verbose("Attempting to remove the card from the data store.");
            result = CardOperations.RemoveCardFromRewardPrograms();
            Context.Log.Verbose("ResultCode after removing the card from the data store: {0}", result);

            return result;
        }
    }
}