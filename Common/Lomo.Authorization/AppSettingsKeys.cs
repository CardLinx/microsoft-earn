//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    /// <summary>
    /// Contains keys under which values stored in configuration AppSettings can be found.
    /// </summary>
    public static class AppSettingsKeys
    {
//TODO: The location and / or contents of these values should change to the correct way to store such secrets.
        /// <summary>
        /// The app settings key under which the secure token signing key can be found.
        /// </summary>
        public const string SecureTokenSigningKey = "SecureTokenSigningKey";

        /// <summary>
        /// The app settings key under which the secure token password can be found.
        /// </summary>
        public const string SecureTokenPassword = "SecureTokenPassword";

        /// <summary>
        /// The app settings key under which the secure token salt can be found.
        /// </summary>
        public const string SecureTokenSalt = "SecureTokenSalt";

        /// <summary>
        /// The app settings key under which the secure token lifetime can be found.
        /// </summary>
        public const string SecureTokenLifetime = "SecureTokenLifetime";

        /// <summary>
        /// The app settings key under which the secure token clock skew can be found.
        /// </summary>
        public const string SecureTokenClockSkew = "SecureTokenClockSkew";
    }
}