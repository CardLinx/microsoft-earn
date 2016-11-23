//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The confirm entity result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;

    /// <summary>
    /// The confirm entity result.
    /// </summary>
    public class ConfirmEntityResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public ConfirmStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the entity id
        /// </summary>
        public string EntityId { get; set; }
    }
}