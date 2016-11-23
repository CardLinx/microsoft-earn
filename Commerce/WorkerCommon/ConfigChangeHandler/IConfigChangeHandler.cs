//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Interface definition for config change handling
    /// </summary>
    public interface IConfigChangeHandler
    {
        /// <summary>
        /// Event handler for the scenario when a config value is changing
        /// </summary>
        /// <param name="sender">
        /// Who the sender is
        /// </param>
        /// <param name="changingEventArgs">
        /// Changing Event args <see cref="RoleEnvironmentChangingEventArgs"/>
        /// </param>
        void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs changingEventArgs);

        /// <summary>
        /// Get updated properties
        /// </summary>
        /// <param name="changedEventArgs">
        /// Changed Event args <see cref="RoleEnvironmentChangedEventArgs"/>
        /// </param>
        IDictionary<string, string> RetrieveNewValues(RoleEnvironmentChangedEventArgs changedEventArgs);
    }
}