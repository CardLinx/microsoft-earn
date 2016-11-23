//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The DealSelector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using DotM.DataContracts;

    /// <summary>
    /// The DealSelector interface.
    /// </summary>
    public interface IDealsSelector
    {
        /// <summary>
        /// The get deals.
        /// </summary>
        /// <param name="emailJob">
        /// The email Job.
        /// </param>
        /// <param name="dealsToExclude"> deals to exclude</param>
        /// <returns>
        /// The <see cref="IEnumerable{Deal}"/>.
        /// </returns>
        IEnumerable<Deal> GetDeals(DealsEmailCargo emailJob, IEnumerable<Guid> dealsToExclude);
    }
}