//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    /// <summary>
    /// Represents the types of entities which can be paid rewards.
    /// </summary>
    public enum PayeeType
    {
        /// <summary>
        /// Indicates the payee is a User.
        /// </summary>
        User = 0,

        /// <summary>
        /// Indicates the payee is a Merchant.
        /// </summary>
        Merchant = 1
    }
}