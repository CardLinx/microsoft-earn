//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Runtime.Serialization;

namespace Earn.DataContract.Commerce
{
    [DataContract]
    public class UserReferral
    {
        [DataMember(Name = "payee_id")]
        public Guid PayeeId { get; set; }

        [DataMember(Name = "agent_id")]
        public string AgentId { get; set; }

        [DataMember(Name = "reward_reason_id")]
        public int RewardReasonId { get; set; }

        [DataMember(Name = "reward_reason")]
        public string RewardReason { get; set; }

        [DataMember(Name = "reward_payout_status_id")]
        public int RewardPayoutStatusId { get; set; }

        [DataMember(Name = "reward_payout_status")]
        public string RewardPayoutStatus { get; set; }

        [DataMember(Name = "explanation")]
        public string Explanation { get; set; }

        [DataMember(Name = "payout_scheduled_date_utc")]
        public DateTime PayoutScheduledDateUtc { get; set; }

        [DataMember(Name = "payout_finalized_date_utc")]
        public DateTime? PayoutFinalizedDateUtc { get; set; }

        [DataMember(Name = "amount")]
        public double Amount { get; set; }
    }
}