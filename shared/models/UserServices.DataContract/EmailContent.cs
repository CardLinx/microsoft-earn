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
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The email content.
    /// </summary>
    [DataContract]
    public class EmailContent
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the from address.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "from")]
        public string From { get; set; }

        /// <summary>
        ///     Gets or sets the from display name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "from_display")]
        public string FromDisplay { get; set; }

        /// <summary>
        ///     Gets or sets the repay to address.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "reply_to")]
        public string ReplyTo { get; set; }

        /// <summary>
        ///     Gets or sets the html body.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "html_body")]
        public string HtmlBody { get; set; }

        /// <summary>
        ///     Gets or sets the subject.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "subject")]
        public string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the text body.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "text_body")]
        public string TextBody { get; set; }

        /// <summary>
        ///     Gets or sets the category.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "category")]
        public string Category { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifiers.
        /// </summary>        
        [DataMember(EmitDefaultValue = false, Name = "unique_identifiers")]
        public Dictionary<string, string> UniqueIdentifiers { get; set; }

        #endregion
    }
}