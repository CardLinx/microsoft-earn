//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Provides invocation of First Data service APIs.
    /// </summary>
    public class FirstDataInvoker : IFirstDataInvoker
    {
        /// <summary>
        /// Adds the described card to FirstData.
        /// </summary>
        /// <param name="cardRegisterRequest">
        /// Description of the card to add.
        /// </param>
        /// <returns>
        /// The response from First Data for the add card attempt.
        /// </returns>
        public async Task<CardRegisterResponse> AddCard(CardRegisterRequest cardRegisterRequest)
        {
            CardRegisterResponse result;

            using (registrationserviceClient registrationClient = new registrationserviceClient("registrationserviceSoap12"))
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    CardRegisterResponse1 response1 = await registrationClient.CardRegisterAsync(cardRegisterRequest);
                    result = response1.CardRegisterResponse;
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("FirstData CardRegister (add card)",
                                             String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }

            return result;
        }

        /// <summary>
        /// Removes the described card from FirstData.
        /// </summary>
        /// <param name="cardRegisterRequest">
        /// Description of the card to remove.
        /// </param>
        /// <returns>
        /// The response from FirstData for the remove card attempt.
        /// </returns>
        public async Task<CardRegisterResponse> RemoveCard(CardRegisterRequest cardRegisterRequest)
        {
            CardRegisterResponse result;

            using (registrationserviceClient registrationClient = new registrationserviceClient("registrationserviceSoap12"))
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
//TODO: IF we decide to delete the card in First Data at all, re-enable the call to First Data.
//                      Since we know that this call can never succeed, let's not flood their logs with failure results and
//                      create more work for them and for us. Note the convoluted means of disabling this are necessary to
//                      satisfy syntactic requirements of the async/await pattern.
//
//TODO: If the above is done, add try/finally so that analytics info is added even for failures.
//
//TODO: If we decide not to delete the card in all partners, remove all relevant code and revert DeactivateCard to a synchronous op.
CardRegisterResponse1 response1 = new CardRegisterResponse1()
{
    CardRegisterResponse = new CardRegisterResponse()
    {
        respCode = FirstDataResponseCode.RemoveCardFieldParseError,
        respText = "No call to First Data was made. Cards are not currently deleted from First Data."
    }
};

                /*CardRegisterResponse1*/ if (response1 == null) response1 = await registrationClient.CardRegisterAsync(cardRegisterRequest);
                sprocTimer.Stop();
                PerformanceInformation.Add("FirstData CardRegister (remove card)",
                                         String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                result = response1.CardRegisterResponse;
            }

            return result;
        }

        /// <summary>
        /// Registers the described deal with FirstData.
        /// </summary>
        /// <param name="offerRegisterRequest">
        /// Description of the deal to register.
        /// </param>
        /// <returns>
        /// The response from First Data for the register deal attempt.
        /// </returns>
        /// <remarks>
        /// First Data uses the same API for registering and claiming a deal, varying only input parameters.
        /// </remarks>
        public async Task<OfferRegisterResponse> RegisterDeal(OfferRegisterRequest offerRegisterRequest)
        {
            OfferRegisterResponse result;

            using (registrationserviceClient registrationClient = new registrationserviceClient("registrationserviceSoap12"))
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    OfferRegisterResponse1 response1 = await registrationClient.OfferRegisterAsync(offerRegisterRequest);
                    result = response1.OfferRegisterResponse;
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("FirstData OfferRegister (register deal)",
                                             String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }
            
            return result;
        }

        /// <summary>
        /// Claims the specified deal for redemption with the specified card with First Data.
        /// </summary>
        /// <param name="offerRegisterRequest">
        /// Description of the deal to claim.
        /// </param>
        /// <returns>
        /// The response from First Data for the claim deal attempt.
        /// </returns>
        /// <remarks>
        /// First Data uses the same API for registering and claiming a deal, varying only input parameters.
        /// </remarks>
        public async Task<OfferRegisterResponse> ClaimDeal(OfferRegisterRequest offerRegisterRequest)
        {
            OfferRegisterResponse result;

            using (registrationserviceClient registrationClient = new registrationserviceClient("registrationserviceSoap12"))
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    OfferRegisterResponse1 response1 = await registrationClient.OfferRegisterAsync(offerRegisterRequest);
                    result = response1.OfferRegisterResponse;
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("FirstData OfferRegister (claim deal)",
                                             String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        public PerformanceInformation PerformanceInformation { get; set; }
    }
}