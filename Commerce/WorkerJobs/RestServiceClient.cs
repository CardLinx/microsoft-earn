//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net;
    using System.Text;
    using Lomo.Authorization;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Client class to invoke a REST service method.
    /// </summary>
    public static class RestServiceClient
    {
        /// <summary>
        /// Calls the described REST service method.
        /// </summary>
        /// <param name="uri">
        /// The URI of the REST service method to call.
        /// </param>
        /// <param name="method">
        /// The HTTP method to use in the call.
        /// </param>
        /// <param name="contentType">
        /// The content type to use in the call.
        /// </param>
        /// <param name="requestBody">
        /// The request body to send in the call, if any.
        /// </param>
        /// <param name="authorizationToken">
        /// The authorization token to include in the request, if any.
        /// </param>
        /// <param name="httpStatusCode">
        /// Receives the HTTP status code returned from the call.
        /// </param>
        /// <returns>
        /// The body of the response, if any.
        /// </returns>
        internal static string CallRestService(Uri uri,
                                               string method,
                                               string contentType,
                                               string requestBody,
                                               string authorizationToken,
                                               out HttpStatusCode httpStatusCode,
                                               string queryString = null)
//TODO: For the queryString parameter, a NameValueCollection or other such data structure would be better than a string.
        {
            string result = null;
            httpStatusCode = HttpStatusCode.NotFound;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(String.Concat(uri.ToString(), queryString));
            httpWebRequest.Method = method;
            if (String.IsNullOrWhiteSpace(contentType) == false)
            {
                httpWebRequest.ContentType = contentType;
            }

            // Add the authorization header, if one was specified.
            if (String.IsNullOrWhiteSpace(authorizationToken) == false)
            {
                httpWebRequest.Headers.Add("Authorization", "SimpleWebToken " + authorizationToken);
            }

            // Add the body, if needed.
            if (String.IsNullOrWhiteSpace(requestBody) == false)
            {
                // Convert specified request object to a byte array.
                byte[] requestBodyBytes = Encoding.UTF8.GetBytes(requestBody);

                // Populate web request body.
                httpWebRequest.ContentLength = requestBodyBytes.Length;
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
                }
            }
            else
            {
                httpWebRequest.ContentLength = 0;
            }

            // Call the endpoint.
            try
            {
                // Send the request and get the response.
                using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
                {
                    httpStatusCode = response.StatusCode;
                    result = ExtractResponseBody(response);
                }
            }
            catch (WebException ex)
            {
                // Process the response from the exception.
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    httpStatusCode = response.StatusCode;
                    result = ExtractResponseBody(response);
                }
                else
                { 
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Obtains an authorization token for the specified resource.
        /// </summary>
        /// <param name="resource">
        /// The resource for which to obtain an authorization token.
        /// </param>
        /// <returns>
        /// The authorization token.
        /// </returns>
        internal static string ObtainAuthorizationToken(string resource)
        {
            string result;

            // Build the fully qualified resource name.
            string environment = String.Concat("commerce-", CommerceWorkerConfig.Instance.Environment);
            string fullyQualifiedResource = String.Format("https://{0}.TODO_INSERT_YOUR_DOMAIN_HERE/api/commerce/service/{1}", environment,
                                                          resource);

            // If the token has already been retrieved and is still valid, use it.
            if (Tokens.ContainsKey(fullyQualifiedResource) == true && Tokens[fullyQualifiedResource].Received + TokenLifetime > DateTime.Now)
            {
                result = Tokens[fullyQualifiedResource].Token;
            }
            // Otherwise, obtain and store a new token.
            else
            {
                ISimpleWebTokenRequestor requestor = PartnerFactory.SimpleWebTokenRequestor();
                result = requestor.RequestToken("Orchestrated Job Queue", CommerceWorkerConfig.Instance.AcsClientCredential,
                                                environment, fullyQualifiedResource);
                Tokens[fullyQualifiedResource] = new SimpleWebToken { Token = result, Received = DateTime.Now };
            }

            return result;
        }

        /// <summary>
        /// Extracts the body from the response.
        /// </summary>
        /// <param name="response">
        /// The response from which to extract the body.
        /// </param>
        /// <returns>
        /// The body of the response, if any.
        /// </returns>
        private static string ExtractResponseBody(HttpWebResponse response)
        {
            string result = null;

            // Get the response body as a string.
            Stream responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    result = streamReader.ReadToEnd();
                }
            }

            return result;
        }

        /// <summary>
        /// The table of resources and their tokens, including expiration dates.
        /// </summary>
        private static ConcurrentDictionary<string, SimpleWebToken> Tokens = new ConcurrentDictionary<string, SimpleWebToken>();

        /// <summary>
        /// The lifetime of the token in seconds.
        /// </summary>
        /// <remarks>
        /// Actual token lifetime is configured as 600 seconds. Smaller value here is overkill, but designed to ensure the token is still valid by the time
        /// all round trips have completed.
        /// </remarks>
        private static TimeSpan TokenLifetime = new TimeSpan(0, 0, 570);
    }
}