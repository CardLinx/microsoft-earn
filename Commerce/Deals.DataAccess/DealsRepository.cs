//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Deals.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using DotM.DataContracts;
    using ProtoBuf;

    public static class DealsRepository
    {
        /// <summary>
        /// connection string for the publisheddealsdb
        /// </summary>
        private readonly static string ConnectionString = ConfigurationManager.ConnectionStrings["PublishedDealsEntities"].ConnectionString;

        /// <summary>
        /// Given a list of ids, get the Deals back from db
        /// </summary>
        /// <param name="dealIds">
        /// Id of the deals
        /// </param>
        /// <returns>
        /// Deal objects
        /// </returns>
        public static IEnumerable<Deal> GetDealsById(IEnumerable<string> dealIds)
        {
            List<Deal> deals = new List<Deal>();
            SqlDataReader sqlDataReader = null;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("dbo.GetDealsById", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter("@DealGuidList", CreateGuidListXml(dealIds)));
                        sqlCommand.Parameters.Add(new SqlParameter("@Sort", "distance"));

                        sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            byte[] bson = (byte[])sqlDataReader["Bson"];
                            Guid dealId = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("DealGuid"));
                            string providerDealId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProviderDealId"));
                            Deal deal;
                            using (MemoryStream readStream = new MemoryStream(bson))
                            {
                                deal = Serializer.Deserialize<Deal>(readStream);
                                deal.Id = dealId.ToString();
                                deal.ProviderDealId = providerDealId;
                            }
                            deals.Add(deal);
                        }

                    }
                }

            }
            finally
            {
                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
            }

            return deals;
        }

        /// <summary>
        /// Create XML list of deal guids for query
        /// </summary>
        /// <param name="dealIds"></param>
        /// <returns></returns>
        private static string CreateGuidListXml(IEnumerable<string> dealIds)
        {
            var guidsAsStrings = dealIds.Select(g => g.ToUpperInvariant());
            return CreateDealIdXml(guidsAsStrings);
        }

        /// <summary>
        /// Create xml list of deal ids
        /// </summary>
        /// <param name="dealIds"></param>
        /// <returns></returns>
        private static string CreateDealIdXml(IEnumerable<string> dealIds)
        {
            string xml = null;
            if (dealIds != null)
            {
                xml = new XElement("guids", dealIds.Select(gid => new XElement("guid", gid))).ToString();
            }

            return xml;
        }

    }
}