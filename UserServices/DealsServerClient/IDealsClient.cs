//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The DealsClient interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DealsServerClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using DotM.DataContracts;
    
    /// <summary>
    /// The DealsClient interface.
    /// </summary>
    public interface IDealsClient
    {
        /// <summary>
        /// Returns a set of deals based on their business ids.
        /// </summary>
        /// <count>Max number of deals to return.</count>
        /// <param name="businessIds">
        /// The business Ids. 
        /// </param>
        /// <param name="maxDealsPerBusiness">
        /// The max Deals Per Business. 
        /// </param>
        /// <param name="sort">
        /// The sort order. <seealso cref="DealsSort"/> 
        /// </param>
        /// <param name="expiresAfter">
        /// The expires After. 
        /// </param>
        /// <param name="activeBefore">
        /// The active Before. 
        /// </param>
        /// <returns>
        /// The deals corresponding to the input ids. 
        /// </returns>
        Task<IEnumerable<Deal>> GetDealsByBusiness(IList<string> businessIds, int? maxDealsPerBusiness = null, DealsSort? sort = null, DateTime? expiresAfter = null, DateTime? activeBefore = null);

        /// <summary>
        /// Returns list of deals with the input id(s), or null if no such deals exist. 
        /// </summary>
        /// <param name="dealIds">
        /// The deal ids.
        /// </param>
        /// <param name="count">
        /// <param name="count">max result size.</param>
        /// </param>
        /// <param name="refinements">The refinements. <seealso cref="Refinements"/></param>
        /// <param name="format">The format of the response: "simple", "all",...</param>
        /// <returns>
        /// The <see cref="IEnumerable{Deal}"/>.
        /// </returns>
        Task<IEnumerable<Deal>> GetDealsById(IList<Guid> dealIds, int? count = null, Refinements refinements = null, string format = "default");

        /// <summary>
        /// Returns list of deals nearby to the input location. Asyc execution
        /// </summary>
        /// <param name="coordinates">
        /// coordinate (required) as latitude and longitude
        /// </param>
        /// <param name="radius">
        /// meters - (optional). Default is 1000
        /// </param>
        /// <param name="count">
        /// max result size - (optional). Default is 50
        /// </param>
        /// <param name="refinements">
        /// The refinements. <seealso cref="Refinements"/> 
        /// </param>
        /// <returns>
        /// <returns>list of nearby deals </returns>
        /// </returns>
        Task<IEnumerable<Deal>> GetNearbyDeals(Coordinates coordinates, double? radius = null, int? count = null, Refinements refinements = null);

        /// <summary>
        /// Get deals by region to the input location. Asyc execution
        /// </summary>
        /// <param name="regionCode">the region code</param>
        /// <param name="coordinates">
        /// coordinate (optional) as latitude and longitude
        /// </param>
        /// <param name="radius">
        /// meters - (optional). Default is 1000
        /// </param>
        /// <param name="count">
        /// max result size - (optional). Default is 50
        /// </param>
        /// <param name="refinements">
        /// The refinements. <seealso cref="Refinements"/> 
        /// </param>
        /// <param name="anid">
        /// Anid of the user
        /// </param>
        /// <returns>list of deals </returns>
        Task<IEnumerable<Deal>> GetDealsByRegion(string regionCode, Coordinates coordinates, double? radius = null, int? count = null, Refinements refinements = null,string anid = null);

        /// <summary>
        ///  list of online deals independent of user location. Asyc execution
        /// </summary>
        /// <param name="count">max result size - (optional). Default is 50</param>
        /// <param name="refinements">
        /// The refinements. <seealso cref="Refinements"/> 
        /// </param>
        /// <returns>list of online deals </returns>
        Task<IEnumerable<Deal>> GetOnlineDeals(int? count = null, Refinements refinements = null);
    }
}