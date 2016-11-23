//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deals selection conditions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.Settings
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The deals selection conditions.
    /// </summary>
    public class DealsSelectionConditions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the providers whitelist.
        /// </summary>
        [XmlArray("ProvidersWhitelist")]
        public List<string> ProvidersWhitelist { get; set; }

        #endregion
    }
}