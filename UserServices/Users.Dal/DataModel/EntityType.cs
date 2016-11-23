//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The entity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The entity type.
    /// </summary>
    [DataContract(Name = "EntityType")]
    public enum EntityType 
    {
        /// <summary>
        /// The none.
        /// </summary>
        [EnumMember]
        None = 0, 

        /// <summary>
        /// Maps to the confirmation record for an authenticated user email address
        /// </summary>
        [EnumMember]
        AuthenticatedEmailAddress = 1,

        /// <summary>
        /// Maps to the confirmation record for an unauthenticated user email address
        /// </summary>
        [EnumMember]
        UnAuthenticatedEmailAddress = 2, 

        /// <summary>
        /// The phone number.
        /// </summary>
        [EnumMember]
        PhoneNumber = 3,
        
        /// <summary>
        ///Maps to the account activation for an unauthenticated user
        /// </summary>
        [EnumMember]
        AccountLink = 4
    }
}