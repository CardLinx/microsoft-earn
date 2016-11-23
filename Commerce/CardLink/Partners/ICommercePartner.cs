//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.CardLink
{
    using System.Threading.Tasks;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Operation interface for all partners.
    /// </summary>
    public interface ICommercePartner
    {
        /// <summary>
        /// Adds the card in the context for the user in the context to this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        Task<ResultCode> AddCardAsync();

        /// <summary>
        /// Removes the card in the context for the user in the context from this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        Task<ResultCode> RemoveCardAsync();

        /// <summary>
        /// Registers the deal in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        Task<ResultCode> RegisterDealAsync();

        /// <summary>
        /// Claims the deal in the context for redemption with the card in the context with this partner.
        /// </summary>
        /// <returns>
        /// A task that will yield the result code for the operation.
        /// </returns>
        Task<ResultCode> ClaimDealAsync();

        /// <summary>
        /// Gets or sets the context of the operation.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}