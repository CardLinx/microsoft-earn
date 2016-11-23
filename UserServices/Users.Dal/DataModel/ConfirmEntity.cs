//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The confirm entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal.DataModel
{
    using System;

    /// <summary>
    /// The confirm entity.
    /// </summary>
    public class ConfirmEntity
    {
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public EntityType Type { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}