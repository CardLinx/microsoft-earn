//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Interface through which the MasterCard service is invoked.
    /// </summary>
    public interface IMasterCardInvoker
    {
        /// <summary>
        /// Adds the described card to MasterCard.
        /// </summary>
        /// <param name="doEnrollmentRequest">
        /// Description of the card to add.
        /// </param>
        /// <returns>
        /// The response from MasterCard for the add card attempt.
        /// </returns>
        Task<DoEnrollmentResp> AddCard(doEnrollment doEnrollmentRequest);

        /// <summary>
        /// Removes the described card to MasterCard.
        /// </summary>
        /// <param name="doCustomerAccountUpdateRequest">
        /// Description of the card to remove.
        /// </param>
        /// <returns>
        /// The response from MasterCard for the remove card attempt.
        /// </returns>
        Task<CustomerAccountUpdateResp> RemoveCard(doCustomerAccountUpdate doCustomerAccountUpdateRequest);

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        PerformanceInformation PerformanceInformation { get; set; }
    }
}