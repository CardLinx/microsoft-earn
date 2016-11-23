//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The rule based deals selector.
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

    using LoMo.UserServices.Storage.Settings;

    using Users.Dal.DataModel;

    /// <summary>
    /// The rule based deals selector.
    /// </summary>
    public class RuleBasedDealsSelector : IDealsSelector
    {
        #region Fields

        /// <summary>
        /// The deals client.
        /// </summary>
        private readonly IDealsClient dealsClient;

        /// <summary>
        /// The deals locations.
        /// </summary>
        private readonly Dictionary<string, DealsLocation> dealsLocations;

        /// <summary>
        /// The default deals selection conditions.
        /// </summary>
        private readonly DealsSelectionConditions defaultDealsSelectionConditions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBasedDealsSelector"/> class.
        /// </summary>
        /// <param name="dealsClient">
        /// The deals client.
        /// </param>
        /// <param name="settingsContainerClient">
        /// The settings Container Client.
        /// </param>
        public RuleBasedDealsSelector(IDealsClient dealsClient, SettingsContainerClient settingsContainerClient)
            : this(dealsClient, settingsContainerClient.GetDealsSelectionRules())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBasedDealsSelector"/> class.
        /// </summary>
        /// <param name="dealsClient">
        /// The deals client.
        /// </param>
        /// <param name="dealsSelectionRules">
        /// The deals selection rules.
        /// </param>
        /// <exception cref="ArgumentNullException">dealsClient or dealsSelectionRules are null </exception>
        public RuleBasedDealsSelector(IDealsClient dealsClient, DealsSelectionRules dealsSelectionRules)
        {
            if (dealsClient == null)
            {
                throw new ArgumentNullException("dealsClient");
            }

            if (dealsSelectionRules == null)
            {
                throw new ArgumentNullException("dealsSelectionRules");
            }

            this.dealsClient = dealsClient;
            this.defaultDealsSelectionConditions = dealsSelectionRules.Default;
            this.dealsLocations = dealsSelectionRules.Locations.ToDictionary(elem => elem.Id, elem => elem);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get deals.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="subscription">
        /// The subscription.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Deal}"/>.
        /// </returns>
        public IEnumerable<Deal> GetDeals(DealsEmailCargo emailJob, IEnumerable<Guid> dealsToExclude)
        {
            throw new NotImplementedException();
            /**
            DealsLocation location;
            DealsSelectionConditions dealsSelectionConditions = this.defaultDealsSelectionConditions;
            if (this.dealsLocations.TryGetValue(subscription.LocationId, out location))
            {
                dealsSelectionConditions = location.Conditions;
            }

            var refinements = new Refinements();

            if (user.Info != null && 
                user.Info.Preferences != null 
                && user.Info.Preferences.Categories != null 
                && user.Info.Preferences.Categories.Any())
            {
                refinements.Categories = user.Info.Preferences.Categories.Select(elem => string.Format("lomo:{0}", elem));
            }

            if (dealsSelectionConditions.ProvidersWhitelist != null && dealsSelectionConditions.ProvidersWhitelist.Any())
            {
                refinements.Sources = dealsSelectionConditions.ProvidersWhitelist;
            }

            Task<IEnumerable<Deal>> getDealsTask;
            if (location != null)
            {
                getDealsTask = this.dealsClient.GetNearbyDeals(new Coordinates { Latitude = location.Latitude, Longitude = location.Longitude }, location.Radius, 5, refinements);
            }
            else
            {
                getDealsTask = this.dealsClient.GetOnlineDeals(5, refinements);
            }

            getDealsTask.Wait(TimeSpan.FromSeconds(20));
            return getDealsTask.Result;
             **/
            //TODO - reimplement if needed

        }

        #endregion
    }
}