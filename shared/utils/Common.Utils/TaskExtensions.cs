//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The task extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Common.Utils
{
    using System;
    using System.Threading.Tasks;

    using Lomo.Logging;

    /// <summary>
    ///     The task extensions.
    /// </summary>
    public static class TaskExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The log exceptions.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        /// <param name="messageFormat">
        /// The message format
        /// </param>
        /// <returns>
        /// A task
        /// </returns>
        public static Task LogExceptions(this Task task, string messageFormat)
        {
            return task.ContinueWith(
                t =>
                {
                    Log.Error(messageFormat, t.Exception.Flatten());
                },
                  TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Creates a continuation that triggers <paramref name="next"/> async execution only if <parparamrefam name="first"/> exuction completed succefully
        /// </summary>
        /// <typeparam name="T1">
        /// The current task return type 
        /// </typeparam>
        /// <typeparam name="T2">
        /// The next task return type 
        /// </typeparam>
        /// <param name="first">
        /// The current task 
        /// </param>
        /// <param name="next">
        /// A function that receive as an input the current task input, triggers async execution and return the async task. The func will be called only if <paramref name="first"/> copmleted successfully 
        /// </param>
        /// <returns>
        /// The System.Threading.Tasks.Task`1[TResult - &gt; T2]. 
        /// </returns>
        /// <remarks>
        /// For information can be found here: http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx
        /// </remarks>
        public static Task<T2> Then<T1, T2>(this Task<T1> first, Func<T1, Task<T2>> next)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            var tcs = new TaskCompletionSource<T2>();
            first.ContinueWith(
                delegate
                {
                    if (first.IsFaulted)
                    {
                        tcs.TrySetException(first.Exception.InnerExceptions);
                    }
                    else if (first.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        try
                        {
                            var t = next(first.Result);
                            if (t == null)
                            {
                                tcs.TrySetCanceled();
                            }
                            else
                            {
                                t.ContinueWith(
                                    delegate
                                    {
                                        if (t.IsFaulted)
                                        {
                                            tcs.TrySetException(t.Exception.InnerExceptions);
                                        }
                                        else if (t.IsCanceled)
                                        {
                                            tcs.TrySetCanceled();
                                        }
                                        else
                                        {
                                            tcs.TrySetResult(t.Result);
                                        }
                                    },
                                    TaskContinuationOptions.ExecuteSynchronously);
                            }
                        }
                        catch (Exception exc)
                        {
                            tcs.TrySetException(exc);
                        }
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        #endregion
    }
}