//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataModels
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a batch of deals (currently used in claiming process).
    /// </summary>
    public class DealBatch
    {
        /// <summary>
        /// Initializes a new instance of the DealBatch class.
        /// </summary>
        public DealBatch()
        {
            DealIds = Enumerable.Empty<int>();
        }

        /// <summary>
        /// Gets or sets the ID for this batch.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the list of deal ids for this batch.
        /// </summary>
        public IEnumerable<int> DealIds { get; set; }
    }
}