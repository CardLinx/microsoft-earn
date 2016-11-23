//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
// Interface for the user services worker job manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.Worker.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for the user services worker job manager
    /// </summary>
    public interface IJobManager
    {
        /// <summary>
        /// Bootstraps the job agents
        /// </summary>
        /// <param name="idPrefix">
        /// The agent id Prefix.
        /// </param>
        /// <returns>
        /// List of instantiaged job processors
        /// </returns>
        IEnumerable<IJobProcessor> Bootstrap(string idPrefix);
    }
}