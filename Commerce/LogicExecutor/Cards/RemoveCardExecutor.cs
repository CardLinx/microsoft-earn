//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Service;
    using System.Threading.Tasks;
    using Lomo.Commerce.DataContracts;

    /// <summary>
    /// Contains logic necessary to execute an remove card request.
    /// </summary>
    public class RemoveCardExecutor
    {
        /// <summary>
        /// Initializes a new instance of the RemoveCardExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public RemoveCardExecutor(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Context = context;
        }

        /// <summary>
        /// Executes the Remove card invocation.
        /// </summary>
        public async Task Execute()
        {
            try
            {
                SharedUserLogic sharedUserLogic = new SharedUserLogic(Context,
                                                                      CommerceOperationsFactory.UserOperations(Context));
                SharedCardLogic sharedCardLogic = new SharedCardLogic(Context,
                                                                      CommerceOperationsFactory.CardOperations(Context));
                RemoveCardConcluder removeCardConcluder = new RemoveCardConcluder(Context);

                User user = sharedUserLogic.RetrieveUser();
                Context[Key.User] = user;
                if (user != null)
                {
                    Card card = sharedCardLogic.RetrieveCard();
                    Context[Key.Card] = card;
                    if (card != null)
                    {
                        if (card.GlobalUserId == user.GlobalId)
                        {
                            Context.Log.Verbose("Attempting to remove the card from all current partners.");
                            RemoveCardInvoker removeCardInvoker = new RemoveCardInvoker(Context);
                            await removeCardInvoker.Invoke();
                        }
                        else
                        {
                            removeCardConcluder.Conclude(ResultCode.CardDoesNotBelongToUser);
                        }
                    }
                    else
                    {
                        removeCardConcluder.Conclude(ResultCode.UnregisteredCard);
                    }
                }
                else
                {
                    removeCardConcluder.Conclude(ResultCode.UnexpectedUnregisteredUser);
                }
            }
            catch (Exception ex)
            {
                ((ResultSummary)Context[Key.ResultSummary]).SetResultCode(ResultCode.UnknownError);
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }
    }
}