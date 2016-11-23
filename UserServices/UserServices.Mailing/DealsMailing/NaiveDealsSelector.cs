//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The naive deals selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DealsServerClient;

    using DotM.DataContracts;

    using Lomo.Logging;

    using LocationType = Users.Dal.DataModel.LocationType;

    /// <summary>
    /// The naive deals selector.
    /// </summary>
    public class NaiveDealsSelector : IDealsSelector
    {
        #region Constants

        /// <summary>
        /// Flight to request only CLO deals from deals server
        /// </summary>
        private const string CloFlight = "DealsServer.CloPlus";

        /// <summary>
        /// Flight to request only pre paid deals from deals server
        /// </summary>
        private const string NonCloFlight = "DealsServer.NoCloPlus";

        /// <summary>
        /// Total number of deals to request from deals server
        /// </summary>
        const int DealsCount = 200;

        #endregion

        /// <summary>
        /// The deals client.
        /// </summary>
        private readonly IDealsClient dealsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveDealsSelector"/> class.
        /// </summary>
        /// <param name="dealsClient">
        /// The deals client.
        /// </param>
        public NaiveDealsSelector(IDealsClient dealsClient)
        {
            this.dealsClient = dealsClient;
        }

        /// <summary>
        /// The get deals.
        /// </summary>
        /// <param name="emailJob">
        /// The email job.
        /// </param>
        /// <param name="dealsToExclude">deals to exclude</param>
        /// <returns>
        /// the list of deals
        /// </returns>
        /// <exception cref="ApplicationException">location id is wrong </exception>
        public IEnumerable<Deal> GetDeals(DealsEmailCargo emailJob, IEnumerable<Guid> dealsToExclude)
        {
            var refinements = new Refinements();

            if (emailJob.Categories != null && emailJob.Categories.Any())
            {
                refinements.Categories = emailJob.Categories.Select(elem => string.Format("lomo:{0}", elem));
            }
            
            refinements.ResultsPerBusiness = 1;
            string locationId = emailJob.LocationId;
            Users.Dal.DataModel.Location location = Users.Dal.DataModel.Location.Parse(locationId);

            Task<IEnumerable<Deal>> getDealsTask;
            if (location.CountryCode != "us")
            {
                Log.Error(string.Format("Location Id isn't in us. Location Id={0}", locationId));
            }

            refinements.Market = string.Format("en-{0}", location.CountryCode);
            string dealFlight = emailJob.IsCloDeal ? CloFlight : NonCloFlight;

            //Construct the flight name of the form <<DealFlight (whether to get clo or prepaid only deals)>>,<<campaign name (for tracking in analytics)?>>
            if (!string.IsNullOrEmpty(emailJob.Campaign))
            {
                refinements.Flights = string.Format("{0},{1}", dealFlight, emailJob.Campaign);
            }
            else
            {
                refinements.Flights = dealFlight;
            }

            List<Deal> selectedDeals;
            if (emailJob.DealIds != null && emailJob.DealIds.Any())
            {
                getDealsTask = this.dealsClient.GetDealsById(emailJob.DealIds.ToList(), refinements: refinements);
                //If we are selecting specific deals to send in the email, do not do any further actions like excluding previously sent deals 
                selectedDeals = getDealsTask.Result.ToList();
            }
            else
            {
                if (location.Type == LocationType.Postal || location.Type == LocationType.City)
                {
                    string regionCode = string.Format("{0} {1} {2}", location.CountryCode, location.AdminDistrict,
                                                      location.Value);
                    getDealsTask = this.dealsClient.GetDealsByRegion(regionCode, null, null, DealsCount, refinements,
                                                                     emailJob.Anid);
                }
                else if (location.Type == LocationType.National)
                {
                    getDealsTask = this.dealsClient.GetOnlineDeals(DealsCount, refinements);
                }
                else
                {
                    throw new ApplicationException(
                        string.Format(
                            "Location Id not supported. Only postal, national or city location type are allowed in this phase. Location Id={0}",
                            locationId));
                }

                var returnedDeals = getDealsTask.Result.ToList();
                HashSet<string> dealsToExcludeHash = new HashSet<string>();
                if (dealsToExclude != null && dealsToExclude.Any())
                {
                    foreach (var dealGuid in dealsToExclude)
                    {
                        dealsToExcludeHash.Add(dealGuid.ToString().ToLowerInvariant());
                    }
                }

                selectedDeals = this.SelectDeals(6, dealsToExcludeHash, returnedDeals);
            }

            return selectedDeals;
        }

        /// <summary>
        /// The select deals.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="dealsToExclude">
        /// The deals to exclude.
        /// </param>
        /// <param name="originalDeals">
        /// The original deals.
        /// </param>
        /// <returns>
        /// The list of deals
        /// </returns>
        private List<Deal> SelectDeals(int count, HashSet<string> dealsToExclude, IEnumerable<Deal> originalDeals)
        {
            return originalDeals.Where(elem => !dealsToExclude.Contains(elem.Id)).Take(count).ToList();
        }
    }
}