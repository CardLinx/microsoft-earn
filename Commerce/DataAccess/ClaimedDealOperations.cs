//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents operations on ClaimedDeal objects within the data store.
    /// </summary>
    public class ClaimedDealOperations : CommerceOperations, IClaimedDealOperations
    {
        /// <summary>
        /// Adds record of the claimed deal in the context to the data store.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        public ResultCode AddClaimedDeal()
        {
            ResultCode result;

            ClaimedDeal claimedDeal = (ClaimedDeal)Context[Key.ClaimedDeal];

            result = SqlProcedure("AddClaimedDeal",
                                  new Dictionary<string, object>
                                  {
                                      { "@globalDealId", claimedDeal.GlobalDealId },
                                      { "@globalUserId", claimedDeal.GlobalUserId },
                                      { "@cardId", claimedDeal.CardId },
                                      { "@partnerId", (int)claimedDeal.Partner },
                                  },

                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        Context[Key.UserFirstClaimedDeal] = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("UserFirstClaimedDeal"));
                        Context[Key.UserNewDealClaimed] = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("UserNewDealClaimed"));
                    }
                });

            if (result == ResultCode.Success)
            {
                result = ResultCode.Created;
            }

            return result;
        }

        /// <summary>
        /// Registers a bath of deals for batch claiming processing
        /// </summary>
        public void RegisterDealBatch()
        {
            DealBatch dealbatch = (DealBatch)Context[Key.DealBatch];
            int batchId = -1;
            using (DataTable dealIdsTable = new DataTable("DealIds"))
            {
                dealIdsTable.Locale = CultureInfo.InvariantCulture;
                dealIdsTable.Columns.Add("Id", typeof(int));
                foreach (int dealId in dealbatch.DealIds)
                {
                    dealIdsTable.Rows.Add(dealId);
                }

                SqlProcedure("RegisterDealBatch",
                            new Dictionary<string, object> { { "@dealIds", dealIdsTable } },
                            (sqlDataReader) =>
                            {
                                if (sqlDataReader.Read() == true)
                                {
                                    batchId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("DealBatchId"));
                                }
                            });
            }

            dealbatch.Id = batchId;
        }

        /// <summary>
        /// Add a record into claimed deals batches store
        /// </summary>
        /// <param name="partners">
        /// list of partners
        /// </param>
        /// <param name="startDate">
        /// batch start date
        /// </param>
        /// <param name="endDate">
        /// batch end date
        /// </param>
        public void AddClaimedDeals(IEnumerable<Partner> partners, DateTime startDate, DateTime endDate)
        {
            int cardId = ((Card)Context[Key.Card]).Id;
            int dealBatchId = ((DealBatch)Context[Key.DealBatch]).Id;
            using (DataTable partnerIds = new DataTable("PartnerIds"))
            {
                partnerIds.Locale = CultureInfo.InvariantCulture;
                partnerIds.Columns.Add("PartnerId", typeof(int));
                if (partners != null)
                {
                    foreach (Partner partner in partners)
                    {
                        partnerIds.Rows.Add((int) partner);
                    }

                    SqlProcedure("AddClaimedDeals",
                                new Dictionary<string, object> 
                        { 
                            { "@cardId", cardId },
                            { "@partnerIds", partnerIds },
                            { "@dealBatchId", dealBatchId },
                            { "@startDate", startDate }, 
                            { "@endDate", endDate } 
                        });
                }
            }
        }

        /// <summary>
        /// Retrieves the list of deals currently claimed by the user in the context.
        /// </summary>
        /// <returns>
        /// The list of deals currently claimed by the user in the context.
        /// </returns>
        public IEnumerable<Guid> RetrieveClaimedDeals()
        {
            List<Guid> result = new List<Guid>();

            SqlProcedure("GetClaimedDealsByUser",
                         new Dictionary<string, object>
                         {
                             { "@globalUserId", ((User)Context[Key.User]).GlobalId }
                         },

                (sqlDataReader) =>
                {
                    while (sqlDataReader.Read() == true)
                    {
                        result.Add(sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("GlobalDealId")));
                    }
                });

            Context.Log.Verbose("GetClaimedDealsByUser retrieved {0} claimed Deals for the specified User.", result.Count);

            return result;
        }
    }
}