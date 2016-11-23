//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerJobs
{
    using System;

    /// <summary>
    /// Contains a simple web token and its creation date.
    /// </summary>
    internal class SimpleWebToken
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        internal string Token { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the token was received.
        /// </summary>
        internal DateTime Received { get; set; }
    }
}