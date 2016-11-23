//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Security.Cryptography;
using System.Text;

namespace Lomo.Core.Cryptography
{
    public static  class Sha256Hash
    {
        public static string ComputeHash (this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            using (var sha256 = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(value);
                var hashBytes = sha256.ComputeHash(inputBytes);
                //var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                var hashString = Convert.ToBase64String(hashBytes);
                return hashString;
            }
        }
    }
}