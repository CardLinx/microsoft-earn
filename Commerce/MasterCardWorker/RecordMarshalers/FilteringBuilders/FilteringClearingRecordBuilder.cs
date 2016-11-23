//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.MasterCardWorker
{
    using Lomo.Commerce.MasterCardClient;

    /// <summary>
    /// Builds a MasterCard filtering clearing record.
    /// </summary>
    public static class FilteringClearingRecordBuilder
    {
        /// <summary>
        /// Builds the filtering clearing record for the specified record.
        /// </summary>
        /// <param name="record">
        /// The record for which to build a filtering clearing record.
        /// </param>
        /// <param name="date">
        /// The date to add to the filtering clearing record.
        /// </param>
        /// <param name="mappingId">
        /// The mapping ID for the filtering record.
        /// </param>
        /// <returns>
        /// The filtering clearing record for the specified record.
        /// </returns>
        internal static string Build(FilteringRecord record,
                                     string date,
                                     int mappingId)
        {
            return FilteringRecordBuilder.Build(record, date, ClearingRecordCode, record.ClearingSetId, mappingId);
        }

        /// <summary>
        /// The code to use for clearing records.
        /// </summary>
        private const string ClearingRecordCode = "STAMT";
    }
}