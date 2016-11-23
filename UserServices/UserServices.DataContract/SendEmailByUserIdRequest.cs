//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The send email by user id request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The send email by user id request.
    /// </summary>
    [DataContract]
    public class SendEmailByUserIdRequest
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "content")]
        public EmailContent Content { get; set; }

        /// <summary>
        ///     Gets or sets the user ids. 
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "user_ids")]
        public List<Guid> UserIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this email job should use sendgrid test account
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "is_test")]
        public bool IsTest { get; set; }

        #endregion
    }
}