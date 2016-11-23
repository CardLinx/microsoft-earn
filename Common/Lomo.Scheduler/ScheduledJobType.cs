//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    /// <summary>
    /// Represents the type of job.
    /// </summary>
    public enum ScheduledJobType
    {
        /// <summary>
        /// Job to apply a reward for from a referral.
        /// </summary>
        ApplyReferralReward,

        /// <summary>
        /// Job to apply a reward for from a redemption.
        /// </summary>
        ApplyRedemptionReward,

        /// <summary>
        /// Job to claim all active discounts for a new card.
        /// </summary>
        ClaimDiscountsForNewCard,

        /// <summary>
        /// Job to claim the discounts for all active cards.
        /// </summary>
        ClaimDiscountForExistingCards,

        /// <summary>
        /// Job to process FDC Ack file
        /// </summary>
        ProcessAcknowledgment,

        /// <summary>
        /// Job to process FDC Extract file
        /// </summary>
        ProcessExtract,

        /// <summary>
        /// Job to build FDC Pts file
        /// </summary>
        ProcessPts,

        /// <summary>
        /// A job tht just logs the execution.
        /// Meant for health check
        /// </summary>
        PingJob,

        /// <summary>
        /// Job to upload merchant registration records to blob store
        /// </summary>
        AmexOfferRegistration,

        /// <summary>
        /// Job to upload download and syn offer registration file with Amex Sftp server
        /// </summary>
        AmexOfferRegistrationFileSync,

        /// <summary>
        /// Job to set a particular discount as Active in Deal Server
        /// </summary>
        DiscountActivationJob,

        /// <summary>
        /// Job to download and process Amex TLog File
        /// </summary>
        ProcessAmexTransactionLog,

        /// <summary>
        /// Job to upload Statement Credit File to Amex
        /// </summary>
        ProcessAmexStatementCredit,

        /// <summary>
        /// Job to build MasterCard Filtering file.
        /// </summary>
        ProcessMasterCardFiltering,

        /// <summary>
        /// Job to parse MasterCard Clearing file.
        /// </summary>
        ProcessMasterCardClearing,

        /// <summary>
        /// Job to build MasterCard Rebate file.
        /// </summary>
        ProcessMasterCardRebate,

        /// <summary>
        /// Job to parse MasterCard Rebate Confirmation file.
        /// </summary>
        ProcessMasterCardRebateConfirmation,

        /// <summary>
        /// Job to create a report to be sent to RN consisting of earn transactions at RN merchants.
        /// </summary>
        ProcessRewardsNetworkReport,

        /// <summary>
        /// Job to process Visa rebater transactions.
        /// </summary>
        ProcessVisaRebate
    }
}