//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The user preferences.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal.DataModel
{
    using System.Runtime.Serialization;
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The user preferences.
    /// </summary>
    [DataContract]
    public class UserPreferences
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [JsonProperty(PropertyName = "categories")]
        [DataMember(IsRequired = false, Name = "categories")]
        public IEnumerable<Guid> Categories { get; set; }

        /// <summary>
        /// Gets or sets the transaction notification medium
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "transaction_notification_medium")]
        public TransactionNotificationPreference TransactionNotificationMedium { get; set; }

        #endregion
    }
}