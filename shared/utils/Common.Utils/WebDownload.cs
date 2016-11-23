//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Provides functionality to control timeout for an instance of webclient
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Utils
{
    using System;
    using System.Diagnostics;
    using System.Net;

    /// <summary>
    ///  Provides functionality to control timeout for an instance of webclient
    /// </summary>
    public class WebDownload : WebClient
    {
        /// <summary>
        ///  Timeout in milliseconds - backing variable
        /// </summary>
        private int timeout;

        /// <summary>
        /// URI of the resource that responded to the request - backing variable
        /// </summary>
        private Uri responseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDownload" /> class.
        /// </summary>
        public WebDownload()
        {
            this.timeout = 600000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDownload" /> class with the specified timeout value
        /// </summary>
        /// <param name="timeout">
        /// Timeout in milliseconds
        /// </param>
        public WebDownload(int timeout)
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds
        /// </summary>
        public int Timeout
        {
            get { return timeout; }

            set { timeout = value; }
        }

        /// <summary>
        /// Gets the URI of the resource that responded to the request
        /// </summary>
        public Uri ResponseUri
        {
            get { return responseUri; }
        }

        /// <summary>
        /// Returns the response for the specified request
        /// </summary>
        /// <param name="request">
        ///  Request to get the response
        /// </param>
        /// <returns>
        ///  Response for the specified request
        /// </returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request);
                if (response != null)
                {
                    responseUri = response.ResponseUri;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message + request.RequestUri);
                throw;
            }

            return response;
        }

        /// <summary>
        ///  Returns the web request object for the specified URI. Overriden here to set the timeout
        /// </summary>
        /// <param name="address">
        ///  URI for the request
        /// </param>
        /// <returns>
        /// webrequest object for the URI
        /// </returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = base.GetWebRequest(address);
            if (result != null)
            {
                result.Timeout = this.timeout;
            }
            
            return result;
        }
    }
}