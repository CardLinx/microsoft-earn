//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailsSubscriptionsBatchResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System.Collections.Generic;

    /// <summary>
    /// The emails subscriptions batch response.
    /// </summary>
    public class EmailsSubscriptionsBatchResponse
    {
        /// <summary>
        /// Gets or sets the email subscriptions.
        /// </summary>
        public IList<DataModel.EmailSubscription> EmailSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the continuation context.
        /// </summary>
        public object ContinuationContext { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has more.
        /// </summary>
        public bool HasMore { get; set; }
    }
}