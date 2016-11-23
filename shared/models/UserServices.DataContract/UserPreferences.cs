//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user preferences.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The user preferences.
    /// </summary>
    [DataContract]
    public class UserPreferences
    {
        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "categories")]
        public List<Guid> Categories { get; set; }
    }
}