//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.DataModels.Offers.Query
{
    /// <summary>
    /// Provider/Originator weight
    /// </summary>
    public class DealTypeWeight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DealTypeWeight" /> class.
        /// </summary>
        /// <param name="dealType">the deal type</param>
        /// <param name="weight">the weight</param>
        public DealTypeWeight(int dealType, double weight)
        {
            DealType = dealType;
            Weight = weight;
        }

        /// <summary>
        /// Gets provider/orignator
        /// </summary>
        public int DealType { get; private set; }

        /// <summary>
        /// Gets weight
        /// </summary>
        public double Weight { get; private set; }
    }
}