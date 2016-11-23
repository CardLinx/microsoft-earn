//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The send email request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DataContract
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The send email request.
    /// </summary>
    [DataContract]
    public class SendEmailRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is test.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "is_test")]
        public bool IsTest { get; set; }

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "content")]
        public EmailContent Content { get; set; }

        /// <summary>
        ///     Gets or sets the to list.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "to_list")]
        public List<string> ToList { get; set; }

        #endregion
    }
}