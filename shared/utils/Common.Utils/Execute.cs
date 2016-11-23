//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class contains helper methods to execute an operation with a specified policy.
    /// </summary>
    public static class Execute
    {
        /// <summary>
        /// Retries the operation till it returns true or till the maximum allowed number of retries fail.
        /// If the operation throws an exception, no more retries will be made and the exception is bubbled 
        /// up to the caller.
        /// </summary>
        /// <param name="retryPolicy">The policy that determines the retry logic.</param>
        /// <param name="func">The operation to be executed with retries.</param>
        /// <returns>True if the operation was executed successfully, false if the operation failed even after the maximum number of retries.</returns>
        public static bool WithRetry(IRetryPolicy retryPolicy, Func<int, bool> func)
        {
            return WithRetry(retryPolicy, func, () => { });
        }

        /// <summary>
        /// Asynchronous retries the given function with the given retry policy
        /// </summary>
        /// <param name="retryPolicy">The policy that determines the retry logic.</param>
        /// <param name="asyncFunc">The operation to be executed with retries.</param>
        /// <returns>A task of Boolean indicating whether the operation was executed successfully within the maximum number of retries.</returns>
        public static async Task<bool> WithRetryAsync(IRetryPolicy retryPolicy, Func<int, Task<bool>> asyncFunc)
        {
            return await WithRetryAsync(retryPolicy, asyncFunc, async () => await Task.Run(() => { }));
        }

        /// <summary>
        /// Retries the operation till it returns true or till the maximum allowed number of retries fail.
        /// If the operation throws a non-fatal exception, it will ignored and 
        /// the operation will be retried (till max allowed number of retries).
        /// </summary>
        /// <param name="retryPolicy">
        /// The policy that determines the retry logic.
        /// </param>
        /// <param name="func">
        /// The operation to be executed with retries.
        /// </param>
        /// <returns>
        /// True if the operation was executed successfully, false if the operation failed even after the maximum number of retries.
        /// </returns>
        public static bool WithRetryIgnoringExceptions(IRetryPolicy retryPolicy, Func<int, bool> func)
        {
            return ExecuteInternal(retryPolicy, func, () => { }, ignoreNonFatalExceptions: true);
        }

        /// <summary>
        /// Retries the operation till it returns true or till the maximum allowed number of retries fail.
        /// If the operation throws a non-fatal exception, it will ignored and 
        /// the operation will be retried (till max allowed number of retries).
        /// </summary>
        /// <param name="retryPolicy">
        /// The policy that determines the retry logic.
        /// </param>
        /// <param name="func">
        /// The operation to be executed with retries.
        /// </param>
        /// <param name="actionToExecuteBeforeRetry">
        /// The action be executed before each retry attempt.
        /// </param>
        /// <returns>
        /// True if the operation was executed successfully, false if the operation failed even after the maximum number of retries.
        /// </returns>
        public static bool WithRetryIgnoringExceptions(IRetryPolicy retryPolicy, Func<int, bool> func, Action actionToExecuteBeforeRetry)
        {
            return ExecuteInternal(retryPolicy, func, actionToExecuteBeforeRetry, ignoreNonFatalExceptions: true);
        }

        /// <summary>
        /// Retries the operation till it returns true or till the maximum allowed number of retries fail.
        /// If the operation throws an exception, no more retries will be made and the exception is bubbled 
        /// up to the caller.
        /// </summary>
        /// <param name="retryPolicy">
        /// The policy that determines the retry logic.
        /// </param>
        /// <param name="func">
        /// The operation to be executed with retries.
        /// </param>
        /// <param name="actionToExecuteBeforeRetry">
        /// The action be executed before each retry attempt.
        /// </param>
        /// <returns>
        /// True if the operation was executed successfully, false if the operation failed even after the maximum number of retries.
        /// </returns>
        public static bool WithRetry(IRetryPolicy retryPolicy, Func<int, bool> func, Action actionToExecuteBeforeRetry)
        {
            return ExecuteInternal(retryPolicy, func, actionToExecuteBeforeRetry, ignoreNonFatalExceptions: false);
        }

        /// <summary>
        /// Executes the specified operation with retries.
        /// </summary>
        /// <param name="retryPolicy">The policy that determines the retry logic.</param>
        /// <param name="func">The operation to be executed with retries.</param>
        /// <param name="actionToExecuteBeforeRetry">The action be executed before each retry attempt.</param>
        /// <param name="ignoreNonFatalExceptions">True if non-critical exceptions should be ignored, false otherwise.</param>
        /// <returns></returns>
        private static bool ExecuteInternal(IRetryPolicy retryPolicy, Func<int, bool> func, Action actionToExecuteBeforeRetry, bool ignoreNonFatalExceptions)
        {
            if (retryPolicy == null)
            {
                throw new ArgumentNullException("retryPolicy");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (actionToExecuteBeforeRetry == null)
            {
                throw new ArgumentNullException("actionToExecuteBeforeRetry");
            }

            // Clone the retry policy to prevent reusing the same instance.
            IRetryPolicy internalRetryPolicy = retryPolicy.CreateInstance();

            if (internalRetryPolicy == null)
            {
                throw new InvalidOperationException("The CreateInstance() for the retrypolicy returned null.");
            }

            int iterationCount = 0;
            while (true)
            {
                try
                {
                    if (func(iterationCount))
                    {
                        return true;
                    }

                    if (!internalRetryPolicy.ShouldRetry)
                    {
                        return false;
                    }

                    actionToExecuteBeforeRetry();

                    Thread.Sleep(internalRetryPolicy.WaitTimeBeforeNextRetry);
                    iterationCount++;
                }
                catch (Exception ex)
                {
                    // Always re-throw fatal exceptions.
                    if (ex.IsFatal()) throw;

                    // Re-throw non-fatal exceptions if specified by caller.
                    if (!ignoreNonFatalExceptions) throw;
                }
            }
        }

        /// <summary>
        /// Asynchronous retries the given action with the given retry policy and and pre-action.
        /// </summary>
        /// <param name="retryPolicy">The policy that determines the retry logic.</param>
        /// <param name="asyncFunc">The operation to be executed with retries.</param>
        /// <param name="asyncActionToExecuteBeforeRetry">The action be executed before each retry attempt.</param>
        /// <returns>A task of Boolean indicating whether the operation was executed successfully within the maximum number of retries.</returns>
        public static async Task<bool> WithRetryAsync(IRetryPolicy retryPolicy, Func<int, Task<bool>> asyncFunc, Func<Task> asyncActionToExecuteBeforeRetry)
        {
            if (retryPolicy == null)
            {
                throw new ArgumentNullException("retryPolicy");
            }

            if (asyncFunc == null)
            {
                throw new ArgumentNullException("asyncFunc");
            }

            if (asyncActionToExecuteBeforeRetry == null)
            {
                throw new ArgumentNullException("asyncActionToExecuteBeforeRetry");
            }

            int iterationCount = 0;
            while (true)
            {
                if (await asyncFunc(iterationCount))
                {
                    return true;
                }

                if (!retryPolicy.ShouldRetry)
                {
                    return false;
                }

                await asyncActionToExecuteBeforeRetry();

                await Task.Delay(retryPolicy.WaitTimeBeforeNextRetry);
                iterationCount++;
            }
        }
    }
}