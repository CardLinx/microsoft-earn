//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Email
{
    using System.Collections.Generic;

    /// <summary>
    ///     The email content.
    /// </summary>
    public class EmailInformation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the email category
        /// </summary>
        internal string Category { get; set; }

        /// <summary>
        /// Gets or sets the unique identifiers
        /// </summary>
        internal Dictionary<string, string> UniqueIdentifiers { get; set; } 

        /// <summary>
        ///     Gets or sets the from.
        /// </summary>
        internal string From { get; set; }

        /// <summary>
        ///     Gets or sets the from display name
        /// </summary>
        internal string FromDisplayName { get; set; }

        /// <summary>
        /// Gets or sets Replay-To Address
        /// </summary>
        internal string ReplayTo { get; set; }

        /// <summary>
        ///     Gets or sets the html body.
        /// </summary>
        internal string HtmlBody { get; set; }

        /// <summary>
        ///     Gets or sets the subject.
        /// </summary>
        internal string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the text body.
        /// </summary>
        internal string TextBody { get; set; }

        /// <summary>
        ///     Gets or sets the to.
        /// </summary>
        internal IList<string> To { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this email job should use sendgrid test account
        /// </summary>
        internal bool IsTest { get; set; }

        #endregion
    }
}