//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The user.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "information")]
        public UserInformation Information { get; set; }
    }
}