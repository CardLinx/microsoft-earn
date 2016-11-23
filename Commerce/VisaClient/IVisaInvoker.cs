//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using services.visa.com.realtime.realtimeservice.datacontracts.v6;

namespace Lomo.Commerce.VisaClient
{
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using System.Threading.Tasks;

    /// <summary>
    ///     Interface through which the Visa service is invoked.
    /// </summary>
    public interface IVisaInvoker
    {
        /// <summary>
        ///     Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        PerformanceInformation PerformanceInformation { get; set; }

        /// <summary>
        ///     Adds the described user with specified card to Visa.
        /// </summary>
        /// <param name="request">
        ///     Description of the card to add.
        /// </param>
        /// <returns>
        ///     The response from Visa for the add card attempt.
        /// </returns>
        Task<EnrollResponse> CreateEnrollment(EnrollRequest request);

        /// <summary>
        ///     Add additional card to user
        /// </summary>
        /// <param name="request">userkey, community, cardnumber</param>
        /// <returns>result</returns>
        Task<SaveCard_Response> AddCard(SaveCardRequest request);

        /// <summary>
        ///     Remove card from user
        /// </summary>
        /// <param name="request">userkey, community, cardnumber</param>
        /// <returns>result</returns>
        Task<DeleteCardResponse> RemoveCard(DeleteCardRequest request);

        /// <summary>
        ///     Remove a user request
        /// </summary>
        /// <param name="request">request for remove user</param>
        /// <param name="shallNotRun">Shall not run for normal cases</param>
        /// <returns>response for removing user</returns>
        Task<Unenroll_Response> RemoveUser(UnenrollRequest request, bool shallNotRun = true);

        ///// <summary>
        ///// Registers the described deal with Visa.
        ///// </summary>
        ///// <param name="offerRegisterRequest">
        ///// Description of the deal to register.
        ///// </param>
        ///// <returns>
        ///// The response from Visa for the register deal attempt.
        ///// </returns>
        Task<RegisterDealResponse> RegisterDeal(RegisterDealRequest offerRegisterRequest);
        
        /// <summary>
        ///  Generate external user id from user 
        /// </summary>
        /// <param name="user">user param</param>
        /// <returns>user id</returns>
        string GenerateExternalUserId(User user);
        
        /// <summary>
        /// Issue credit to user during a burn transaction in lieu of earn credit that user has in his account
        /// </summary>
        /// <param name="request"></param>
        /// <returns>response from Visa indicating if credit was success</returns>
        Task<SaveStatementCredit_Response> SaveStatementCreditAsync(SaveStatementCreditRequest request);
    }
}