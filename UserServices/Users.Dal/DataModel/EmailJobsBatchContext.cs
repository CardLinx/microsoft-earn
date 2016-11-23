//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailJobsBatchContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;

    /// <summary>
    /// Defines the EmailJobsBatchContext.
    /// </summary>
    internal class EmailJobsBatchContext
    {
        /// <summary>
        /// Gets or sets the partition index.
        /// </summary>
        internal int PartitionIndex { get; set; }

        /// <summary>
        /// Gets or sets the from partition id.
        /// </summary>
        internal int? FromPartitionId { get; set; }

        /// <summary>
        /// Gets or sets the from user id.
        /// </summary>
        internal Guid? FromUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has more.
        /// </summary>
        internal bool HasMore { get; set; }
    }
}