//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The confirm status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The confirm status.
    /// </summary>
    [DataContract(Name = "ConfirmStatus")]
    public enum ConfirmStatus
    {
        /// <summary>
        /// The none.
        /// </summary>
        [EnumMember]
        None = 0, 

        /// <summary>
        /// The code confirmed.
        /// </summary>
        [EnumMember]
        CodeConfirmed = 1, 

        /// <summary>
        /// The code wrong.
        /// </summary>
        [EnumMember]
        CodeWrong = 2, 

        /// <summary>
        /// The invalid.
        /// </summary>
        [EnumMember]
        Invalid = 3,

        /// <summary>
        /// The invalid.
        /// </summary>
        [EnumMember]
        CodeNotFound = 4
    }
}