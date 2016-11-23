//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents a log record to be added to the azure table
    /// </summary>
    public class DashboardLogEntity : TableEntity 
    {
        /// <summary>
        /// Initializes LogEntity. 
        /// </summary>
        /// <param name="requestId">
        /// Request Id that uniquely identifies the service request to the commerce server
        /// </param>
        /// <param name="resultCode">
        /// Result code of the service invocation
        ///  </param>
        /// <param name="description">
        /// Detailed description of the result code
        /// </param>
        public DashboardLogEntity(Guid requestId,
                         string resultCode,
                         string description)
        {
            RequestId = requestId;
            ResultCode = resultCode;
            Description = description;
            HourOfTheDay = DateTime.UtcNow.Hour;

            PartitionKey = DateTime.UtcNow.ToString("yyyy-MM-dd");
            RowKey = RequestId + ResultCode;
        }
        
        /// <summary>
        /// Gets or Sets the request Id that uniquely identifies the service request to the commerce server
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or Sets the result code of the service invocation
        /// </summary>
        public string ResultCode { get; set; }

        /// <summary>
        /// Gets or Sets the summary explanation of the result code
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets the hour of the day when the record is logged - Added this field for filtering results
        /// </summary>
        public int HourOfTheDay { get; set; }
    }
}