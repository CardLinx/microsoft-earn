//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The email content creator factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoMo.UserServices.DealsMailing
{
    using System;

    using DealsServerClient;

    using OffersEmail.DataContracts;

    /// <summary>
    /// The email content creator factory.
    /// </summary>
    public class EmailContentCreatorFactory : IEmailContentCreatorFactory
    {
        /// <summary>
        /// The client name.
        /// </summary>
        private const string ClientName = "BO_EMAIL";

        /// <summary>
        /// The content creator.
        /// </summary>
        private readonly EmailContentCreator<DailyDealsContract> contentCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailContentCreatorFactory"/> class.
        /// </summary>
        /// <param name="dealsServerBaseAddress">
        /// The deals server base address.
        /// </param>
        public EmailContentCreatorFactory(Uri dealsServerBaseAddress)
        {
            DailyDealsTemplateCreator mockModelContentCreator = new DailyDealsTemplateCreator();
            DealsClient dealsClient = new DealsClient(dealsServerBaseAddress,ClientName);
            IDealsSelector dealsSelector = new NaiveDealsSelector(dealsClient);
            IEmailRenderingClient<DailyDealsContract> renderingClient = new EmailRenderingClient();
            this.contentCreator = new EmailContentCreator<DailyDealsContract>(dealsSelector, renderingClient, mockModelContentCreator);
        }

        /// <summary>
        /// The get content creator.
        /// </summary>
        /// <param name="emailCargo">
        /// The job.
        /// </param>
        /// <returns>
        /// The <see cref="IEmailContentCreator"/>.
        /// </returns>
        public IEmailContentCreator GetContentCreator(EmailCargo emailCargo)
        {
            return this.contentCreator;
        }
    }
}