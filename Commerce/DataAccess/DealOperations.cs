//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on Deal objects within the data store.
    /// </summary>
    public class DealOperations : CommerceOperations, IDealOperations
    {
        /// <summary>
        /// Registers the Deal in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter deal cannot be null.
        /// </exception>
        public ResultCode RegisterDeal()
        {
            MergeDeal();
            return ResultCode.Created;
        }

        /// <summary>
        /// Retrieves the Deal with the ID in the context.
        /// </summary>
        /// <returns>
        /// * The Deal with the specified ID if it exists.
        /// * Else returns null.
        /// </returns>
        public Deal RetrieveDeal()
        {
            Deal result = null;

            Guid dealId = (Guid)Context[Key.GlobalDealId];
            SqlProcedure("GetOfferByGlobalID",
                         new Dictionary<string, object>
                         {
                             { "@globalOfferID", dealId }
                         },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        // Get the deal.
                        result = new Deal();
                        result.GlobalId = dealId;
                        result.Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("OfferId"));
                        result.StartDate = DateTime.MinValue;
                        result.EndDate = DateTime.MaxValue;
                        result.Currency = "USD";
                        result.Amount = 0;
                        result.MinimumPurchase = 0;
                        result.Count = 0;
                        result.UserLimit = 0;
                        result.MaximumDiscount = 0;
                        result.DealStatusId = DealStatus.Activated;

                        // Add items that need special handling.
                        result.ReimbursementTender = (ReimbursementTender)(sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("OfferType")) + 2);
                        decimal percentBack = sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("PercentBack"));
                        if (percentBack > 0)
                        {
                            result.Percent = percentBack;
                        }

                        PartnerDealInfo partnerDealInfo = new PartnerDealInfo
                        {
                            PartnerId = Partner.FirstData,
                            PartnerDealId = "e03bae20c9cda439",
                            PartnerDealRegistrationStatusId = PartnerDealRegistrationStatus.Pending
                        };
                        result.PartnerDealInfoList.Add(partnerDealInfo);



/* DEPRECATED CODE
                        // Add nullable items.
                        int providerIdColumnId = sqlDataReader.GetOrdinal("ProviderId");
                        if (sqlDataReader.IsDBNull(providerIdColumnId) == false)
                        {
                            result.ProviderId = sqlDataReader.GetString(providerIdColumnId);
                        }

                        int merchantIdColumnId = sqlDataReader.GetOrdinal("MerchantId");
                        if (sqlDataReader.IsDBNull(merchantIdColumnId) == false)
                        {
                            result.MerchantId = sqlDataReader.GetString(merchantIdColumnId);
                        }

                        int dayTimeRestrictionsColumnId = sqlDataReader.GetOrdinal("DayTimeRestrictions");
                        if (sqlDataReader.IsDBNull(dayTimeRestrictionsColumnId) == false)
                        {
                            result.DayTimeRestrictions = XElement.Parse(sqlDataReader.GetString(dayTimeRestrictionsColumnId));
                        }

                        int merchantNameColumnId = sqlDataReader.GetOrdinal("MerchantName");
                        if (sqlDataReader.IsDBNull(merchantNameColumnId) == false)
                        {
                            result.MerchantName = sqlDataReader.GetString(merchantNameColumnId);
                        }

                        int parentDealIdColumnId = sqlDataReader.GetOrdinal("ParentDealId");
                        if (sqlDataReader.IsDBNull(parentDealIdColumnId) == false)
                        {
                            result.ParentDealId = sqlDataReader.GetGuid(parentDealIdColumnId);
                        }

                        int discountSummaryColumnId = sqlDataReader.GetOrdinal("DiscountSummary");
                        if (sqlDataReader.IsDBNull(discountSummaryColumnId) == false)
                        {
                            result.DiscountSummary = sqlDataReader.GetString(discountSummaryColumnId);
                        }

                        // Get the partner deals for the deal.
                        if (sqlDataReader.NextResult() == true)
                        {
                            Dictionary<Partner, PartnerDealInfo> partnerDeals = new Dictionary<Partner, PartnerDealInfo>();
                            while (sqlDataReader.Read() == true)
                            {
                                Partner partner = (Partner)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PartnerId"));
                                PartnerDealInfo partnerDealInfo = new PartnerDealInfo
                                {
                                    PartnerId = partner,
                                    PartnerDealId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerDealId")),
                                    PartnerDealRegistrationStatusId = (PartnerDealRegistrationStatus)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PartnerDealRegistrationStatusId"))
                                };
                                partnerDeals[partner] = partnerDealInfo;
                                result.PartnerDealInfoList.Add(partnerDealInfo);
                            }

                            // Get the partner merchant IDs for the deal.
                            if (sqlDataReader.NextResult() == true)
                            {
                                while (sqlDataReader.Read() == true)
                                {
                                    Partner partner = (Partner)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PartnerId"));
                                    if (partnerDeals.ContainsKey(partner) == true)
                                    {
                                        PartnerDealInfo partnerDealInfo = partnerDeals[partner];
                                        int timeZonePos = sqlDataReader.GetOrdinal("MerchantTimeZoneId");
                                        PartnerMerchantLocationInfo partnerMerchantLocationInfo = new PartnerMerchantLocationInfo
                                        {
                                            PartnerMerchantId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("PartnerMerchantId")),
                                            PartnerMerchantIdType = (PartnerMerchantIdType)sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PartnerMerchantIdTypeId")),
                                            MerchantTimeZoneId = sqlDataReader.IsDBNull(timeZonePos) ? null : sqlDataReader.GetString(timeZonePos)
                                        };
                                        partnerDealInfo.PartnerMerchantLocations.Add(partnerMerchantLocationInfo);
                                    }
                                }
                            }
                        }
// END DEPRECATED CODE */
                    }
                });

            if (result != null)
            {
                Context.Log.Verbose("GetDealById retrieved the specified Deal.");
            }
            else
            {
                Context.Log.Verbose("GetDealById could not find the specified Deal.");
            }

            return result;
        }

        /// <summary>
        /// Updates the Deal in the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode UpdateDeal()
        {
            MergeDeal();
            return ResultCode.Success;
        }

        /// <summary>
        /// Retrieve all currently active discounts in the system
        /// </summary>
        /// <returns>
        /// Enumeration of all such discount ids
        /// </returns>
        public IEnumerable<Guid> RetrieveActiveDiscountIds()
        {
            List<Guid> result = new List<Guid>();

//TODO: Receiving ReimbursementTender.MicrosoftBurn is a hack. Since ReimbursementTender was not intended for bitmasking, it has neither a None nor an All.
//       Until this can be changed, sending Burn will be treated the same as DealCurrency | MicrosoftBurn would be were it an option. Since masking is not an
//       option, there's an additional hack to translate this into "exclude Earn deals". All this can be cleaned up when First Data has been deprecated and can be removed.
            ReimbursementTender reimbursementTender = ReimbursementTender.DeprecatedBurn;
            if (Context.ContainsKey(Key.RewardProgramType) == true)
            {
                reimbursementTender = (ReimbursementTender)Context[Key.ReimbursementTender];
            }

            SqlProcedure("GetActiveDealIds",
                         new Dictionary<string, object>
                         {
                             { "@excludeEarn", reimbursementTender != ReimbursementTender.MicrosoftEarn },
                             { "@partnerId", Context[Key.Partner] }
                         },
                 (sqlDataReader) =>
                 {
                     while (sqlDataReader.Read() == true)
                     {
                         result.Add(sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalId")));
                     }
                 });

             Context.Log.Verbose("RetrieveActiveDiscountIds retrieved {0} Discounts.", result.Count);

             return result;
        }

        /// <summary>
        /// Retrive the discount id, given partner id and partner deal id
        /// </summary>
        /// <returns>
        /// DiscountId if exists, else null
        /// </returns>
        public Guid? RetrieveDiscountIdFromPartnerDealId()
        {
            Guid? discountId = null;
            string partnerDealId = (string)Context[Key.PartnerDealId];
            int partnerId = (int)Context[Key.Partner];
            SqlProcedure("GetDealByPartnerDealId",
                new Dictionary<string, object>
                {
                    { "@partnerDealId", partnerDealId },
                    { "@partnerId", partnerId}
                },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read())
                    {
                        discountId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalId"));
                    }
                });

            if (discountId != null)
            {
                Context.Log.Verbose("GetDealByPartnerDealId retrieved the specified Deal.");
            }
            else
            {
                Context.Log.Verbose("GetDealByPartnerDealId could not find the specified Deal.");
            }

            return discountId;
        }

        /// <summary>
        /// Merges the specified Deal into the deals within the data store.
        /// </summary>
        /// <param name="deal">
        /// The Deal to merge.
        /// </param>
        private void MergeDeal()
        {
            Deal deal = (Deal)Context[Key.Deal];

            using (DataTable partnerDealInfoTable = new DataTable("PartnerDealInfo"))
            {
                // Build the PartnerDealInfo table parameter.
                partnerDealInfoTable.Locale = CultureInfo.InvariantCulture;
                partnerDealInfoTable.Columns.Add("PartnerId", typeof(Int32));
                partnerDealInfoTable.Columns.Add("PartnerDealId", typeof(string));
                partnerDealInfoTable.Columns.Add("PartnerDealRegistrationStatusId", typeof (Int32));
                partnerDealInfoTable.Columns.Add("DealId", typeof (Guid));

                using (DataTable dealPartnerMerchantIds = new DataTable("DealPartnerMerchants"))
                {
                    // Build the DealPartnerMerchantIds table parameter.
                    dealPartnerMerchantIds.Locale = CultureInfo.InvariantCulture;
                    dealPartnerMerchantIds.Columns.Add("PartnerId", typeof(Int32));
                    dealPartnerMerchantIds.Columns.Add("PartnerMerchantId", typeof(string));
                    dealPartnerMerchantIds.Columns.Add("PartnerMerchantIdTypeId", typeof(Int32));
                    dealPartnerMerchantIds.Columns.Add("MerchantTimeZoneId", typeof(string));

                    // Populate the PartnerDealInfo and DealPartnerMerchantIds tables.
                    foreach(PartnerDealInfo partnerDealInfo in deal.PartnerDealInfoList)
                    {
                        partnerDealInfoTable.Rows.Add(partnerDealInfo.PartnerId, partnerDealInfo.PartnerDealId, partnerDealInfo.PartnerDealRegistrationStatusId, deal.GlobalId);
                        foreach (PartnerMerchantLocationInfo partnerMerchantLocationInfo in partnerDealInfo.PartnerMerchantLocations)
                        {
                            dealPartnerMerchantIds.Rows.Add(partnerDealInfo.PartnerId,
                                                            partnerMerchantLocationInfo.PartnerMerchantId,
                                                            partnerMerchantLocationInfo.PartnerMerchantIdType,
                                                            partnerMerchantLocationInfo.MerchantTimeZoneId);
                        }
                    }

                    // Add the deal to the data layer.
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                                                            {
                                                                { "@globalId", deal.GlobalId },
                                                                { "@parentDealId", deal.ParentDealId },
                                                                { "@providerId", deal.ProviderId },
                                                                { "@merchantId", deal.MerchantId },
                                                                { "@merchantCategory", deal.ProviderCategory },
                                                                { "@merchantName", deal.MerchantName },
                                                                { "@startDate", deal.StartDate },
                                                                { "@endDate", deal.EndDate },
                                                                { "@currency", deal.Currency },
                                                                { "@reimbursementTenderId", deal.ReimbursementTender },
                                                                { "@amount", deal.Amount },
                                                                { "@percent", deal.Percent },
                                                                { "@minimumPurchase", deal.MinimumPurchase },
                                                                { "@count", deal.Count },
                                                                { "@userLimit", deal.UserLimit },
                                                                { "@discountSummary", deal.DiscountSummary },
                                                                { "@maximumDiscount", deal.MaximumDiscount },
                                                                { "@partnerDealInfo", partnerDealInfoTable },
                                                                { "@dealPartnerMerchantIds", dealPartnerMerchantIds },
                                                                { "@dealStatusId", deal.DealStatusId },
                                                                { "@dayTimeRestrictions", deal.DayTimeRestrictions != null ? deal.DayTimeRestrictions.ToString() : null }
                                                         };
                    SqlProcedure("MergeDeal", 
                        parameters,
                        (sqlDataReader) =>
                        {
                             while (sqlDataReader.Read() == true)
                             {
                                 deal.Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Id"));
                             }
                        });
                }
            }
        }
    }
}