//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardClient
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Provides invocation of MasterCard service APIs.
    /// </summary>
    public class MasterCardInvoker : IMasterCardInvoker
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
        public async Task<DoEnrollmentResp> AddCard(doEnrollment doEnrollmentRequest)
        {
            DoEnrollmentResp result = null;

            using (masterCardMRSServiceClient registrationClient = new masterCardMRSServiceClient("mastercardregistrationserviceSoap11"))
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    doEnrollmentResponse1 response1 = await registrationClient.doEnrollmentAsync(doEnrollmentRequest).ConfigureAwait(false);
                    result = response1.doEnrollmentResponse.doEnrollmentResult;
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("MasterCard DoEnrollment (add card)",
                                               String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }

            return result;
        }

        /// <summary>
        /// Removes the described card from MasterCard.
        /// </summary>
        /// <param name="doCustomerAccountUpdateRequest">
        /// Description of the card to remove.
        /// </param>
        /// <returns>
        /// The response from MasterCard for the remove card attempt.
        /// </returns>
        public async Task<CustomerAccountUpdateResp> RemoveCard(doCustomerAccountUpdate doCustomerAccountUpdateRequest)
        {
//TODO: IF we decide to delete the card in MasterCard at all, re-enable the call to MasterCard.
//                      Since we know that this call can never succeed, let's not flood their logs with failure results and
//                      create more work for them and for us. Note the convoluted means of disabling this are necessary to
//                      satisfy syntactic requirements of the async/await pattern.
//
//TODO: If we decide not to delete the card in all partners, remove all relevant code and revert DeactivateCard to a synchronous op.
            doCustomerAccountUpdateResponse1 response = new doCustomerAccountUpdateResponse1
            {
                doCustomerAccountUpdateResponse = new doCustomerAccountUpdateResponse
                {
                    doCustomerAccountUpdateResult = new CustomerAccountUpdateResp
                    {
                        returnCode = MasterCardResponseCode.InvalidCard,
                        returnMsg = "No call to MasterCard was made. Cards are not currently deleted from MasterCard."
                    }
                }
            };

            await Task.Factory.StartNew(() => {});
            return response.doCustomerAccountUpdateResponse.doCustomerAccountUpdateResult;

            //CustomerAccountUpdateResp result = null;

            //using (masterCardMRSServiceClient registrationClient = new masterCardMRSServiceClient("mastercardregistrationserviceSoap11"))
            //{
            //    Stopwatch sprocTimer = Stopwatch.StartNew();
            //    try
            //    {
            //        doCustomerAccountUpdateResponse1 response1 = await registrationClient.doCustomerAccountUpdateAsync(doCustomerAccountUpdateRequest);
            //        result = response1.doCustomerAccountUpdateResponse.doCustomerAccountUpdateResult;
            //    }
            //    finally
            //    {
            //        sprocTimer.Stop();
            //        PerformanceInformation.Add("MasterCard DoCustomerAccountUpdateRequest (remove card)",
            //                                   String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
            //    }
            //}

            //return result;
        }

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        public PerformanceInformation PerformanceInformation { get; set; }
    }
}