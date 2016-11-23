//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.ViewModels
{
    /// <summary>
    /// Subscription confirmation email view model.
    /// </summary>
    public class ConfirmSubscriptionVM
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmSubscriptionVM" /> class.
        /// </summary>
        /// <param name="confirmationUrl">The confirmation URL.</param>
        public ConfirmSubscriptionVM(string confirmationUrl)
        {
            this.ConfirmationUrl = confirmationUrl;
        }

        /// <summary>
        /// Gets or sets the confirmation URL.
        /// </summary>
        /// <value>
        /// The confirmation URL.
        /// </value>
        public string ConfirmationUrl { get; set; }
    }
}