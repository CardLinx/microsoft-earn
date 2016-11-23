//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Scheduler
{
    /// <summary>
    /// Factory that gives an implementation of Scheduler
    /// </summary>
    public class SchedulerFactory
    {
        /// <summary>
        /// Get appropriate Scheduler
        /// </summary>
        /// <returns>
        /// Scheduler Instance
        /// </returns>
        public static IScheduler GetScheduler(string connectionString, string queueName, string tableName)
        {
            IScheduler result = null;
            
            if (string.IsNullOrWhiteSpace(connectionString) == false)
            {
                result = new AzureScheduler(connectionString, queueName, tableName);
            }

            return result;
        }
    }
}