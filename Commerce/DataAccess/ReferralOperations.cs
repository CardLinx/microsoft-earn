//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on Referral objects within the data store.
    /// </summary>
    public class ReferralOperations : CommerceOperations, IReferralOperations
    {
        /// <summary>
        /// Adds the referral type in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddReferralType()
        {
            ResultCode result = ResultCode.Created;

            ReferralType referralType = (ReferralType)Context[Key.ReferralType];
            using (DataTable eventRewardsTable = new DataTable("EventRewards"))
            {
                // Build the EventRewards table parameter.
                eventRewardsTable.Locale = CultureInfo.InvariantCulture;
                eventRewardsTable.Columns.Add("Id", typeof(Guid));
                eventRewardsTable.Columns.Add("ReferralEventId", typeof(Int32));
                eventRewardsTable.Columns.Add("RewardId", typeof(Guid));
                eventRewardsTable.Columns.Add("PerUserLimit", typeof(Int32));
                foreach (ReferralEventReward referralEventReward in referralType.ReferralEventRewards)
                {
                    eventRewardsTable.Rows.Add(referralEventReward.Id, referralEventReward.ReferralEvent,
                                               referralEventReward.RewardId, referralEventReward.PerUserLimit);
                }

                // Add the referral type to the data layer.
                SqlProcedure("MergeReferralType",
                             new Dictionary<string, object>
                             {
                                 { "@id", referralType.Id },
                                 { "@referrerId", referralType.ReferrerId },
                                 { "@referrerTypeId", referralType.ReferrerType },
                                 { "@referralVectorId", referralType.ReferralVector },
                                 { "@rewardRecipientId", referralType.RewardRecipient },
                                 { "@eventRewards", eventRewardsTable }
                             },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        referralType.Code = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Code"));
                        if (sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("NewCode")) == false)
                        {
                            result = ResultCode.Success;
                        }
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// Adds the referral in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddReferral()
        {
            // Add the referral to the data layer.
            ResultCode resultCode = SqlProcedure("AddReferral", new Dictionary<string, object>
                                                 {
                                                     { "@userId", Context[Key.ReferredUserId] },
                                                     { "@referralTypeCode", Context[Key.ReferralTypeCode] },
                                                     { "@referralEventId", Context[Key.ReferralEvent] },
                                                     { "@earnAmount", (int)Context[Key.ReferredUserFirstEarnRewardAmount] },
                                                     { "@earnExplanation", Context[Key.ReferredUserFirstEarnRewardExplanation] }
                                                 },

            (sqlDataReader) =>
            {
                if (sqlDataReader.Read() == true)
                {
                    Context[Key.ReferralAdded] = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ReferralsAdded")) == 1;
                }
            });

            if (resultCode == ResultCode.Success)
            {
                resultCode = ResultCode.Created;
            }

            return resultCode;
        }

        /// <summary>
        /// Gets the referrals that resulted from the referrer specified in the context and the events associated with those
        /// referrals.
        /// </summary>
        /// <returns>
        /// A list of referral types and counts.
        /// </returns>
        public Collection<ReferralCodeReportDataContract> RetrieveReferralCounts()
        {
            Collection<ReferralCodeReportDataContract> result = new Collection<ReferralCodeReportDataContract>();

            SqlProcedure("GetReferralCounts",
                         new Dictionary<string, object>
                         {
                             { "@referrerId", Context[Key.ReferrerId] },
                             { "@referrerTypeId", Context[Key.ReferrerType] }
                         },

                (sqlDataReader) =>
                {
                    Dictionary<string, List<ReferralCountsDataContract>> referralCountRepository =
                                                                      new Dictionary<string, List<ReferralCountsDataContract>>();
                    while (sqlDataReader.Read() == true)
                    {
                        string code = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Code"));
                        if (referralCountRepository.ContainsKey(code) == false)
                        {
                            referralCountRepository[code] = new List<ReferralCountsDataContract>();
                        }
                        List<ReferralCountsDataContract> codeList = referralCountRepository[code];

                        ReferralCountsDataContract referralCounts = new ReferralCountsDataContract
                        {
                            ReferralEventId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ReferralEventId")),
                            RewardPayoutStatusId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("RewardPayoutStatusId")),
                            Count = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Count"))
                        };
                        codeList.Add(referralCounts);
                    }

                    foreach(string key in referralCountRepository.Keys)
                    {
                        ReferralCodeReportDataContract referralReport = new ReferralCodeReportDataContract
                        {
                            ReferralCode = key,
                            ReferralEventCounts = referralCountRepository[key]
                        };
                        result.Add(referralReport);
                    }
                });

            return result;
        }

        /// <summary>
        /// Gets the unprocessed referral records belonging to the user in the context.
        /// </summary>
        /// <returns>
        /// The unprocessed referral records belonging to the user.
        /// </returns>
        public Collection<RewardPayoutRecord> RetrieveUserUnprocessedReferrals()
        {
            Collection<RewardPayoutRecord> result = new Collection<RewardPayoutRecord>();

            SqlProcedure("GetUserUnprocessedReferrals",
                         new Dictionary<string, object>
                         {
                             { "@userId", Context[Key.GlobalUserId] },
                             { "@referralEventId", Context[Key.ReferralEvent] }
                         },

                (sqlDataReader) =>
                {
                    while (sqlDataReader.Read() == true)
                    {
                        result.Add(new RewardPayoutRecord
                        {
                            RewardPayoutId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("Id")),
                            RewardType = (RewardType)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("RewardTypeId")),
                            Properties = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Properties")),
                            PayeeId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("PayeeId")),
                            PayeeType = (PayeeType)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PayeeTypeId")),
                            Rescinded = false
                        });
                    }
                });

            return result;
        }
    }
}