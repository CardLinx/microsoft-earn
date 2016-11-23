//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The EmailContentCreator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;

    using LoMo.UserServices.DataContract;
    
    /// <summary>
    /// The EmailContentCreator interface.
    /// </summary>
    public interface IEmailContentCreator
    {

        void Initialize();

        /// <summary>
        /// The get content.
        /// </summary>
        /// <returns>
        /// The <see cref="EmailContent"/>.
        /// </returns>
        EmailData GetContent(object emailCargo);
    }

    /// <summary>
    /// The email data.
    /// </summary>
    public class EmailData
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the html body.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets or sets the text body.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// Gets or sets the deals.
        /// </summary>
        public List<Guid> DealIds{ get; set; }
    }
}