//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the Response class.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Initializes the specified Response object.
        /// </summary>
        /// <param name="response">
        /// The Response object to initialize.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter response cannot be null.
        /// </exception>
        public static void Initialize(this CommerceResponse response,
                                      Guid requestId)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response", "Parameter response cannot be null.");
            }

            response.RequestInformation = new RequestInformation();
            response.RequestInformation.Initialize(requestId);
            response.ResultSummary = new ResultSummary();
            response.ResultSummary.Initialize();
        }
    }
}