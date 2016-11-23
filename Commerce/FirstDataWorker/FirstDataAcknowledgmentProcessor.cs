//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.FirstDataClient;
    using Lomo.Commerce.Worker.Actions;
    using System.Threading.Tasks;
    using Lomo.Commerce.WorkerCommon;
    using Lomo.Commerce.DataModels;

    public class FirstDataAcknowledgmentProcessor : ISettlementFileProcessor
    {
        public FirstDataAcknowledgmentProcessor()
        {
            Context = new CommerceContext(string.Empty, CommerceWorkerConfig.Instance);
            RedeemedDealOperations = CommerceOperationsFactory.RedeemedDealOperations(Context);
            RewardOperations = CommerceOperationsFactory.RewardOperations(Context);
        }
        /// <summary>
        /// Processes the Acknowledgment file.
        /// </summary>
        public async Task Process()
        {
            await Task.Factory.StartNew(ProcessAcknowledgement).ConfigureAwait(false);
        }

        private void ProcessAcknowledgement()
        {
            // Deserialize ack file into an ack object.
            AcknowledgmentParser acknowledgmentParser = new AcknowledgmentParser(Context.Log);
            Acknowledgment acknowledgment = acknowledgmentParser.Parse(AcknowledgmentFileName, AcknowledgmentFileStream);

            Collection<int> successfullyRedeemedReferenceNumbers = new Collection<int>();
            Collection<int> rejectedByPartnerReferenceNumbers = new Collection<int>();
            Collection<int> successfullyGrantedRewards = new Collection<int>();
            Collection<int> rejectedRewards = new Collection<int>();
            if (acknowledgment != null)
            {
                // if we have general ack records to process, get the first one
                // We should ideally have only one B record, and we will log warning in parsing code otherwise.
                if (acknowledgment.GeneralAcknowledgments.Count > 0)
                {
                    GeneralAcknowledgment generalAcknowledgment = acknowledgment.GeneralAcknowledgments[0];
                    if (generalAcknowledgment.AcknowledgementCode != AcknowledgmentConstants.SuccessfulSubmissionAckCode)
                    {
                        // log failure and return
                        Context.Log.Warning(
                            "The PTS file submission was rejected. Details: \r\n:" +
                            "Record Seq Number : {0} \r\n" +
                            "Received Acknowledgment Code : {1} \r\n" +
                            "Sumission Id : {2} \r\n",
                            (int)ResultCode.SubmissionRejected,
                            generalAcknowledgment.RecordSequenceNumber,
                            generalAcknowledgment.AcknowledgementCode,
                            generalAcknowledgment.SubmissionId);

                        return;
                    }
                }


                // process each Detail Ack Record.
                foreach (DetailAcknowledgment detailAcknowledgment in acknowledgment.DetailAcknowledgments)
                {
                    if (detailAcknowledgment != null)
                    {
                        // No overflow because reference number will never be more than int range. 
                        int referenceNumber = Convert.ToInt32(detailAcknowledgment.ReferenceNumber);

                        // add all the successfull 
                        if (detailAcknowledgment.AcknowledgementCode == AcknowledgmentConstants.SuccessfulRedemptionAckCode)
                        {
                            if (String.Equals(detailAcknowledgment.MerchantDescriptor,
                                              ReferredRedemptionRewardsMerchantDescriptor, StringComparison.OrdinalIgnoreCase) == false)
                            {
                                successfullyRedeemedReferenceNumbers.Add(referenceNumber);
                            }
                            else
                            {
                                successfullyGrantedRewards.Add(referenceNumber);
                            }
                        }
                        else
                        {
                            Context.Log.Warning(
                                "Deal with reference number {0} was not accepted by the partner. " +
                                "We expected AcknowledgmentCode of {1} but we got {2}",
                                (int)ResultCode.RedeemedDealRejectedByPartner,
                                referenceNumber,
                                AcknowledgmentConstants.SuccessfulRedemptionAckCode,
                                detailAcknowledgment.AcknowledgementCode);

                            if (String.Equals(detailAcknowledgment.MerchantDescriptor,
                                              ReferredRedemptionRewardsMerchantDescriptor, StringComparison.OrdinalIgnoreCase) == false)
                            {
                                rejectedByPartnerReferenceNumbers.Add(referenceNumber);
                            }
                            else
                            {
                                rejectedRewards.Add(referenceNumber);
                            }
                        }
                    }
                }
            }

            if (successfullyRedeemedReferenceNumbers.Count > 0)
            {
                UpdateRedeemedDeals(successfullyRedeemedReferenceNumbers, CreditStatus.CreditGranted);
                WorkerActions.UpdateOutstandingReferredRedemptionRewards(successfullyGrantedRewards, RewardPayoutStatus.Paid, RewardOperations, Context);
            }

            if (rejectedByPartnerReferenceNumbers.Count > 0)
            {
                UpdateRedeemedDeals(rejectedByPartnerReferenceNumbers, CreditStatus.RejectedByPartner);
                WorkerActions.UpdateOutstandingReferredRedemptionRewards(rejectedRewards, RewardPayoutStatus.Rescinded, RewardOperations, Context);
            }
        }

        /// <summary>
        /// Updates the redeemed deals to the credit status specified, if the credit status
        /// is valid.
        /// </summary>
        /// <param name="referenceNumbers">
        /// The list of reference numbers whose redeemed deals to update.
        /// </param>
        /// <param name="creditStatus">
        /// The credit status to which to set the redeemed deals.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Parameter creditStatus contains an invalid CreditStatus for this operation.
        /// </exception>
//TODO: We need to move this to someplace else so that PtsBuilder can also use it.
        internal void UpdateRedeemedDeals(Collection<int> referenceNumbers,
                                                 CreditStatus creditStatus)
        {
            // Ensure specified credit status is valid for this operation.
            if (creditStatus != CreditStatus.CreditGranted && creditStatus != CreditStatus.RejectedByPartner)
            {
                throw new InvalidOperationException("Parameter creditStatus contains an invalid CreditStatus for this operation.");
            }

            // Update the credit status for the specified list of merchant records.
            WorkerActions.Instance.UpdateDealStatus(referenceNumbers, creditStatus, RedeemedDealOperations, Context, ThreadId);
        }

        /// <summary>
        /// Gets or sets data Stream for Acknowledgment file contents
        /// </summary>
        public Stream AcknowledgmentFileStream { get; set; }

        /// <summary>
        /// Gets or sets Acknowledgment file name
        /// </summary>
        public string AcknowledgmentFileName { get; set; }

        /// <summary>
        /// The ID of the task thread in which this object is operating.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the Context object to use for this operation.
        /// </summary>
        private CommerceContext Context { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for RedeemedDeal operations.
        /// </summary>
        private IRedeemedDealOperations RedeemedDealOperations { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for Reward operations.
        /// </summary>
        private IRewardOperations RewardOperations { get; set; }

        /// <summary>
        /// The merchant descriptor used for referred redemption rewards.
        /// </summary>
        private const string ReferredRedemptionRewardsMerchantDescriptor = "BING OFFERS-REFER FRI";
    }
}