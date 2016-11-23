//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The category weight
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    /// <summary>
    /// Category weight
    /// </summary>
    public class CategoryWeight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryWeight" /> class.
        /// </summary>
        /// <param name="category">category name</param>
        /// <param name="weight">the weight</param>
        public CategoryWeight(string category, double weight)
        {
            Category = category;
            Weight = weight;
        }

        /// <summary>
        /// Gets category name
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// Gets weight
        /// </summary>
        public double Weight { get; private set; }
    }
}