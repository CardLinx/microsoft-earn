//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Interface through which the FirstData service is invoked.
    /// </summary>
    public interface IFirstDataInvoker
    {
        /// <summary>
        /// Adds the described card to FirstData.
        /// </summary>
        /// <param name="cardRegisterRequest">
        /// Description of the card to add.
        /// </param>
        /// <returns>
        /// The response from FirstData for the add card attempt.
        /// </returns>
        Task<CardRegisterResponse> AddCard(CardRegisterRequest cardRegisterRequest);

        /// <summary>
        /// Removes the described card to FirstData.
        /// </summary>
        /// <param name="cardRegisterRequest">
        /// Description of the card to remove.
        /// </param>
        /// <returns>
        /// The response from FirstData for the remove card attempt.
        /// </returns>
        Task<CardRegisterResponse> RemoveCard(CardRegisterRequest cardRegisterRequest);

        /// <summary>
        /// Registers the described deal with FirstData.
        /// </summary>
        /// <param name="offerRegisterRequest">
        /// Description of the deal to register.
        /// </param>
        /// <returns>
        /// The response from FirstData for the register deal attempt.
        /// </returns>
        Task<OfferRegisterResponse> RegisterDeal(OfferRegisterRequest offerRegisterRequest);

        /// <summary>
        /// Claims the specified deal for redemption with the specified card with First Data.
        /// </summary>
        /// <param name="offerRegisterRequest">
        /// Description of the deal to claim.
        /// </param>
        /// <returns>
        /// The response from FirstData for the claim deal attempt.
        /// </returns>
        Task<OfferRegisterResponse> ClaimDeal(OfferRegisterRequest offerRegisterRequest);

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        PerformanceInformation PerformanceInformation { get; set; }
    }
}