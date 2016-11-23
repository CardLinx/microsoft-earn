//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The user info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal.DataModel
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// The user info.
    /// </summary>
    [DataContract]
    public class UserInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the preferences.
        /// </summary>
        [JsonProperty(PropertyName = "preferences")]
        [DataMember(IsRequired = false, Name = "preferences")]
        public UserPreferences Preferences { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        [DataMember(IsRequired = false, Name = "location")]
        public Location Location { get; set; }

        #endregion
    }
}