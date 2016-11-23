//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Defines Logging api controller for client.
    /// </summary>
    public class LogController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the LogController class.
        /// </summary>
        public LogController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogController class.
        /// </summary>
        /// <param name="request">
        /// The request to process within this instance.
        /// </param>
        internal LogController(HttpRequestMessage request)
        {
            Request = request;
        }

        /// <summary>
        /// Log errors
        /// </summary>
        /// <returns>Http response</returns>
        [HttpPost]
        public HttpResponseMessage Error()
        {
            HttpResponseMessage result = null;

            try
            {
                Request.Content.ReadAsStringAsync().ContinueWith(r =>
                {
                    JObject content = JsonConvert.DeserializeObject(r.Result) as JObject;

                    if (content != null)
                    {
                        if (CommerceClientLogger.IsAvailable)
                        {
                            // Log this as an error unless it's InvalidCard. Users can directly trigger that and there's no need for a SCOM alert when this happens.
                            string details = content["details"].Value<string>();
                            if (details.Contains("InvalidCard") == false)
                            {
                                CommerceClientLogger.Instance.Error("UA: {0} \n\r Details: {1}", null, content["context"].Value<string>(), details);
                            }
                            else
                            {
                                CommerceClientLogger.Instance.Warning("UA: {0} \n\r Details: {1}", content["context"].Value<string>(), details);
                            }
                        }
                    }
                });

                result = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}