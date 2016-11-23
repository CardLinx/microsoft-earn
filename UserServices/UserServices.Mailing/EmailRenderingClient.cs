//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The email rendering client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Net.Http;
    using System.Text;

    using Lomo.Logging;

    using LoMo.TransientFaultHandling;

    using Microsoft.Practices.TransientFaultHandling;

    using Newtonsoft.Json;

    /// <summary>
    /// The email rendering client.
    /// </summary>
    public class EmailRenderingClient<T> : IEmailRenderingClient<T>
    {
        public string EmailRenderingServiceUrl { get; set; }

        /// <summary>
        /// Posts a request to the Template service and returns the Html
        /// </summary>
        /// <param name="model">
        /// The model which is the payload for the Post request to template service
        /// </param>
        /// <returns>
        /// Html Content
        /// </returns>
        public string RenderHtml(T model)
        {
            return this.GetRetryPolicy().ExecuteAction(
                delegate
                {
                    var client = new HttpClient();
                    string requestJson = JsonConvert.SerializeObject(model);
                    Log.Verbose(requestJson);
                    HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(EmailRenderingServiceUrl, content).Result;
                    response.EnsureSuccessStatusCode();
                    string html = response.Content.ReadAsStringAsync().Result;

                    return html;
                });
        }

        /// <summary>
        /// Returns the Html by doing a Get request to the Template Service
        /// </summary>
        /// <returns>Html Content</returns>
        public string RenderHtml()
        {
            return this.GetRetryPolicy().ExecuteAction(
               delegate
               {
                   var client = new HttpClient();
                   HttpResponseMessage response = client.GetAsync(EmailRenderingServiceUrl).Result;
                   response.EnsureSuccessStatusCode();
                   string html = response.Content.ReadAsStringAsync().Result;

                   return html;
               });
        }
      
        /// <summary>
        /// The retry policy.
        /// </summary>
        /// <returns>
        /// The <see cref="RetryPolicy"/>.
        /// </returns>
        private RetryPolicy GetRetryPolicy()
        {
            // SQL Azure retry strategy
            var retryStrategy = new Incremental("EmailRenderingClient", 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            var retryPolicy = new RetryPolicy<AllTransientStrategy>(retryStrategy);
            retryPolicy.Retrying += (sender, args) => Log.Verbose("Retry - Count:{0}, Delay:{1}, Exception:{2}", args.CurrentRetryCount, args.Delay, args.LastException);
            return retryPolicy;
        }
    }
}