//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the ResultSummary class.
    /// </summary>
    public static class ResultSummaryExtensions
    {
        /// <summary>
        /// Initializes the specified ResultSummary object.
        /// </summary>
        /// <param name="resultSummary">
        /// The ResultSummary object to initialize.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter resultSummary cannot be null.
        /// </exception>
        public static void Initialize(this ResultSummary resultSummary)
        {
            if (resultSummary == null)
            {
                throw new ArgumentNullException("resultSummary", "Parameter resultSummary cannot be null.");
            }

            resultSummary.ResultCode = ResultCode.Success.ToString();
        }

        /// <summary>
        /// Gets the ResultCode for the specified ResultSummary object.
        /// </summary>
        /// <param name="resultSummary">
        /// The ResultSummary object whose ResultCode to retrieve.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter resultSummary cannot be null.
        /// </exception>
        public static ResultCode GetResultCode(this ResultSummary resultSummary)
        {
            if (resultSummary == null)
            {
                throw new ArgumentNullException("resultSummary", "Parameter resultSummary cannot be null.");
            }

            return (ResultCode) Enum.Parse(typeof (ResultCode), resultSummary.ResultCode);
        }

        /// <summary>
        /// Sets the ResultCode for the specified ResultSummary object to the specified value.
        /// </summary>
        /// <param name="resultSummary">
        /// The ResultSummary object whose ResultCode to set.
        /// </param>
        /// <param name="value">
        /// The value to which to set the ResultSummary's ResultCode.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter resultSummary cannot be null.
        /// </exception>
        public static void SetResultCode(this ResultSummary resultSummary,
                                         ResultCode value)
        {
            if (resultSummary == null)
            {
                throw new ArgumentNullException("resultSummary", "Parameter resultSummary cannot be null.");
            }

            // Set the ResultCode and the corresponding Explanation.
            resultSummary.ResultCode = value.ToString();
            resultSummary.Explanation = ResultCodeExplanation.Get(resultSummary.GetResultCode());
        }
    }
}