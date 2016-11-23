//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System.Collections.ObjectModel;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;

    /// <summary>
    /// Represents operations on Referral objects within the data store.
    /// </summary>
    public interface IReferralOperations
    {
        /// <summary>
        /// Adds the referral type in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddReferralType();

        /// <summary>
        /// Adds the referral in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        ResultCode AddReferral();

        /// <summary>
        /// Gets the referrals that resulted from the referrer specified in the context and the events associated with those
        /// referrals.
        /// </summary>
        /// <returns>
        /// A list of referral types and counts.
        /// </returns>
        Collection<ReferralCodeReportDataContract> RetrieveReferralCounts();

        /// <summary>
        /// Gets the unprocessed referral records belonging to the user in the context.
        /// </summary>
        /// <returns>
        /// The unprocessed referral records belonging to the user.
        /// </returns>
        Collection<RewardPayoutRecord> RetrieveUserUnprocessedReferrals();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}