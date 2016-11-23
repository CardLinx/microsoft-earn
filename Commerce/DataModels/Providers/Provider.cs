//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System;

    /// <summary>
    /// Represents a provider stored in the data store.
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Provider"/> class.
        /// </summary>
        public Provider()
        {
            Name = String.Empty;
        }

        /// <summary>
        /// The Earn program ID for the Provider.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID for this provider in the wider services space.
        /// </summary>
        public string GlobalID { get; set; }

        /// <summary>
        /// The name of this provider.
        /// </summary>
        public string Name { get; set; }
    }
}