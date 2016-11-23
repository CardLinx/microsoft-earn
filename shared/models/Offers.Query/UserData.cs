//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.DataModels.Offers.Query
{
    using System;
    using ProfileService.DataContract;

    /// <summary>
    /// Encapsulates the user identifiers.
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserData"/> class.
        /// </summary>
        /// <param name="anid">The anid.</param>
        /// <param name="muid">The muid.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="upanid">The upanid.</param>
        /// <param name="demographics">The user demographics</param>
        /// <param name="sessionId">The session id</param>
        public UserData(string anid = null, string muid = null, string ip = null, string upanid = null, UserDemographics demographics = null, Guid? sessionId = null)
        {
            Anid = anid;
            Muid = muid;
            Ip = ip;
            Upanid = upanid;
            Demographics = demographics;
            SessionId = sessionId;
        }

        /// <summary>
        /// Gets the user anid.
        /// </summary>
        public string Anid { get; private set; }

        /// <summary>
        /// Gets the user muid.
        /// </summary>
        public string Muid { get; private set; }

        /// <summary>
        /// Gets the requestor computer ip
        /// </summary>
        public string Ip { get; private set; }

        /// <summary>
        /// Gets the user fbid.
        /// </summary>
        public string Upanid { get; private set; }

        /// <summary>
        /// Gets the user demographic profile.
        /// </summary>
        public UserDemographics Demographics { get; private set; }

        /// <summary>
        /// Gets the current session id
        /// </summary>
        public Guid? SessionId { get; private set; }
    }
}