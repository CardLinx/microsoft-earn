//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
#define XXXDEBUG
using services.visa.com.realtime.realtimeservice.datacontracts.v6;
using Visa.Proxy;

namespace Lomo.Commerce.VisaClient
{
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Logging;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides invocation of Visa RTM service APIs.
    /// </summary>
    public class VisaInvoker : IVisaInvoker
    {
#if !XXXDEBUG
        private const string Logfilename = @"c:\temp\visalog.txt";
#endif
        /// <summary>
        ///     Create Card Enrollment Request
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public async Task<EnrollResponse> CreateEnrollment(EnrollRequest request)
        {
            EnrollResponse response;

            using (var client = VisaRtmClientManager.Instance.GetVisaRtmClient())
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    // Todo: Should remove this later
                    // Visa has issues to add the same card to multiple users. 
                    // Requests are logged so we can clean up the users using an external tool 
#if !XXXDEBUG
                    string logstring = string.Format("{0} CreateEnrollmentRequest {1} {2}\n",
                                                     DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                                     request.CardHolderDetails.ExternalUserId,
                                                     request.CardDetails[0].CardNumber.Substring(12));
                        // just the last four digits
                    File.AppendAllText(Logfilename, logstring);
#endif
                    response = await client.EnrollAsync(request).ConfigureAwait(false);

#if !XXXDEBUG
                    if (response.EnrollmentRecord != null && response.EnrollmentRecord.CardDetails.Length > 0)
                    {
                        logstring = string.Format("{0} CreateEnrollmentResponse {1} {2}\n",
                                                  DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                                  response.EnrollmentRecord.CardHolderDetails.ExternalUserId,
                                                  response.EnrollmentRecord.CardDetails[0].CardId);
                        File.AppendAllText(Logfilename, logstring);
                    }
#endif
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("Visa CreateEnrollment ",
                                               String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }
            return response;
        }


        /// <summary>
        ///     Remove a user request
        /// </summary>
        /// <param name="request">request for remove user</param>
        /// <param name="shallNotRun">Shall not run for normal cases</param>
        /// <returns>response for removing user</returns>
        public async Task<Unenroll_Response> RemoveUser(UnenrollRequest request, bool shallNotRun = true)
        {
            Unenroll_Response response;

            using (var client = VisaRtmClientManager.Instance.GetVisaRtmClient())
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    if (shallNotRun)
                    {
                        response = new Unenroll_Response();
                        response.Success = true;
                    }
                    else
                    {
#if !XXXDEBUG
                        string logstring = string.Format("{0} UnenrollRequest {1}\n",
                                                         DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                                         ((ExternalAuthenticationRecord) request.AuthenticationRecord)
                                                             .UserKey);
                        // just the last four digits
                        File.AppendAllText(Logfilename, logstring);
#endif
                        response = await client.UnenrollAsync(request).ConfigureAwait(false);
#if !XXXDEBUG
                        if (response.Success)
                        {
                            logstring = string.Format("{0} UnenrollResponse {1} {2}\n",
                                                      DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                                      ((ExternalAuthenticationRecord) request.AuthenticationRecord)
                                                          .UserKey,
                                                      response.Success);
                            File.AppendAllText(Logfilename, logstring);
                        }
#endif
                    }
                }
                finally
                {
                    sprocTimer.Stop();
                    if (PerformanceInformation != null)
                        PerformanceInformation.Add("Visa RemoveUser ",
                                               String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }
            return response;
        }

        //Todo: Remove the mock code once Visa provides the saveoffer api
        /// <summary>
        ///     Visa is supposed to implemented an api for register deal
        ///     it is not available now and all deals are registered manually offline
        ///     We are mocking the request now
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RegisterDealResponse> RegisterDeal(RegisterDealRequest request)
        {
#if !XXXDEBUG
            string logstring = string.Format("{0} RegisterDealRequest {1}\n",
                                             DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                             request.BingOfferDealId);
            // just the last four digits
            File.AppendAllText(Logfilename, logstring);
#endif
            RegisterDealResponse response;
            Stopwatch sprocTimer = Stopwatch.StartNew();
            try
            {
                response = await RegisterDealMock.RegisterDeal(request).ConfigureAwait(false);
            }
            finally
            {
                sprocTimer.Stop();
                PerformanceInformation.Add("Visa RegisterDeal ",
                                           String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
            }
#if !XXXDEBUG
            logstring = string.Format("{0} RegisterDealResponse {1}, {2}\n",
                                             DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                             request.BingOfferDealId, response.VisaDealId);
            // just the last four digits
            File.AppendAllText(Logfilename, logstring);
#endif
            return response;
        }


        /// <summary>
        ///     Add additional card to user
        /// </summary>
        /// <param name="request">userkey, community, cardnumber</param>
        /// <returns>result</returns>
        public async Task<SaveCard_Response> AddCard(SaveCardRequest request)
        {
            SaveCard_Response response;

            using (var client = VisaRtmClientManager.Instance.GetVisaRtmClient())
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    response = await client.SaveCardAsync(request).ConfigureAwait(false);
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("Visa SaveCard ",
                                               String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }
            return response;
        }

        /// <summary>
        ///     Remove card from user
        /// </summary>
        /// <param name="request">userkey, community, cardnumber</param>
        /// <returns>result</returns>
        public async Task<DeleteCardResponse> RemoveCard(DeleteCardRequest request)
        {
            DeleteCardResponse response;

            using (var client = VisaRtmClientManager.Instance.GetVisaRtmClient())
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    response = await client.DeleteCardAsync(request).ConfigureAwait(false);
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("Visa RemoveCard ",
                                               String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }
            return response;
        }

        /// <summary>
        /// Issue credit to user during a burn transaction in lieu of earn credit that user has in his account
        /// </summary>
        /// <param name="request"></param>
        /// <returns>result</returns>
        public async Task<SaveStatementCredit_Response> SaveStatementCreditAsync(SaveStatementCreditRequest request)
        {
            SaveStatementCredit_Response response;

            using (var client = VisaRtmClientManager.Instance.GetVisaRtmClient())
            {
                Stopwatch sprocTimer = Stopwatch.StartNew();
                try
                {
                    response = await client.SaveStatementCreditAsync(request).ConfigureAwait(false);
                }
                finally
                {
                    sprocTimer.Stop();
                    PerformanceInformation.Add("SaveStatementCreditAsync ",
                                               String.Format("{0} ms", sprocTimer.ElapsedMilliseconds));
                }
            }
            return response;
        }

        /// <summary>
        ///  Generate external user id from user
        /// </summary>
        /// <param name="user">user param</param>
        /// <returns>user id</returns>
        public string GenerateExternalUserId(User user)
        {
            return user.UserIdHexFormat;
        }

        /// <summary>
        ///     Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        public PerformanceInformation PerformanceInformation { get; set; }
    }
}