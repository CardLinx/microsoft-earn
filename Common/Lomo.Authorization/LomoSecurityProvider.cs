//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The lomo security provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace Lomo.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Text;
    using System.Linq;
    using Lomo.Authentication;
    using Lomo.Logging;
    using Users.Dal;
    using Users.Dal.DataModel;

    /// <summary>
    /// The lomo security provider.
    /// </summary>
    public class LomoSecurityProvider : ISecurityProvider
    {
        /// <summary>
        /// The users dal.
        /// </summary>
        private readonly IUsersDal usersDal;

        private const string SecurityProvider = "lomo";

        private readonly bool saveUserName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LomoSecurityProvider"/> class.
        /// </summary>
        /// <param name="usersDal">
        /// The users dal.
        /// </param>
        /// <param name="saveUserName"> store user name, when true </param>
        /// <param name="bypassTokenValidation"> hook for test environment, when true </param>
        public LomoSecurityProvider(IUsersDal usersDal, bool saveUserName = false, bool bypassTokenValidation = false)
        {
            this.usersDal = usersDal;
            this.saveUserName = saveUserName;
        }

        #region Public Methods and Operators

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="flags">A general purpose flags set </param>
        /// <returns>
        /// The <see cref="AuthPayload"/>.
        /// </returns>
        public AuthPayload Authenticate(SecurityCredentials credentials, HashSet<string> flags = null)
        {
            bool useExternalIdentity = flags != null && flags.Contains("UseExternalIdentity");

            if (credentials == null)
            {
                // || string.IsNullOrWhiteSpace(credentials.IdentificationCode))
                throw new Exception("The LomoSecurityProvider.Authenticate() method was called with invalid Credentials.");
            }

            var externalIdentityInfo = Authentication.BingSocialAccessorRepository.GetUserAccountInfo(credentials.Token);

            if (externalIdentityInfo != null && externalIdentityInfo.UserId != null)
            {
                User user = null;
                if (!useExternalIdentity)
                {
                    user = this.CreateOrGetInternalUser(externalIdentityInfo);
                }

                Guid userId = default(Guid);
                var userName = externalIdentityInfo.Name;
                var emailAddress = externalIdentityInfo.UserEmail;
                if (user != null)
                {
                    userId = user.Id;
                    if (string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(user.Name))
                    {
                        userName = user.Name;
                    }

                    emailAddress = user.Email;
                }

                var identity = new CustomIdentity(userId, userName, SecurityProvider, externalIdentityInfo.UserId)
                                   {
                                       EmailAddress = emailAddress
                                   };

                var payload = new AuthPayload(identity);
                return payload;
            }

            return null;
        }

        /// <summary>
        /// The is authorized.
        /// </summary>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <param name="roles">
        /// The roles.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsAuthorized(AuthPayload payload, string[] roles)
        {
            return payload != null; // TODO
        }

        /// <summary>
        /// The create or get internal user.
        /// </summary>
        /// <param name="externalIdentityInfo">
        /// The external identity info.
        /// </param>
        /// <returns>
        /// The <see cref="AuthPayload"/>.
        /// </returns>
        private User CreateOrGetInternalUser(LomoUserAccountInfo externalIdentityInfo)
        {
            string userSource = this.GetSource();
            UserLocation userLocation = this.GetUserLocation();
            User user = this.usersDal.CreateOrGetUserByMsId(externalIdentityInfo.UserId, userSource, saveUserName ? externalIdentityInfo.Name : null, userLocation: userLocation);

            if (string.IsNullOrWhiteSpace(user.Email) && !string.IsNullOrWhiteSpace(externalIdentityInfo.UserEmail))
            {
                try
                {
                    // The email address was taken from the user identity token, this is a confirmed email address
                    user = this.usersDal.UpdateUserEmail(user.Id, externalIdentityInfo.UserEmail, true);
                }
                catch (UserUpdateConflictException updateUserConfilct)
                {
                    // We can't assign the email since that it is already in use. Hadle the error and don't propogate it
                    Log.Warn(updateUserConfilct.Message);
                }
            }

            return user;
        }

        /// <summary>
        /// The get source.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetSource()
        {
            var sb = new StringBuilder();
            if (HttpContext.Current != null)
            {
                // The cookie "bo_referrer" has the publisher info that drove the user sign up to bing offers. 
                // Add this publisher info to the user's source column in the db for new user
                var cookie = HttpContext.Current.Request.Cookies["bor"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    sb.Append(cookie.Value);
                    Log.Info(string.Format("bor cookie is : {0}", cookie.Value));
                }
                else
                {
                    Log.Info("bor Cookie is null");
                }

                cookie = HttpContext.Current.Request.Cookies["bof"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    sb.Append("#");
                    sb.Append(cookie.Value);
                    Log.Info(string.Format("bof cookie is : {0}", cookie.Value));
                }
                else
                {
                    Log.Info("bof Cookie is null");
                }
            }
            else
            {
                Log.Info("HttpContext is null");
            }

            string userSource = sb.Length > 0 ? sb.ToString() : null;

            return userSource;
        }

        /// <summary>
        /// Returns the userlocation by parsing the values from Bing FD Header
        /// </summary>
        /// <returns></returns>
        private UserLocation GetUserLocation()
        {
            UserLocation userLocation = null;
            if (HttpContext.Current != null)
            {
                string latitudeStr;
                string longitudeStr;
                double lat;
                double lng;
                IDictionary<string, string> frontDoorValues;
                string locationHeader = HttpContext.Current.Request.Headers["X-FD-Location"];
                if (!string.IsNullOrEmpty(locationHeader))
                {
                    frontDoorValues = ParseFDValues(locationHeader, '|');
                    if (frontDoorValues.TryGetValue("lat", out latitudeStr) && double.TryParse(latitudeStr, out lat)
                        && frontDoorValues.TryGetValue("long", out longitudeStr) && double.TryParse(longitudeStr, out lng))
                    {
                        userLocation = new UserLocation()
                        {
                            Latitude = lat,
                            Longitude = lng
                        };
                    }
                }

                string revIp = HttpContext.Current.Request.Headers["X-FD-RevIP"];
                if (!string.IsNullOrEmpty(revIp))
                {
                    frontDoorValues = ParseFDValues(revIp, ',');
                    if (userLocation == null)
                    {
                        if (frontDoorValues.TryGetValue("lat", out latitudeStr) && double.TryParse(latitudeStr, out lat)
                        && frontDoorValues.TryGetValue("long", out longitudeStr) && double.TryParse(longitudeStr, out lng))
                        {
                            userLocation = new UserLocation()
                            {
                                Latitude = lat,
                                Longitude = lng
                            };
                        }
                    }
                    if (userLocation != null)
                    {
                        string zip, dma, iso;
                        if (frontDoorValues.TryGetValue("zip", out zip))
                        {
                            userLocation.Zipcode = zip;
                        }
                        if (frontDoorValues.TryGetValue("dma", out dma))
                        {
                            userLocation.Dma = dma;
                        }
                        if (frontDoorValues.TryGetValue("iso", out iso))
                        {
                            userLocation.Iso = iso;
                        }
                    }
                }
            }

            return userLocation;
        }

        /// <summary>
        /// Parses the Bing FD Header
        /// </summary>
        /// <param name="fdValues">FD Header value to parse</param>
        /// <param name="splitChar">Delimiter character for each value within a FD header</param>
        /// <returns>FD Header name and its value</returns>
        private IDictionary<string, string> ParseFDValues(string fdValues, char splitChar)
        {
            var ar = fdValues.Split(new[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);
            IDictionary<string, string> propertyBag =
            ar.Select(elem => elem.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)).
                Where(array => array.Length == 2).
                ToDictionary(array => array[0], array => array[1]);

            return propertyBag;
        }

        #endregion
    }
}