//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Notifications
{
    public class NotificationContent
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
        /// Override ToString
        /// </summary>
        /// <returns>
        /// Returns the string representation of the content
        /// </returns>
        public override string ToString()
        {
            return string.Format("Subject : {0} \r\n" +
                                 "HtmlBody : {1} \r\n" +
                                 "TextBody : {2}", Subject, HtmlBody, TextBody);
        }
    }
}