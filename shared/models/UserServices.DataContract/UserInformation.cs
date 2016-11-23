//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     The user information.
    /// </summary>
    [DataContract]
    public class UserInformation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the user preferences.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "preferences")]
        public UserPreferences Preferences { get; set; }

        /// <summary>
        ///     Gets or sets the user location.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "location")]
        public Location Location { get; set; }

        #endregion
    }
}