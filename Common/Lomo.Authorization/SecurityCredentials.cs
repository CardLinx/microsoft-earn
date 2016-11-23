//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    /// <summary>
    /// The security credentials.
    /// </summary>
    public class SecurityCredentials
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the identification code.
        /// </summary>
        public string IdentificationCode { get; set; }

        /// <summary>
        /// Gets or sets the security provider name.
        /// </summary>
        public string SecurityProviderName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

    }
}