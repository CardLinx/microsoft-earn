//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Security
{
    using System;
    using System.Collections.Generic;

    using global::Common.Utils;

    using Microsoft.HolMon.Common;

    public class KeyPair : IEquatable<KeyPair>
    {
        public KeyPair(string primaryKey, string secondaryKey)
        {
            this.PrimaryKey = primaryKey;
            this.SecondaryKey = secondaryKey;
        }

        public string PrimaryKey { get; private set; }

        public string SecondaryKey { get; private set; }

        public IEnumerable<string> Keys
        {
            get
            {
                yield return this.PrimaryKey;
                yield return this.SecondaryKey;
            }
        }

        /// <summary>
        /// Retrieves the primary key as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] PrimaryKeyToByteArray()
        {
            return Convert.FromBase64String(this.PrimaryKey);
        }

        /// <summary>
        /// Retrieves the secondary key as byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] SecondaryKeyToByteArray()
        {
            return Convert.FromBase64String(this.SecondaryKey);
        }

        /// <summary>
        /// Checks whether the specified key is equal to
        /// one of the pairs.
        /// </summary>
        /// <param name="key">The key to compare against.</param>
        /// <returns>
        /// True if the specified key is same as the primary key or secondary key
        /// of the current instance, false otherwise.
        /// </returns>
        public bool IsOneAmongThePair(string key)
        {
            return this.PrimaryKey.SecureCompare(key)
                || this.SecondaryKey.SecureCompare(key);
        }

        /// <summary>
        /// Compares the specified KeyPair with the current instance.
        /// </summary>
        /// <param name="other">The KeyPair instance to compare against.</param>
        /// <returns>
        /// True if the KeyPairs are equal, false otherwise.
        /// </returns>
        public bool Equals(KeyPair other)
        {
            return other != null &&
                this.PrimaryKey.SecureCompare(other.PrimaryKey) &&
                this.SecondaryKey.SecureCompare(other.SecondaryKey);
        }

        /// <summary>
        /// Compares the specified object with the current instance.
        /// </summary>
        /// <param name="obj">The object instance to compare against.</param>
        /// <returns>
        /// True if the KeyPairs are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            return EquatableHelper<KeyPair>.AreEqual(this, obj);
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeCalculator.Calculate(
                this.PrimaryKey.GetNullAwareHashCode(),
                this.SecondaryKey.GetNullAwareHashCode());
        }
    }
}