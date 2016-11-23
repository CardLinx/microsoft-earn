//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using System.Collections.Generic;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Represents common operations dealing with the data store.
    /// </summary>
    public class CommonOperations : CommerceOperations
    {
        /// <summary>
        /// Gets partner merchant time zone id from the data store.
        /// </summary>
        /// <returns>time zone id string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetPartnerMerchantTimeZoneId()
        {
            string merchantTimeZoneId = null;
            SqlProcedure("GetPartnerMerchantTimeZoneId",
                new Dictionary<string, object>
                {
                    { "@partnerId", (int)Context[Key.Partner] },
                    { "@partnerMerchantId", Context[Key.PartnerMerchantId] },
                },
                (sqlDataReader) =>
                {
                    if (sqlDataReader.Read() == true)
                    {
                        int position = sqlDataReader.GetOrdinal("MerchantTimeZoneId");
                        if (sqlDataReader.IsDBNull(position) == false)
                        {
                            merchantTimeZoneId = sqlDataReader.GetString(position);
                        }
                    }
                });

            return merchantTimeZoneId;
        }
    }
}