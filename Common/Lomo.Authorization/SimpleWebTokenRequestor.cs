//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Provides functionality to request a simple web token for a specified resource.
    /// </summary>
    public class SimpleWebTokenRequestor : ISimpleWebTokenRequestor
    {
        /// <summary>
        /// Initializes a new instance of the SimpleWebTokenRequestor class.
        /// </summary>
        public SimpleWebTokenRequestor()
        {
            TokenProvider = "accesscontrol.windows.net";
        }

        /// <summary>
        /// Requests a token for the specified resource using the specified credentials.
        /// </summary>
        /// <param name="clientName">
        /// The name of the client requesting a token to access the resource.
        /// </param>
        /// <param name="password">
        /// The password for the client requesting a token to access the resource.
        /// </param>
        /// <param name="resourceNamespace">
        /// The namespace to which the resource belongs.
        /// </param>
        /// <param name="resource">
        /// The resource for which an access token is being requested, i.e. the resource URI.
        /// </param>
        /// <returns>
        /// * The requested token, if successful.
        /// * Else returns null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Unable to obtain a token for the requested resource. See inner exception for details.
        /// </exception>
        public string RequestToken(string clientName,
                                   string password,
                                   string resourceNamespace,
                                   string resource)
        {
            string result = null;

            // Create name / value pair properties to hold credentials and the resource for which a token is being requested.
            NameValueCollection values = new NameValueCollection();
            values.Add("wrap_name", clientName);
            values.Add("wrap_password", password);
            values.Add("wrap_scope", resource);

            // Send request to get the token.
            try
            {
                string response;
                using (WebClient webClient = new WebClient())
                {
                    webClient.BaseAddress = string.Format("https://{0}.{1}", resourceNamespace, TokenProvider);
                    response = Encoding.UTF8.GetString(webClient.UploadValues("WRAPv0.9/", "POST", values));
                }

                // Extract the token.
                string[] tokenPairs = response.Split('&');
                string tokenPair = tokenPairs.Single(value => value.StartsWith("wrap_access_token=",
                                                                               StringComparison.OrdinalIgnoreCase));
                result = HttpUtility.UrlDecode(tokenPair.Split('=')[1]);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to obtain a token for the requested resource. See inner exception " +
                                                    " for details.", ex);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the provider service that will issue the token.
        /// </summary>
        private string TokenProvider { get; set; }
    }
}