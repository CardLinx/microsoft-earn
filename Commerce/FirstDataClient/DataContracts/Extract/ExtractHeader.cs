//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;

    /// <summary>
    /// Represents the header for a First Data extract file.
    /// </summary>
    public class ExtractHeader
    {
        /// <summary>
        /// Gets or sets the ID of the provider to which the extract file belongs.
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the extract file was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider to which the extract file belongs.
        /// </summary>
        public string ProviderName { get; set; }
    }
}