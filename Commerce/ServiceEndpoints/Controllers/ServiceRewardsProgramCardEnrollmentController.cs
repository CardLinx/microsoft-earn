//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Diagnostics;

    using Lomo.Authorization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Controller for enrolling/unenrolling a user's cards into a rewards program.
    /// </summary>
    public class ServiceRewardsProgramCardEnrollmentController : ApiController
    {
        readonly ICardOperations cardOperations;

        public ServiceRewardsProgramCardEnrollmentController()
            : this(new CardOperations())
        {
        }

        /// <summary>
        /// Constructor for testing purposes to inject dependencies.
        /// </summary>
        public ServiceRewardsProgramCardEnrollmentController(
            ICardOperations cardOperations)
        {
            if (cardOperations == null)
            {
                throw new ArgumentNullException("cardOperations");
            }

            this.cardOperations = cardOperations;
        }

        /// <summary>
        /// Enroll a user's card into a rewards program.
        /// </summary>
        /// <param name="enrollment">
        /// The enrollment details.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "ValidateEnrollment method does the validation.")]
        [HttpPut]
        [MutualSslAuth("Ingestion")]
        public HttpResponseMessage Put([FromBody] RewardsProgramCardEnrollment enrollment)
        {
            Stopwatch callTimer = Stopwatch.StartNew();

            // Build a context object to pass down the pipeline.
            CommerceContext context = CommerceContext.BuildSynchronousRestContext("EnrollCardsInRewardsPrograms", Request,
                                                                                  new GetEarnBurnTransactionHistoryResponse(), callTimer);

            this.cardOperations.Context = context;
            RewardPrograms rewardPrograms;
            CardBrand[] cardBrands;
            Guid globalUserId;
            ValidateEnrollment(context, enrollment,out globalUserId, out cardBrands, out rewardPrograms);

            try
            {
                ResultCode result = this.cardOperations.EnrollCardsInRewardPrograms(
                    globalUserId,
                    rewardPrograms,
                    cardBrands);

                if (result == ResultCode.Success)
                {
                    return this.Request.CreateResponse(HttpStatusCode.OK);
                }

                if (result == ResultCode.UnregisteredUser)
                {
                    return this.Request.CreateResponse(HttpStatusCode.NotFound, "The user was not found.");
                }

                if (result == ResultCode.AggregateError)
                {
                    return this.Request.CreateResponse(
                        HttpStatusCode.Conflict,
                        "One or more cards could not be enrolled in the requested programs. The card might be enrolled in the same reward program by a different user account.");
                }

                return this.Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                context.Log.Warning(
                    "Enrolling the user's cards in the reward program failed. Error = '{0}'",
                    ex.ToString());

                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The response object cannot be disposed here.")]
        void ValidateEnrollment(
            CommerceContext context,
            RewardsProgramCardEnrollment enrollment,
            out Guid globalUserId,
            out CardBrand[] cardBrands,
            out RewardPrograms rewardPrograms)
        {
            cardBrands = null;
            rewardPrograms = RewardPrograms.None;
            globalUserId = Guid.Empty;

            string errorMessage = null;
            if (enrollment == null)
            {
                errorMessage = "The enrollment request body is invalid.";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(enrollment.UserId))
                {
                    errorMessage = "The user id should be specified.";
                }
                else if (!Guid.TryParse(enrollment.UserId, out globalUserId))
                {
                    errorMessage = "The user id is not valid.";
                }
                else if (enrollment.CardBrands == null || enrollment.CardBrands.Length == 0)
                {
                    errorMessage = "No card brands were specified.";
                }
                else if (enrollment.RewardPrograms == null || enrollment.RewardPrograms.Length == 0)
                {
                    errorMessage = "No reward programs were specified.";
                }
                else
                {
                    cardBrands = new CardBrand[enrollment.CardBrands.Length];

                    for (int i = 0; i < enrollment.CardBrands.Length; i++)
                    {
                        var cardBrand = enrollment.CardBrands[i];
                        CardBrand brand;
                        if (Enum.TryParse(cardBrand, true, out brand))
                        {
                            cardBrands[i] = brand;
                        }
                        else
                        {
                            errorMessage = string.Format("The card brand '{0}' is not valid.", cardBrand);
                            break;
                        }
                    }

                    if (errorMessage == null)
                    {
                        rewardPrograms = RewardPrograms.None;

                        foreach (var rewardProgram in enrollment.RewardPrograms)
                        {
                            RewardPrograms program;
                            if (Enum.TryParse(rewardProgram, true, out program))
                            {
                                rewardPrograms |= program;
                            }
                            else
                            {
                                errorMessage = string.Format("The reward program '{0}' is not valid.", rewardProgram);
                                break;
                            }
                        }
                    }
                }
            }

            if (errorMessage != null)
            {
                context.Log.Information("The enrollment request body had errors: {0}", errorMessage);

                throw new HttpResponseException(
                    this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage));
            }
        }
    }
}