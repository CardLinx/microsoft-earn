//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    public interface IAmexInvoker
    {
        /// <summary>
        /// Adds the described card to Amex.
        /// </summary>
        /// <param name="amexCardSyncRequest">
        /// Description of the card to add.
        /// </param>
        /// <returns>
        /// The response from Amex for the add card attempt.
        /// </returns>
        Task<AmexCardResponse> AddCardAsync(AmexCardSyncRequest amexCardSyncRequest);

        /// <summary>
        /// Adds the described card to Amex.
        /// </summary>
        /// <param name="amexCardUnSyncRequest">
        /// Description of the card to Remove.
        /// </param>
        /// <returns>
        /// The response from Amex for the remove card attempt.
        /// </returns>
        Task<AmexCardResponse> RemoveCardAsync(AmexCardUnSyncRequest amexCardUnSyncRequest);        

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        PerformanceInformation PerformanceInformation { get; set; }

        /// <summary>
        /// Gets or sets the object through which logs can be added and obtained.
        /// </summary>
        CommerceLog CommerceLog { get; set; }
    }
}