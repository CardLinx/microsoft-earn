//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The deals selection rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.Settings
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The deals selection rules.
    /// </summary>
    [XmlRoot("DealsSelectionRules")]
    public class DealsSelectionRules
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        [XmlElement("Default")]
        public DealsSelectionConditions Default { get; set; }

        /// <summary>
        /// Gets or sets the locations.
        /// </summary>
        [XmlArray("Locations")]
        public List<DealsLocation> Locations { get; set; }

        #endregion
    }
}