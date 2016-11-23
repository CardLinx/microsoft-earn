//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Base class that defines the data needed for sending the email
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    /// <summary>
    /// Base class that defines the data needed for sending the email
    /// </summary>
    public class EmailTemplateData
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Gets or sets the unsubscribe url 
        /// </summary>
        public string UnsubscribeUrl { get; set; }
    }
}