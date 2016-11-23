//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal.DataModel
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The location type.
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0, 

        /// <summary>
        /// The postal.
        /// </summary>
        Postal = 1, 

        /// <summary>
        /// The National location type
        /// </summary>
        National = 2,

        /// <summary>
        /// The City Location Type
        /// </summary>
        City = 3,
    }
}