//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts.Extensions
{
    using System;
    using System.Net;

    /// <summary>
    /// Extension methods for the RequestInformationExtensions class.
    /// </summary>
    public static class RequestInformationExtensions
    {
        /// <summary>
        /// Initializes the specified RequestInformation object.
        /// </summary>
        /// <param name="requestInformation">
        /// The RequestInformation object to initialize.
        /// </param>
        /// <param name="requestId">
        /// The ID of the current request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter requestInformation cannot be null.
        /// </exception>
        public static void Initialize(this RequestInformation requestInformation,
                                      Guid requestId)
        {
            if (requestInformation == null)
            {
                throw new ArgumentNullException("requestInformation", "Parameter requestInformation cannot be null.");
            }

            // Set request ID and server ID.
            requestInformation.RequestId = requestId;
            requestInformation.ServerId = ServerId;
        }

        /// <summary>
        /// Gets or sets an identifier for the current server.
        /// </summary>
        /// <remarks>
        /// * By default, the server ID is the last three characters in the host name.
        /// * This value can be changed for environments where a partial server name cannot adequately identify the server.
        /// </remarks>
        public static string ServerId
        {
            get
            {
                if (serverId == null)
                {
                    serverId = "Unknown";
                    string dnsHostName = Dns.GetHostName();
                    if (dnsHostName.Length > ServerIdLength)
                    {
                        serverId = dnsHostName.Substring(dnsHostName.Length - ServerIdLength, ServerIdLength);
                    }
                }

                return serverId;
            }
            set
            {
                serverId = value;
            }
        }
        private static string serverId;

        /// <summary>
        /// The length of the server ID.
        /// </summary>
        private const int ServerIdLength = 3;
    }
}