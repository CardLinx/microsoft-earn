//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the RetryPolicyProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;

    using Lomo.Logging;

    using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
    using Microsoft.Practices.TransientFaultHandling;

    /// <summary>
    /// The retry policy provider.
    /// </summary>
    internal static class RetryPolicyProvider
    {
        #region Consts /Static

        /// <summary>
        ///     The number of retries.
        /// </summary>
        private const int NumberOfRetries = 5;

        /// <summary>
        ///     The retry delay.
        /// </summary>
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(1);

        /// <summary>
        ///     The retry increment.
        /// </summary>
        private static readonly TimeSpan RetryIncrement = TimeSpan.FromSeconds(2);

        #endregion

        /// <summary>
        ///     Returns a retry policy for use in accessing the database.
        /// </summary>
        /// <returns> A retry policy for use in accessing the database. </returns>
        internal static RetryPolicy<SqlAzureTransientErrorDetectionStrategy> GetRetryPolicy()
        {
            // SQL Azure retry strategy
            var retryStrategy = new Incremental(NumberOfRetries, RetryDelay, RetryIncrement);
            var retryPolicy = new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += (sender, args) => Log.Verbose("Retry - Count:{0}, Delay:{1}, Exception:{2}", args.CurrentRetryCount, args.Delay, args.LastException);
            return retryPolicy;
        }

        /// <summary>
        /// Returns a retry policy for use in accessing the database.
        /// </summary>
        /// <param name="retryStrategy">
        /// The retry Strategy.
        /// </param>
        /// <returns>
        /// A retry policy for use in accessing the database. 
        /// </returns>
        internal static RetryPolicy<SqlAzureTransientErrorDetectionStrategy> GetRetryPolicy(RetryStrategy retryStrategy)
        {
            // SQL Azure retry strategy
            var retryPolicy = new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += (sender, args) => Log.Verbose("Retry - Count:{0}, Delay:{1}, Exception:{2}", args.CurrentRetryCount, args.Delay, args.LastException);
            return retryPolicy;
        }
    }
}