//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user service client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    /// <summary>
    /// The user service client.
    /// </summary>
    public class UserServiceClient : IUserServicesClient
    {
        /// <summary>
        /// The service base url.
        /// </summary>
        private readonly Uri serviceBaseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceClient"/> class.
        /// </summary>
        /// <param name="serviceBaseUrl">
        /// The service base url.
        /// </param>
        public UserServiceClient(Uri serviceBaseUrl)
        {
            this.serviceBaseUrl = serviceBaseUrl;
        }

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="correlationId">
        /// The correlation id.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="requestTimeout">
        /// The request timeout.
        /// </param>
        public void SendEmail(Guid correlationId, SendEmailRequest request, TimeSpan? requestTimeout)
        {
            const string SendEmailRelativeAddress = "api/email/send?correlationid={0}";
            var client = new HttpClient();
            string requestRelativePath = string.Format(SendEmailRelativeAddress, correlationId);
            var uri = new Uri(this.serviceBaseUrl, requestRelativePath);
            string requestJson = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> responseTask = client.PostAsync(uri, content);
            if (requestTimeout == null)
            {
                responseTask.Wait();
            }
            else
            {
                if (!responseTask.Wait(requestTimeout.Value))
                {
                    throw new TimeoutException(string.Format("Operation didn't completed within the defined timeout. Timeout={0}", requestTimeout));
                }
            }

            responseTask.Result.EnsureSuccessStatusCode();
        }
    }
}