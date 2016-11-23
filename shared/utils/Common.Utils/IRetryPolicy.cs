//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;

    /// <summary>
    /// Represents a retry policy.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Creates a new instance of the current retry policy with clean state.
        /// </summary>
        /// <returns>
        /// An instance of the current retry policy type.
        /// </returns>
        IRetryPolicy CreateInstance();

        /// <summary>
        /// Gets a value indicating whether the operation should be retried.
        /// </summary>
        bool ShouldRetry { get; }

        /// <summary>
        /// Gets a value indicating the time period to wait before retrying the operation.
        /// This property is not idempotent. It should be called only once before a retry attempt.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown when the operation has already been executed the maximum allowed number of times.
        /// Check the ShouldRetry property before calling this property. 
        /// </exception>
        TimeSpan WaitTimeBeforeNextRetry { get; }
    }
}