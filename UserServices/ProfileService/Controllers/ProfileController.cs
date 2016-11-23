//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Profile Controller. Has a single Get method to fetch the user profile info from the EFS Service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProfileService.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Web.Http;

    using ProfileService.DataContract;
    using ProfileProxy;

    /// <summary>
    /// The Profile Controller. Has a single Get method to fetch the user profile info from the EFS Service
    /// </summary>
    public class ProfileController : ApiController
    {
        /// <summary>
        /// Returns the user profile info from the EFS Service 
        /// </summary>
        /// <param name="userAnid">
        /// Encrypted Anid of the user
        /// </param>
        /// <returns>
        /// UserInfo for the Anid
        /// </returns>
        public UserDemographics GetUserProfile(string userAnid)
        {
            Trace.WriteLine(string.Format("Received ProfileInfo request for Encrypted Anid : {0}", userAnid));
            UserDemographics userProfile = null;

            if (!string.IsNullOrEmpty(userAnid))
            {
                try
                {
                    userProfile = Proxy.GetUserProfile(userAnid);
                    Trace.WriteLine(string.Format("Finished Processing ProfileInfo request for Anid : {0}", userAnid));
                }
                catch (CryptographicException)
                {
                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid key...Unbale to decrypt the anid"));
                }
                catch (Exception exception)
                {
                    Trace.TraceError(exception.Message,exception);
                }
            }
            else
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Empty useranid passed"));
            }

            return userProfile;
        }
    }
}