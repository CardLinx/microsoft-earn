//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The Provider weight
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Lomo.DataModels.Offers.Query
{
    /// <summary>
    /// Provider/Originator weight
    /// </summary>
    public class ProviderWeight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderWeight" /> class.
        /// </summary>
        /// <param name="provider">provider string</param>
        /// <param name="weight">the weight</param>
        public ProviderWeight(string provider, double weight)
        {
            Provider = provider;
            Weight = weight;
        }

        /// <summary>
        /// Gets provider/orignator
        /// </summary>
        public string Provider { get; private set; }

        /// <summary>
        /// Gets weight
        /// </summary>
        public double Weight { get; private set; }
    }
}