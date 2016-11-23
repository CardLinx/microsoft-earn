//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using System;
    using System.Net;
    using System.Net.Http;

	public class HttpRequestStatusException : HttpRequestException
	{
        public HttpStatusCode StatusCode { get; private set; }

        public HttpRequestStatusException(HttpStatusCode statusCode)
            : this(statusCode, null, null)
        {
        }

        public HttpRequestStatusException(HttpStatusCode statusCode, string message)
            : this(statusCode, message, null)
        {
        }

        public HttpRequestStatusException(HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            this.StatusCode = statusCode;
        }
	}
}