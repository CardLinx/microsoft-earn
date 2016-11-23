//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailSubscriptionsBatchContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    /// <summary>
    /// The batch requests continuation context.
    /// </summary>
    internal class EmailSubscriptionsBatchContext : EmailJobsBatchContext
    {
        /// <summary>
        /// Gets or sets the from location id.
        /// </summary>
        internal string FromLocationId { get; set; }
    }
}