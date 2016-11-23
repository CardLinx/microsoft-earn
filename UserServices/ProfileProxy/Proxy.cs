//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Proxy class for the EFS service in PHX to fetch the user profile
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProfileProxy
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Azure;
    using ProfileService.DataContract;
    using ProfileService.Utility;

    /// <summary>
    /// Proxy class for the EFS service in PHX to fetch the user profile
    /// </summary>
    public static class Proxy
    {
        /// <summary>
        /// Key to decrypt the user Anid
        /// </summary>
        private static readonly string AnidPassphrase = string.Empty;
        
        static Proxy()
        {
            AnidPassphrase = CloudConfigurationManager.GetSetting("AnidPassphrase");
        }

        /// <summary>
        ///  Returns the user profile from the EFS service for the Anid
        /// </summary>
        /// <param name="userAnid">
        ///  Encrypted Anid
        /// </param>
        /// <returns>
        ///  User profile info for the Anid
        /// </returns>
        public static UserDemographics GetUserProfile(string userAnid)
        {
            UserDemographics userProfile = null;
            string decryptedAnid = Cipher.Decrypt(userAnid, AnidPassphrase);
            Trace.WriteLine(string.Format("Decrypted Anid : {0}",decryptedAnid));

            var client = new EfsService.UserInfoServiceClient();
            var lst = new List<KeyValuePair<EfsService.UserIdType, string>>
                {
                    new KeyValuePair<EfsService.UserIdType, string>(EfsService.UserIdType.Anid, decryptedAnid)
                };

            Trace.WriteLine(string.Format("Calling EFS Service to get Profile info for Anid : {0}", decryptedAnid));
            EfsService.UserDemographics result = client.GetDemographics(lst.ToArray());
            if (result != null)
            {
                Trace.WriteLine(string.Format("Successfully retrieved Profile info for Anid : {0}", decryptedAnid));
                userProfile = new UserDemographics { Gender = result.Gender.ToString(), Age = result.Age };
                Trace.WriteLine("Constructed UserProfile Object");
                Trace.WriteLine(string.Format("Calling EFS Service to get opt out info for Anid : {0}", decryptedAnid));
                var pref = new EfsService.UserPreferenceServiceClient();
                bool? optout = pref.GetOptOutState(decryptedAnid, EfsService.UserIdType.Anid);
                userProfile.OptOut = optout.GetValueOrDefault(false);
            }
            else
            {
                Trace.WriteLine(string.Format("Profile not found fsor Anid : {0}", decryptedAnid));
            }

            return userProfile;
        }
    }
}