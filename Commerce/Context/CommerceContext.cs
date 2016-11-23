//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Context
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Lomo.Authorization;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataContracts;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Context for commerce API calls.
    /// </summary>
    public class CommerceContext
    {
        /// <summary>
        /// Initializes a new instance of the CommerceContext class.
        /// </summary>
        public CommerceContext() : this(String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommerceContext class.
        /// If not specified, we assume CommerceService Config.
        /// </summary>
        /// <param name="apiCallDescription">
        /// The call description for the API in which this context object is used.
        /// </param>
        public CommerceContext(string apiCallDescription) : this (apiCallDescription, CommerceServiceConfig.Instance)
        {
        }

        /// <summary>
        /// Creates a new instance for the Commerce Context class.
        /// You can specify the config to use.
        /// </summary>
        /// <param name="apiCallDescription">
        /// The call description for the API in which this context object is used.
        /// </param>
        /// <param name="config">
        /// The config to use to get configuration info from.
        /// </param>
        public CommerceContext(string apiCallDescription,
                               CommerceConfig config)
        {
            ApiCallDescription = apiCallDescription;
            Config = config;
            if (config != null)
            {
                Log = new CommerceLog(Guid.NewGuid(), config.LogVerbosity, General.CommerceLogSource);
                CommerceLog.Config = config;
            }
            PerformanceInformation = new PerformanceInformation();
            Values = new Hashtable();
        }

        /// <summary>
        /// Builds a context for a synchronous API call.
        /// </summary>
        /// <param name="apiCallDescription">
        /// The call description for the API in which this context object being created will be used.
        /// </param>
        /// <param name="request">
        /// The API call request.
        /// </param>
        /// <param name="response">
        /// The API call response.
        /// </param>
        /// <returns>
        /// The CommerceContext built from the passed objects.
        /// </returns>
        public static CommerceContext BuildSynchronousContext(string apiCallDescription,
                                                              object request,
                                                              object response)
        {
            CommerceContext result = new CommerceContext(apiCallDescription);

            result[Key.Request] = request;
            result[Key.Response] = response;

            return result;
        }

        /// <summary>
        /// Builds a context for a synchronous REST API call.
        /// </summary>
        /// <typeparam name="TResponseType">
        /// The type of the response being placed within the context object.
        /// </typeparam>
        /// <param name="apiCallDescription">
        /// The call description for the API in which this context object being created will be used.
        /// </param>
        /// <param name="request">
        /// The API call request.
        /// </param>
        /// <param name="response">
        /// The API call response.
        /// </param>
        /// <param name="callTimer">
        /// The stopwatch with which the API call as a whole is timed.
        /// </param>
        /// <returns>
        /// The CommerceContext built from the passed objects.
        /// </returns>
        public static CommerceContext BuildSynchronousRestContext<TResponseType>(string apiCallDescription,
                                                                                 object request,
                                                                                 TResponseType response,
                                                                                 Stopwatch callTimer)
            where TResponseType : CommerceResponse
        {
            CommerceContext result = BuildSynchronousContext(apiCallDescription, request, response);

            result[Key.CallTimer] = callTimer;
            response.Initialize(result.Log.ActivityId);
            result[Key.ResultSummary] = response.ResultSummary;

            return result;
        }

        /// <summary>
        /// Builds a context for an asynchronous REST API call.
        /// </summary>
        /// <typeparam name="TResponseType">
        /// The type of the response being placed within the context object.
        /// </typeparam>
        /// <param name="apiCallDescription">
        /// The call description for the API in which this context object being created will be used.
        /// </param>
        /// <param name="request">
        /// The API call request.
        /// </param>
        /// <param name="response">
        /// The API call response.
        /// </param>
        /// <param name="callTimer">
        /// The stopwatch with which the API call as a whole is timed.
        /// </param>
        /// <returns>
        /// The CommerceContext built from the passed objects.
        /// </returns>
        public static CommerceContext BuildAsynchronousRestContext<TResponseType>(string apiCallDescription,
                                                                                  object request,
                                                                                  TResponseType response,
                                                                                  Stopwatch callTimer)
            where TResponseType : CommerceResponse
        {
            CommerceContext result = BuildSynchronousRestContext(apiCallDescription, request, response, callTimer);

            TaskCompletionSource<HttpResponseMessage> taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
            result[Key.TaskCompletionSource] = taskCompletionSource;

            return result;
        }

        /// <summary>
        /// Populates the specified context object with the ID of the user making the request.
        /// </summary>
        /// <param name="context">
        /// The context of the current request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter context cannot be null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// An unidentifiable user may not be allowed through the auth layers.
        /// </exception>
        public static Guid PopulateUserId(CommerceContext context)
        {
            Guid result;

            if (context == null)
            {
                throw new ArgumentNullException("context", "Parameter context cannot be null.");
            }

            CustomIdentity userIdentity = Thread.CurrentPrincipal.Identity as CustomIdentity;
            if (userIdentity != null)
            {
                context.Log.Verbose("{0} called by user with ID: {1}", context.ApiCallDescription, userIdentity.UserId);
                result = userIdentity.UserId;
            }
            else
            {
                throw new InvalidOperationException("An unidentifiable user may not be allowed through the auth layers.");
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the call description for the API in which this context object is used.
        /// </summary>
        public string ApiCallDescription { get; private set; }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        public CommerceLog Log { get; private set; }

        /// <summary>
        /// Gets or sets the Commerce Config settings (worker/service etc).
        /// </summary>
        public CommerceConfig Config { get; private set; }

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        public PerformanceInformation PerformanceInformation { get; private set; }

        /// <summary>
        /// Gets or sets connection string suffix. If not null this value gets added to main connection string.
        /// </summary>
        public string ConnectionStringSuffix { get; set; }

        /// <summary>
        /// Gets or sets the value of the context for a given key.
        /// </summary>
        /// <param name="key">
        /// The key for which to get or set a value.
        /// </param>
        /// <returns>
        /// * The value for the given key if successfully retrieved.
        /// * Else returns null.
        /// </returns>
        public Object this[Key key]
        {
            get
            {
                return Values[key];
            }
            set
            {
                Values[key] = value;
            }
        }

        /// <summary>
        /// Determines whether the Values hash table contains the specified key.
        /// </summary>
        /// <param name="key">
        /// The key whose existence to check.
        /// </param>
        /// <returns>
        /// * True if the key exists within the Values hash table.
        /// * Else returns false.
        /// </returns>
        public bool ContainsKey(Key key)
        {
            return Values.ContainsKey(key);
        }

        /// <summary>
        /// Gets or sets the hashtable in which context values are stored.
        /// </summary>
        private Hashtable Values { get; set; }
    }
}