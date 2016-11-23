//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Contains logic necessary to execute an add referral type request.
    /// </summary>
    public class AddReferralTypeExecutor
    {
        /// <summary>
        /// Initializes a new instance of the AddReferralTypeExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context describing the referral type to register.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public AddReferralTypeExecutor(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Context = context;
            ReferralOperations = CommerceOperationsFactory.ReferralOperations(Context);
        }

        /// <summary>
        /// Executes the add referral type API invocation.
        /// </summary>
        public virtual void Execute()
        {
            SharedUserLogic sharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];

            if (sharedUserLogic.RetrieveUser() != null)
            {
                resultSummary.SetResultCode(CoreExecute());
            }
            else
            {
                resultSummary.SetResultCode(ResultCode.UnexpectedUnregisteredUser);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Executes core logic for the add referral type API invocation.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        protected ResultCode CoreExecute()
        {
            ResultCode result = BuildReferralType();
            if (result == ResultCode.Success)
            {
                result = AddReferralType();
                if (result == ResultCode.Created || result == ResultCode.Success)
                {
                    ((AddReferralTypeResponse)Context[Key.Response]).ReferralCode = ReferralType.Code;
                }
            }

            return result;
        }

        /// <summary>
        /// Build the ReferralType from the ReferralTypeDataContract.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        protected virtual ResultCode BuildReferralType()
        {
            ResultCode result = ResultCode.Success;

            string passedRewardRecipient = ((int)Context[Key.RewardRecipient]).ToString();
            string rewardRecipient = ((RewardRecipient)Context[Key.RewardRecipient]).ToString();
            if (passedRewardRecipient != rewardRecipient)
            {
                ReferralEventRewardDataContract signupReward = new ReferralEventRewardDataContract
                {
                    ReferralEvent = ReferralEvent.Signup.ToString(),
                    RewardId = CommerceServiceConfig.Instance.UserSignupReferralRewardId,
                    PerUserLimit = 0 // No limit
                };
                List<ReferralEventRewardDataContract> referalEventRewards = new List<ReferralEventRewardDataContract>
                {
                    signupReward
                };
                ReferralTypeDataContract referralTypeDataContract = new ReferralTypeDataContract
                {
                    ReferralVector =  ReferralVector.Unknown.ToString(),
                    RewardRecipient = rewardRecipient,
                    ReferralEventRewards = referalEventRewards
                };
                ReferralType referralType = new ReferralType()
                {
                    ReferrerId = (Guid)Context[Key.GlobalUserId],
                    ReferrerType = ReferrerType.User
                };

                result = BuildReferralType(referralTypeDataContract, referralType);
            }
            else
            {
                result = ResultCode.InvalidParameter;
            }

            return result;
        }

        /// <summary>
        /// Populate the specified ReferralType from the specified ReferralTypeDataContract.
        /// </summary>
        /// <param name="referralTypeDataContract">
        /// The ReferralTypeDataContract to use to populate the ReferralType.
        /// </param>
        /// <param name="referralType">
        /// The ReferralType to populate.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter referralTypeDataContract cannot be null.
        /// -OR-
        /// * Parameter referralType cannot be null.
        /// </exception>
        protected internal ResultCode BuildReferralType(ReferralTypeDataContract referralTypeDataContract,
                                                        ReferralType referralType)
        {
            if (referralTypeDataContract == null)
            {
                throw new ArgumentNullException("referralTypeDataContract", "Parameter referralTypeDataContract cannot be null.");
            }

            if (referralType == null)
            {
                throw new ArgumentNullException("referralType", "Parameter referralType cannot be null.");
            }

            ResultCode result = ResultCode.Success;

            ReferralVector referralVector = ReferralVector.Twitter;
            RewardRecipient rewardRecipient = RewardRecipient.Referrer;
            ReferralEvent referralEvent = ReferralEvent.Signup;
            bool stepResult = true;

            stepResult = Enum.TryParse<ReferralVector>(referralTypeDataContract.ReferralVector, true, out referralVector);
            if (stepResult == true)
            {
                referralType.ReferralVector = referralVector;

                stepResult = Enum.TryParse<RewardRecipient>(referralTypeDataContract.RewardRecipient, true, out rewardRecipient);
            }
            if (stepResult == true)
            {
                referralType.RewardRecipient = rewardRecipient;

                stepResult = referralTypeDataContract.ReferralEventRewards != null;
            }
            if (stepResult == true)
            {
                Dictionary<ReferralEvent, bool> registeredEvents = new Dictionary<ReferralEvent, bool>();

                foreach (ReferralEventRewardDataContract referralEventRewardDataContract
                                                                                in referralTypeDataContract.ReferralEventRewards)
                {
                    ReferralEventReward referralEventReward = new ReferralEventReward();

                    stepResult = Enum.TryParse<ReferralEvent>(referralEventRewardDataContract.ReferralEvent, true,
                                                              out referralEvent);
                    if (stepResult == true)
                    {
                        if (registeredEvents.ContainsKey(referralEvent) == false)
                        {
                            registeredEvents[referralEvent] = true;
                        }
                        else
                        {
                            stepResult = false;
                        }
                    }
                    if (stepResult == true)
                    {
                        referralEventReward.ReferralEvent = referralEvent;

                        stepResult = referralEventRewardDataContract.RewardId != Guid.Empty;
                    }
                    if (stepResult == true)
                    {
                        referralEventReward.RewardId = referralEventRewardDataContract.RewardId;

                        stepResult = referralEventRewardDataContract.PerUserLimit >= 0;
                    }
                    if (stepResult == true)
                    {
                        referralEventReward.PerUserLimit = referralEventRewardDataContract.PerUserLimit;

                        referralType.ReferralEventRewards.Add(referralEventReward);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (stepResult == true)
            {
                stepResult = referralType.ReferralEventRewards.Count > 0;
            }
            if (stepResult == true)
            {
                Context[Key.ReferralType] = referralType;
                ReferralType = referralType;
            }
            else
            {
                result = ResultCode.InvalidParameter;
            }

            return result;
        }

        /// <summary>
        /// Adds the referral type in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        protected ResultCode AddReferralType()
        {
            ResultCode result = ResultCode.Created;

            Context.Log.Verbose("Adding user referral type to the data store.");
            result = ReferralOperations.AddReferralType();
            if (result == ResultCode.Created)
            {
                Context.Log.Verbose("User referral type added to the data store.");
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the ReferralType being added.
        /// </summary>
        protected ReferralType ReferralType { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on referrals.
        /// </summary>
        protected IReferralOperations ReferralOperations { get; set; }
    }
}