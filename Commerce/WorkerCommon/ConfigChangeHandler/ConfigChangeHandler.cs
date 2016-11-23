//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Default implementation for handling config changes
    /// </summary>
    public class ConfigChangeHandler : IConfigChangeHandler
    {
        public ConfigChangeHandler(CommerceLog log, string[] exemptConfigurationItems)
        {
            Log = log;
            ExemptConfigurationItems = exemptConfigurationItems;
        }

        /// <summary>
        /// Event handler for the scenario when a config value is changing
        /// </summary>
        /// <param name="sender">
        /// Who the sender is
        /// </param>
        /// <param name="changingEventArgs">
        /// Changing Event args <see cref="RoleEnvironmentChangingEventArgs"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter changingEventArgs cannot be null.
        /// </exception>
        public void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs changingEventArgs)
        {
            if (changingEventArgs == null)
            {
                throw new ArgumentNullException("changingEventArgs", "Parameter changingEventArgs cannot be null.");
            }

            Log.Information("Got a configuration change event");

            //Cancel true means reboot. So we need to ask if we have a change we care about then don't reboot.
            changingEventArgs.Cancel = ShouldRebootRole(changingEventArgs);
        }

        /// <summary>
        /// Get updated properties
        /// </summary>
        /// <param name="changedEventArgs">
        /// Changed Event args <see cref="RoleEnvironmentChangedEventArgs"/>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter changedEventArgs cannot be null.
        /// </exception>
        public IDictionary<string, string> RetrieveNewValues(RoleEnvironmentChangedEventArgs changedEventArgs)
        {
            if (changedEventArgs == null)
            {
                throw new ArgumentNullException("changedEventArgs", "Parameter changedEventArgs cannot be null.");
            }

            IEnumerable<RoleEnvironmentConfigurationSettingChange> environmentChanges = changedEventArgs.Changes.OfType<RoleEnvironmentConfigurationSettingChange>();
            Dictionary<string, string> newKeyValues = new Dictionary<string, string>();
            foreach (RoleEnvironmentConfigurationSettingChange configurationSettingChange in environmentChanges)
            {
                foreach (string key in ExemptConfigurationItems)
                {
                    if (key == configurationSettingChange.ConfigurationSettingName)
                    {
                        newKeyValues.Add(key, ReadChangedProperty(key));
                    }
                }
            }

            return newKeyValues;
        }

        /// <summary>
        /// Figure out when we have changes in config we expect
        /// </summary>
        /// <param name="eventArgs">
        /// Event payload
        /// </param>
        /// <returns>
        /// True/False
        /// </returns>
        private bool ShouldRebootRole(RoleEnvironmentChangingEventArgs eventArgs)
        {
            IEnumerable<RoleEnvironmentConfigurationSettingChange> environmentChanges = eventArgs.Changes.OfType<RoleEnvironmentConfigurationSettingChange>();
            return environmentChanges.Any(x => !ExemptConfigurationItems.Contains(x.ConfigurationSettingName));
        }

        /// <summary>
        /// If the property has changed, this method will read in the new value
        /// </summary>
        /// <param name="propertyKey">
        /// Property we are interested in
        /// </param>
        /// <returns>
        /// New Value
        /// </returns>
        internal static string ReadChangedProperty(string propertyKey)
        {
            if (General.RunningInAzure == true)
            {
                return RoleEnvironment.GetConfigurationSettingValue(propertyKey);
            }
            return null;
        }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        internal CommerceLog Log { get; set; }

        /// <summary>
        /// Exempt config on which we handle restart
        /// </summary>
        private readonly string[] ExemptConfigurationItems;
    }
}