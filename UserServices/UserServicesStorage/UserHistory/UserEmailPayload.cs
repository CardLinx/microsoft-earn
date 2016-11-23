//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user email payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.Storage.UserHistory
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The user email payload.
    /// </summary>
    [XmlRoot("data")]
    public class UserEmailPayload
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the deal ids.
        /// </summary>
        [XmlArray("deals")]
        public List<Guid> DealIds { get; set; }

        #endregion
    }
}