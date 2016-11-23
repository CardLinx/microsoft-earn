//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user id type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    /// <summary>
    /// The user id type.
    /// </summary>
    public enum UserIdType
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The anid.
        /// </summary>
        Anid = 1,

        /// <summary>
        /// The muid.
        /// </summary>
        Muid = 2,

        /// <summary>
        /// The fb.
        /// </summary>
        Upanid = 3,

        /// <summary>
        /// The ip.
        /// </summary>
        Ip = 4,
    }

    /// <summary>
    /// The user id info.
    /// </summary>
    public class UserIdInfo
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public UserIdType Type { get; set; }
    }
}