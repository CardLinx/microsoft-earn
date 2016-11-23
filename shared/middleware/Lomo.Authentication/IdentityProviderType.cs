//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authentication
{
    /// <summary>
    /// Types of Identity providers supported by Lomo Authentication
    /// </summary>
    public enum IdentityProviderType
    {
        /// <summary>
        /// Default or unknown identity provider.
        /// </summary>
        None = 0,

        /// <summary>
        /// Microsoft Account identity provider.
        /// </summary>
        MicrosoftAccount = 1,

        /// <summary>
        /// Facebook identity provider.
        /// </summary>
        Facebook = 2
    }
}