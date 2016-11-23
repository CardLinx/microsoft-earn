//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deals server client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DealsServerClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using DealsServerClient.Exceptions;

    using DotM.DataContracts;

    using Lomo.Logging;

    using Newtonsoft.Json;

    /// <summary>
    /// The deals server client.
    /// </summary>
    public class DealsClient : IDealsClient
    {
        #region Fields

        /// <summary>
        /// The url builder.
        /// </summary>
        private readonly DealsUrlBuilder urlBuilder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsClient"/> class.
        /// </summary>
        /// <param name="dealsServerBasePath">
        /// The deals server base path.
        /// </param>
        /// <param name="clientName">
        /// The client name.
        /// </param>
        /// <exception cref="ArgumentException">dealsServerBasePath or clientName are null or empty </exception>
        public DealsClient(Uri dealsServerBasePath, string clientName)
        {
            if (dealsServerBasePath == null)
            {
                throw new ArgumentNullException("dealsServerBasePath");
            }

            if (string.IsNullOrEmpty(clientName))
            {
                throw new ArgumentException("clientName");
            }

            this.urlBuilder = new DealsUrlBuilder(dealsServerBasePath, clientName);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a set of deals based on their business ids.
        /// </summary>
        /// <count>Max number of deals to return.</count>
        /// <param name="businessIds">
        /// The business Ids. 
        /// </param>
        /// <param name="maxDealsPerBusiness">
        /// The max Deals Per Business. 
        /// </param>
        /// <param name="sort">
        /// The sort order. <seealso cref="DealsSort"/> 
        /// </param>
        /// <param name="expiresAfter">
        /// The expires After. 
        /// </param>
        /// <param name="activeBefore">
        /// The active Before. 
        /// </param>
        /// <returns>
        /// The deals corresponding to the input ids. 
        /// </returns>
        public Task<IEnumerable<Deal>> GetDealsByBusiness(IList<string> businessIds, int? maxDealsPerBusiness = null, DealsSort? sort = null, DateTime? expiresAfter = null, DateTime? activeBefore = null)
        {
            if (businessIds == null || !businessIds.Any())
            {
                throw new ArgumentException("BusinessIds list must contain elements", "businessIds");
            }

            Uri methodUri = this.urlBuilder.GetDealsByBusinessUri(businessIds, maxDealsPerBusiness, sort, expiresAfter, activeBefore);
            return this.GetDealsAsync(methodUri);
        }

        /// <summary>
        /// Returns list of deals with the input id(s), or null if no such deals exist. 
        /// </summary>
        /// <param name="dealIds"> The deal ids. </param>
        /// <param name="count">max result size.</param>
        /// <param name="refinements">The refinements. <seealso cref="Refinements"/></param>
        /// <param name="format">The format of the response: "simple", "all",...</param>
        /// <returns> The <see cref="IEnumerable{Deal}"/>. </returns>
        /// <exception cref="ArgumentException"> Deal ids are null or empty</exception>
        public Task<IEnumerable<Deal>> GetDealsById(IList<Guid> dealIds, int? count = null, Refinements refinements = null, string format = "default")
        {
            if (dealIds == null || !dealIds.Any())
            {
                throw new ArgumentException("Deal Ids list must contain elements", "dealIds");
            }

            Uri methodUri = this.urlBuilder.GetDealsById(dealIds, count, refinements, format);
            return this.GetDealsAsync(methodUri);
        }

        /// <summary>
        /// Returns list of deals nearby to the input location. Asyc execution
        /// </summary>
        /// <param name="coordinates">
        /// coordinate (required) as latitude and longitude
        /// </param>
        /// <param name="radius">
        /// meters - (optional). Default is 1000
        /// </param>
        /// <param name="count">
        /// max result size - (optional). Default is 50
        /// </param>
        /// <param name="refinements">
        /// The refinements. <seealso cref="Refinements"/> 
        /// </param>
        /// <returns>list of nearby deals </returns>
        public Task<IEnumerable<Deal>> GetNearbyDeals(Coordinates coordinates, double? radius = null, int? count = null, Refinements refinements = null)
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException("coordinates");
            }

            Uri methodUri = this.urlBuilder.GetNearByDealsUri(coordinates, radius, count, refinements);
            return this.GetDealsAsync(methodUri);
        }

        /// <summary>
        /// Get deals by region to the input location. Asyc execution
        /// </summary>
        /// <param name="regionCode">the region code</param>
        /// <param name="coordinates">
        /// coordinate (optional) as latitude and longitude
        /// </param>
        /// <param name="radius">
        /// meters - (optional). Default is 1000
        /// </param>
        /// <param name="count">
        /// max result size - (optional). Default is 50
        /// </param>
        /// <param name="refinements">
        /// The refinements. <seealso cref="Refinements"/> 
        /// </param>
        /// <param name="anid">
        /// Anid of the user
        /// </param>
        /// <returns>list of deals </returns>
        public Task<IEnumerable<Deal>> GetDealsByRegion(string regionCode, Coordinates coordinates, double? radius = null, int? count = null, Refinements refinements = null,string anid = null)
        {
            Uri methodUri = this.urlBuilder.GetDealsByRegionUri(regionCode, coordinates, radius, count, refinements, anid);
            return this.GetDealsAsync(methodUri);
        }

        /// <summary>
        ///  list of online deals independent of user location. Asyc execution
        /// </summary>
        /// <param name="count">max result size - (optional). Default is 50</param>
        /// <param name="refinements">
        /// The refinements. <seealso cref="Refinements"/> 
        /// </param>
        /// <returns>list of online deals </returns>
        public Task<IEnumerable<Deal>> GetOnlineDeals(int? count = null, Refinements refinements = null)
        {
            Uri methodUri = this.urlBuilder.GetOnlineDealsUri(count, refinements);
            return this.GetDealsAsync(methodUri);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get deals async.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private Task<IEnumerable<Deal>> GetDealsAsync(Uri uri)
        {
            Log.Verbose("Starting Get Deals. Uri={0}", uri);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient.GetAsync(uri).ContinueWith(httpResponseTask => this.GetDealsCallback(httpResponseTask, uri)).Unwrap();
        }

        /// <summary>
        /// The get deals callback.
        /// </summary>
        /// <param name="httpResponseTask">
        /// The http response task.
        /// </param>
        /// <param name="requestUri">
        /// The request uri.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="DealsClientCommunicationException"> Error communicating with the deals server </exception>
        private Task<IEnumerable<Deal>> GetDealsCallback(Task<HttpResponseMessage> httpResponseTask, Uri requestUri)
        {
            if (httpResponseTask.Exception != null)
            {
                throw new DealsClientCommunicationException(string.Format("Error Getting Deals. Request Uri: {0}", requestUri), httpResponseTask.Exception);
            }

            if (!httpResponseTask.Result.IsSuccessStatusCode)
            {
                throw new DealsClientCommunicationException(string.Format("Bad Response Status returned. State Code: {0}; Message: {1}; Request Uri: {2}", httpResponseTask.Result.StatusCode, httpResponseTask.Result.ReasonPhrase, requestUri));
            }

            return httpResponseTask.Result.Content.ReadAsStringAsync().ContinueWith(contentTask => this.ReadDealsCallback(contentTask, requestUri), TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// The read deals callback.
        /// </summary>
        /// <param name="contentTask">
        /// The content task.
        /// </param>
        /// <param name="requestUri">
        /// The request uri.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Deal}"/>.
        /// </returns>
        /// <exception cref="DealsClientCommunicationException"> Error communicating with the deals server </exception>
        /// <exception cref="DealsClientSerializationException"> Error deserializing the json response with the deals server </exception>
        private IEnumerable<Deal> ReadDealsCallback(Task<string> contentTask, Uri requestUri)
        {
            IEnumerable<Deal> deals;
            if (contentTask.Exception != null)
            {
                if (contentTask.Exception.InnerException is DealsClientException)
                {
                    throw contentTask.Exception;
                }

                throw new DealsClientCommunicationException(string.Format("Error Getting Deals Content. Request Uri: {0}", requestUri), contentTask.Exception);
            }

            if (string.IsNullOrEmpty(contentTask.Result))
            {
                deals = new List<Deal>();
            }
            else
            {
                try
                {
                    deals = JsonConvert.DeserializeObject<IEnumerable<Deal>>(contentTask.Result);
                }
                catch (Exception exception)
                {
                    throw new DealsClientSerializationException(string.Format("Error De-Serializing the json response. Request Uri: {0}", requestUri), exception);
                }
            }

            int dealsCount = 0;
            if (deals != null && deals.Any())
            {
                dealsCount = deals.Count();
            }

            Log.Verbose("Get Deals Completed. Original Uri={0}. Deals Count={1}", requestUri, dealsCount);

            return deals;
        }

        #endregion
    }
}