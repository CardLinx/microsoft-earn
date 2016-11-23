//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;

    /// <summary>
    /// A retry policy that retries a specified number of times with a randomized exponential backoff scheme, 
    /// using specified minimum and maximum backoff values.
    /// The instances of this class are neither re-usable nor threadsafe.
    /// </summary>
    public class ExponentialBackoffRetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// The minimum time to wait before retrying.
        /// </summary>
        private readonly TimeSpan minBackoff;

        /// <summary>
        /// The maximum time to wait before retrying.
        /// </summary>
        private readonly TimeSpan maxBackoff;

        /// <summary>
        /// The delta backoff value used by the exponential backoff retry policy.
        /// </summary>
        private readonly TimeSpan deltaBackoff;

        /// <summary>
        /// The maximum allowed retry attempts.
        /// </summary>
        private readonly int maxRetryAttempts;

        /// <summary>
        /// The base used in the exponential expression.
        /// In the expression x raised to the yth power, x is the base and y is the exponent.
        /// </summary>
        private readonly double exponentBase;

        /// <summary>
        /// The count of retry attempts already made.
        /// </summary>
        private int currentAttempt;

        /// <summary>
        /// The current value of wait time before next retry.
        /// </summary>
        private TimeSpan currentBackoff;

        /// <summary>
        /// Random number generator for adding randomness for the backoff values.
        /// Since this instance is static, it should NOT be used directly since it
        /// can result in multi-threading issues. Instead it should used via the 
        /// GetNextRandomNumber() method which synchronizes access to this instance.
        /// Benchmarking showed that this is way better than creating new instances of Random.
        /// </summary>
        private static readonly Random RandomNumberGenerator = new Random();

        /// <summary>
        /// Initializes a new instance of the ExponentialBackoffRetryPolicy class with default values.
        /// Default values are:
        /// minBackOff - 100 ms
        /// maxBackOff - 1 seconds
        /// deltaBackOff - 100 ms
        /// maxRetryCount - 10
        /// exponentBase - 2.0
        /// This retries are made with the following interval pattern: 
        /// 100ms, 200ms, 400ms, 800ms, 1s, 1s, 1s, 1s, 1s, 1s.
        /// </summary>
        public ExponentialBackoffRetryPolicy()
            : this(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100), 10)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExponentialBackoffRetryPolicy class.
        /// This overload uses a default value of 2.0 for the exponent base. 
        /// </summary>
        /// <param name="minBackoff">The minimum time to wait before retrying.</param>
        /// <param name="maxBackoff">The maximum time to wait before retrying.</param>
        /// <param name="deltaBackoff">The delta backoff value used by the exponential backoff retry policy.</param>
        /// <param name="maxRetryAttempts">A non-negative number indicating the maximum number of times to retry.</param>
        public ExponentialBackoffRetryPolicy(TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff, int maxRetryAttempts)
            : this(minBackoff, maxBackoff, deltaBackoff, maxRetryAttempts, 2.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExponentialBackoffRetryPolicy class.
        /// This overload allows the caller to specify a non-default value for the base component of the exponential expression.
        /// </summary>
        /// <param name="minBackoff">The minimum time to wait before retrying.</param>
        /// <param name="maxBackoff">The maximum time to wait before retrying.</param>
        /// <param name="deltaBackoff">The delta backoff value used by the exponential backoff retry policy.</param>
        /// <param name="maxRetryAttempts">A non-negative number indicating the maximum number of times to retry.</param>
        /// <param name="exponentBase">
        /// In the expression x raised to the yth power, x is the base and y is the exponent. By default, 2.0 is used as the base.
        /// Pass in a different value for the base if the exponential should grow differently.
        /// </param>
        public ExponentialBackoffRetryPolicy(TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff, int maxRetryAttempts, double exponentBase)
        {
            if (minBackoff > maxBackoff)
            {
                throw new ArgumentException("minBackOff should not be greater than maxBackOff.", "minBackoff");
            }

            if (maxRetryAttempts < 0)
            {
                throw new ArgumentException("maxRetryAttempts cannot be a negative value.", "maxRetryAttempts");
            }

            if (exponentBase < 1.0)
            {
                // TODO: should we allow values less than 1.0 ?
                throw new ArgumentException("The base for the exponential expression should not be less than 1.0", "exponentBase");
            }

            this.minBackoff = minBackoff;
            this.maxBackoff = maxBackoff;
            this.deltaBackoff = deltaBackoff;
            this.maxRetryAttempts = maxRetryAttempts;
            this.currentAttempt = 0;
            this.currentBackoff = this.minBackoff;
            this.exponentBase = exponentBase;
        }

        /// <summary>
        /// Gets a value indicating whether the operation should be retried.
        /// </summary>
        public bool ShouldRetry
        {
            get
            {
                return this.currentAttempt < this.maxRetryAttempts;
            }
        }

        /// <summary>
        /// Gets a value indicating the time span to wait before retrying the operation.
        /// When this property is called, an internal counter which tracks the number of retry attempts
        /// is incremented and hence it should be called only once per retry attempt.
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

                if (this.currentAttempt > 1 && this.currentBackoff < this.maxBackoff)
                {
                    var randomValue = GetNextRandomNumber((int)(this.deltaBackoff.TotalMilliseconds * 0.8), (int)(this.deltaBackoff.TotalMilliseconds * 1.2));

                    // Calculate Exponential back-off with +/- 20% tolerance
                    var increment = Math.Pow(this.exponentBase, this.currentAttempt - 1.0) * randomValue;

                    var totalWaitTime = this.minBackoff.TotalMilliseconds + increment;

                    // Enforce back-off boundaries
                    this.currentBackoff = TimeSpan.FromMilliseconds(Math.Min(totalWaitTime, this.maxBackoff.TotalMilliseconds));
                }

                return this.currentBackoff;
            }
        }

        /// <summary>
        /// Gets a pseudo-random value within the specified boundaries.
        /// This function should be used to generate the random numbers instead of directly 
        /// using the RandomNumberGenerator static instance.
        /// </summary>
        /// <param name="minValue">The lower bound for the random value.</param>
        /// <param name="maxValue">The upper bound for the random value.</param>
        /// <returns>A pseudo-random integer within the specified bounds.</returns>
        private static int GetNextRandomNumber(int minValue, int maxValue)
        {
            lock (RandomNumberGenerator)
            {
                return RandomNumberGenerator.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Creates a new instance of ExponentialBackoffRetryPolicy with clean state.
        /// </summary>
        /// <returns>
        /// An instance of ExponentialBackoffRetryPolicy class.
        /// </returns>
        public IRetryPolicy CreateInstance()
        {
            return new ExponentialBackoffRetryPolicy(
                this.minBackoff,
                this.maxBackoff,
                this.deltaBackoff,
                this.maxRetryAttempts,
                this.exponentBase);
        }
    }
}