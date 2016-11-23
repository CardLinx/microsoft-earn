//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public static class RequestExtensions
    {
        /// <summary>
        /// Extension method that checks if the request contains a header with specified name and value.
        /// </summary>
        /// <param name="request">The Http request.</param>
        /// <param name="expectedHeaderName">The name of the header to search for.</param>
        /// <param name="expectedHeaderValue">The expected value of the header.</param>
        /// <returns>
        /// True if a header with specified name and value was found, false otherwise.
        /// </returns>
        public static bool ContainsHeader(this HttpRequestMessage request, string expectedHeaderName, string expectedHeaderValue)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (!request.Headers.Contains(expectedHeaderName))
            {
                return false;
            }

            IEnumerable<string> headerValues = request.Headers.GetValues(expectedHeaderName);

            return headerValues.Any(headerValue => string.Equals(headerValue, expectedHeaderValue, StringComparison.OrdinalIgnoreCase));
        }
    }
}