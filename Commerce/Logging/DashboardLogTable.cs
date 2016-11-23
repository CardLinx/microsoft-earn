//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.IO;

namespace Lomo.Commerce.Logging
{
    using System;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Configuration;
    using Lomo.Logging;

    /// <summary>
    /// ILogTable implementation to log the data to azure table
    /// </summary>
    public static class DashboardLogTable
    {
        /// <summary>
        /// Adds a log entry to the log table
        /// </summary>
        /// <param name="requestId">
        /// Request Id that uniquely identifies the service request to the commerce server
        /// </param>
        /// <param name="resultCode">
        /// Result code of the service invocation
        /// </param>
        /// <param name="resultSummary">
        /// Summary explanation of the result code
        /// </param>
        /// <param name="config">
        /// The configuration to use to get settings.
        /// </param>
        public static void Add(Guid requestId,
                               ResultCode resultCode,
                               string resultSummary,
                               CommerceConfig config)
        {
            try
            {
                if (config == null)
                {
                    throw new ArgumentNullException("config", "Parameter config cannot be null.");
                }

                if (config.EnableServiceHealth == true)
                {
                    DashboardLogEntity logEntity = new DashboardLogEntity(requestId, resultCode.ToString(), resultSummary);
                    PartnerFactory.AzureTable(config).InsertEntity(logEntity);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception,string.Format("Unable to log the data to azure table : {0} ",exception.Message));
            }            
        }
    }
}