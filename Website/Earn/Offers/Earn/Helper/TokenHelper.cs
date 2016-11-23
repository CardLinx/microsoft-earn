//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using DataCollection.IdentityModel;
using Earn.Offers.Earn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Earn.Offers.Earn.Helper
{
    public class TokenHelper
    {
        public static string ConvertPuidToAnid(string puid)
        {
            return AnidIdentityProvider.Instance.DeriveIdentity(string.Format("{0:X16}", puid));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = new SHA1CryptoServiceProvider())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        public static ClaimsPrincipal GetClaimsPrincipal(string anid, string profileName, string authType, string puid)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(puid))
            {
                claims.Add(new Claim(ClaimTypes.Sid, puid));
            }

            if (!string.IsNullOrEmpty(profileName))
            {
                claims.Add(new Claim(ClaimTypes.Name, profileName));
            }

            if (!string.IsNullOrEmpty(anid))
            {
                claims.Add(new Claim(ClaimTypes.Anonymous, anid));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, authType);
            return new ClaimsPrincipal(identity);
        }
    }
}