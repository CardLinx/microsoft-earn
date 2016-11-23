//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace AnalyticsClient
{
    using System.Security.Cryptography;
    using System.Text;

    using DataCollection.IdentityModel;

    /// <summary>
    /// The analytics user id.
    /// </summary>
    public static class AnalyticsUserId
    {
        /// <summary>
        /// The user ANID type.
        /// </summary>
        private const string UserIdAnidType = "ANID";

        /// <summary>
        /// The user id IP type.
        /// </summary>
        private const string UserIdIpType = "IP";

        /// <summary>
        /// The user MUID type.
        /// </summary>
        private const string UserIdMuidType = "MUID";

        /// <summary>
        /// The user id fb type.
        /// </summary>
        private const string UserIdFbType = "UPANID";

        /// <summary>
        /// The get user id from fb.
        /// </summary>
        /// <param name="upanid">
        /// The fb id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetUserIdFromUpanid(string upanid)
        {
            return GetUserId(UserIdFbType, upanid);
        }

        /// <summary>
        /// Gets user from anid
        /// </summary>
        /// <param name="anid">
        /// the anid
        /// </param>
        /// <returns>
        /// the user
        /// </returns>
        public static string GetUserIdFromAnid(string anid)
        {
            return GetUserId(UserIdAnidType, anid);
        }

        /// <summary>
        /// Converts ms id (puid) to anid and return anid user id
        /// </summary>
        /// <param name="msId"></param>
        /// <returns></returns>
        public static string GetAnidUserIdFromMsId(string msId)
        {
            if (string.IsNullOrWhiteSpace(msId))
            {
                return null;
            }
            return GetUserId(UserIdAnidType, AnidIdentityProvider.Instance.DeriveIdentity(msId.ToUpperInvariant()));
        }

        /// <summary>
        /// Gets user from ip
        /// </summary>
        /// <param name="ip">
        /// the ip
        /// </param>
        /// <returns>
        /// the user
        /// </returns>
        public static string GetUserIdFromIp(string ip)
        {
            return GetUserId(UserIdIpType, GetSecureHashString(ip));
        }

        /// <summary>
        /// Gets the user from muid
        /// </summary>
        /// <param name="muid">
        /// the muid
        /// </param>
        /// <returns>
        /// the user
        /// </returns>
        public static string GetUserIdFromMuid(string muid)
        {
            return GetUserId(UserIdMuidType, muid);
        }

        private static string GetUserId(string typeStr, string value)
        {
            return string.Format("{0}:{1}", typeStr, value);
        }

        /// <summary>
        /// The get secure hash string.
        /// </summary>
        /// <param name="unhashedString">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetSecureHashString(string unhashedString)
        {
            if (string.IsNullOrEmpty(unhashedString))
            {
                return null;
            }

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sb = new StringBuilder();

            using (SHA256 sha256Hasher = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hasher.ComputeHash(Encoding.Default.GetBytes(unhashedString));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                foreach (byte t in data)
                {
                    sb.Append(t.ToString("x2"));
                }
            }

            // Return the hexadecimal string.
            return sb.ToString();
        }
    }
}

