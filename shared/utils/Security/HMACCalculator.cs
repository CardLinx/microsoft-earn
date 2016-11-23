//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HolMon.Security
{
    using System.Security.Cryptography;

    using Microsoft.HolMon.Common.Utils;

    /// <summary>
    /// Simple wrapper class over HMACSHA256 class.
    /// </summary>
    public class HMACCalculator : IDisposable
    {
        readonly HMACSHA256 hmac;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Base64 encoded key.</param>
        public HMACCalculator(string key)
        {
            this.hmac = new HMACSHA256(Convert.FromBase64String(key));
        }

        public string ComputeHash(string text)
        {
            byte[] hash = this.hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BytesHexConverter.ToHexString(hash);
        }

        public void Dispose()
        {
            this.hmac.Dispose();
        }
    }
}