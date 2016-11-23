//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;
    using Lomo.Scheduler;

    /// <summary>
    /// Contains logic necessary to execute an add referral request.
    /// </summary>
    public class AddReferralExecutor
    {
        /// <summary>
        /// Initializes a new instance of the AddReferralExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context describing the referral type to register.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        public AddReferralExecutor(CommerceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            Context = context;
            ReferralOperations = CommerceOperationsFactory.ReferralOperations(Context);
        }

        /// <summary>
        /// Executes the add referral API invocation.
        /// </summary>
        public void Execute()
        {
            SharedReferralLogic sharedReferralLogic = new SharedReferralLogic(Context, ReferralOperations);
            ResultSummary resultSummary = (ResultSummary)Context[Key.ResultSummary];

            ResultCode resultCode = ValidateParameters();
            if (resultCode == ResultCode.Success)
            {
                resultSummary.SetResultCode(sharedReferralLogic.AddReferral(UserIdString));
            }
            else
            {
                resultSummary.SetResultCode(resultCode);
            }
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Validates parameters within the context.
        /// </summary>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        private ResultCode ValidateParameters()
        {
            ResultCode result = ResultCode.Success;

            ReferralDataContract referralDataContract = Context[Key.ReferralDataContract] as ReferralDataContract;
            bool stepResult = referralDataContract != null;
            if (stepResult == true)
            {
                ReferralTypeCode = referralDataContract.ReferralTypeCode;
                Context[Key.ReferralTypeCode] = ReferralTypeCode;

                stepResult = String.IsNullOrWhiteSpace(ReferralTypeCode) == false;
            }
            if (stepResult == true)
            {
                UserIdString = referralDataContract.UserId;
                Context[Key.ReferredUserId] = UserIdString;

                stepResult = String.IsNullOrWhiteSpace(UserIdString) == false;
            }
            if (stepResult == true)
            {
                ReferralEvent referralEvent = (ReferralEvent)referralDataContract.ReferralEvent; 
                Context[Key.ReferralEvent] = referralEvent;
                ReferralEventString = referralEvent.ToString();
            }
            else
            {
                result = ResultCode.InvalidParameter;
            }
            
            return result;
        }

        /// <summary>
        /// Gets or sets the ID of the user being referred.
        /// </summary>
        private string UserIdString { get; set; }

        /// <summary>
        /// Gets or sets the referral type code describing the referral.
        /// </summary>
        private string ReferralTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the event that resulted in a referral.
        /// </summary>
        private string ReferralEventString { get; set; }

        /// <summary>
        /// Gets or sets the object to use to perform operations on referrals.
        /// </summary>
        private IReferralOperations ReferralOperations { get; set; }
    }
}