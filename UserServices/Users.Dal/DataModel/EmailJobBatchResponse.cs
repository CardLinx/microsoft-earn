//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Defines the EmailJobBatchResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the EmailJobBatchResponse type.
    /// </summary>
    public class EmailJobBatchResponse
    {
        /// <summary>
        /// Gets or sets the email subscriptions.
        /// </summary>
        public IList<DataModel.MailingJob> EmailJobs { get; set; }

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