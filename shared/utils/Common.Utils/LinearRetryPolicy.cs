//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;

    /// <summary>
    /// A retry policy that retries a specified number of times and waits a fixed time 
    /// between each retry.
    /// Instances of this class are not re-usable across operations.
    /// </summary>
    public class LinearRetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// The maximum allowed number of retry attempts.
        /// </summary>
        private readonly int maxRetries;

        /// <summary>
        /// The time to wait between retries.
        /// </summary>
        private readonly TimeSpan waitTimeBetweenRetries;

        /// <summary>
        /// Counts the number of times the operation was attempted.
        /// </summary>
        private int currentAttempt = 0;

        /// <summary>
        /// Initializes a new instance of the LinearRetryPolicy class.
        /// </summary>
        /// <param name="waitTimeBetweenRetries">The time to wait before the next retry attempt.</param>
        /// <param name="maxRetries">The maximum allowed number of retries.</param>
        public LinearRetryPolicy(TimeSpan waitTimeBetweenRetries, int maxRetries)
        {
            if (maxRetries < 0)
            {
                throw new ArgumentException("The retry count cannot be less than zero", "maxRetries");
            }

            this.maxRetries = maxRetries;
            this.waitTimeBetweenRetries = waitTimeBetweenRetries;
        }

        /// <summary>
        /// Initializes a new instance of the LinearRetryPolicy class.
        /// </summary>
        /// <param name="waitTimeInMilliseconds">The time to wait in milliseconds before the next retry attempt.</param>
        /// <param name="maxRetries">The maximum allowed number of retries.</param>
        public LinearRetryPolicy(long waitTimeInMilliseconds, int maxRetries)
            : this(TimeSpan.FromMilliseconds(waitTimeInMilliseconds), maxRetries)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the operation should be retried.
        /// </summary>
        public bool ShouldRetry
        {
            get
            {
                return this.currentAttempt < this.maxRetries;
            }
        }

        /// <summary>
        /// Gets a value indicating the time period to wait before retrying the operation.
        /// This property is not idempotent. It should be called only once before a retry attempt.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown when the operation has already been executed the maximum allowed number of times.
        /// Check the ShouldRetry property before calling this property. 
        /// </exception>
        public TimeSpan WaitTimeBeforeNextRetry
        {
            get
            {
                if (!this.ShouldRetry)
                {
                    throw new InvalidOperationException("The operation exceeded the maximum retries.");
                }

                this.currentAttempt++;

                return this.waitTimeBetweenRetries;
            }
        }

        /// <summary>
        /// Creates a new instance of LinearRetryPolicy with clean state.
        /// </summary>
        /// <returns>
        /// An instance of LinearRetryPolicy class.
        /// </returns>
        public IRetryPolicy CreateInstance()
        {
            return new LinearRetryPolicy(this.waitTimeBetweenRetries, this.maxRetries);
        }
    }
}