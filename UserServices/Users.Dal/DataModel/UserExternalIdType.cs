//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The external id type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal.DataModel
{
    /// <summary>
    /// The external id type.
    /// </summary>
    public enum UserExternalIdType
    {
        /// <summary>
        /// External id is the users microsoft id
        /// </summary>
        MsId = 0,

        /// <summary>
        /// The email.
        /// </summary>
        Email = 1,   
    }
}