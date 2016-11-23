//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    /// <summary>
    /// Contains possible authentication vectors for an account.
    /// </summary>
    public enum AuthenticationVector
    {
        /// <summary>
        /// Indicates the authentication vector for the account is only the email address.
        /// </summary>
        Email,

        /// <summary>
        /// Indicates the authentication vector for the account is a Microsoft account.
        /// </summary>
        MicrosoftAccount,

        /// <summary>
        /// Indicates the authentication vector for the account is a Facebook account.
        /// </summary>
        Facebook
    }
}