//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The DotM User .
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Users.Dal.DataModel
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The DotM User .
    /// </summary>
    [DataContract]
    public class User
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets Email.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is email confirmed.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "is_email_confirmed")]
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        ///     Gets or sets microsoft id.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "ms_id")]
        public string MsId { get; set; }

        /// <summary>
        ///     Gets or sets Id.
        /// </summary>
        [DataMember(IsRequired = true, Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the activation token for an unauthenticated user. 
        ///     Used to associate the unauthenticated user when linking the user with their MS or FB id
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "activation_token")]
        public string ActivationToken { get; set; }

        /// <summary>
        ///     Gets or sets User Information.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "info")]
        public UserInfo Info { get; set; }

        /// <summary>
        ///     Gets or sets the Name.
        /// </summary>
        [DataMember(IsRequired = true, Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is suppressed.
        /// </summary>
        [DataMember(IsRequired = true, Name = "is_suppressed")]
        public bool IsSuppressed { get; set; }

        /// <summary>
        /// Gets or sets the user's phone number
        /// </summary>
        [DataMember(IsRequired = true, Name = "phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the user's source
        /// </summary>
        [DataMember(IsRequired = true, Name = "source")]
        public string Source { get; set; }
        
        #endregion
    }
}