//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The lookup service settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.EmailSubscription.Service
{
    /// <summary>
    /// The lookup service settings.
    /// </summary>
    public class LookupServiceSettings
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the publication description.
        /// </summary>
        public string PublicationDescription { get; set; }

        /// <summary>
        /// Gets or sets the publication id.
        /// </summary>
        public string PublicationId { get; set; }

        /// <summary>
        /// Gets or sets the publication name.
        /// </summary>
        public string PublicationName { get; set; }

        /// <summary>
        /// Gets or sets the publication opt-in link.
        /// </summary>
        public string PublicationOptinLink { get; set; }

        /// <summary>
        /// Gets or sets the mdm application id.
        /// </summary>
        public int MdmApplicationId { get; set; }

        #endregion
    }
}