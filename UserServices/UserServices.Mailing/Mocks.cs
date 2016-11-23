//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The mock email content creator factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// This is a temporary file with mocks for parts that will be implemented later.
// TODO - delete this file
namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;

    using DealsServerClient;

    using DotM.DataContracts;

    using LoMo.Templating;
    using LoMo.UserServices.Storage.Settings;

    /// <summary>
    /// The mock email content creator factory.
    /// </summary>
    public class MockEmailContentCreatorFactory : IEmailContentCreatorFactory
    {
        /// <summary>
        /// The mock templates identifier.
        /// </summary>
        private const string MockTemplatesIdentifier = "test-templates";

        /// <summary>
        /// The client name.
        /// </summary>
        private const string ClientName = "test_test";

        /// <summary>
        /// The content creator.
        /// </summary>
        private readonly EmailContentCreator<DealsEmailModel> contentCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockEmailContentCreatorFactory"/> class.
        /// </summary>
        /// <param name="storageAccount">
        /// The storage account.
        /// </param>
        /// <param name="dealsServerBaseAddress">
        /// The deals server base address.
        /// </param>
        /// <param name="settingsContainerClient">
        /// The settings container client.
        /// </param>
        public MockEmailContentCreatorFactory(string storageAccount, Uri dealsServerBaseAddress, SettingsContainerClient settingsContainerClient)
        {
            MockModelContentCreator mockModelContentCreator = new MockModelContentCreator();
            DealsClient dealsClient = new DealsClient(dealsServerBaseAddress, ClientName);
            IDealsSelector dealsSelector = new NaiveDealsSelector(dealsClient);
            EmailTemplatesFetcher<DealsEmailModel> emailTemplatesFetcher = new EmailTemplatesFetcher<DealsEmailModel>(new TemplateService(new TemplateBlobStoreClient(storageAccount)), MockTemplatesIdentifier);
            this.contentCreator = new EmailContentCreator<DealsEmailModel>(dealsSelector, null, mockModelContentCreator);
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
            return contentCreator;
        }
    }

    /// <summary>
    /// The mock model content creator.
    /// </summary>
    public class MockModelContentCreator : ITemplateModelCreator<DealsEmailModel>
    {
        /// <summary>
        /// The generate model.
        /// </summary>
        /// <param name="emailJob">
        /// The email job.
        /// </param>
        /// <param name="deals">
        /// The deals.
        /// </param>
        /// <returns>
        /// The <see cref="DealsEmailModel"/>.
        /// </returns>
        public DealsEmailModel GenerateModel(DealsEmailCargo emailJob, IEnumerable<Deal> deals)
        {
            /**
            List<MockDealModel> dealsModel = new List<MockDealModel>();
            foreach (Deal deal in deals)
            {
                var dealModel = new MockDealModel
                                    {
                                        TransactionUrl = deal.TransactionUrl,
                                        BrandString = deal.DealProvider.ProviderBrandingName,
                                        Description = deal.Description,
                                        Discount = deal.DealInfo != null && deal.DealInfo.VoucherDiscountPercent != 0 ? Math.Round(deal.DealInfo.VoucherDiscountPercent, MidpointRounding.AwayFromZero) + "%" : string.Empty,
                                        OriginalPrice = deal.DealInfo != null && deal.DealInfo.VoucherValue != 0 ? "$" + Math.Round(deal.DealInfo.VoucherValue, MidpointRounding.AwayFromZero) : string.Empty,
                                        Price = deal.PriceDisplayString,
                                        LargeImageUrl = string.Format("https://az389013.vo.msecnd.net/{0}?size=10", deal.Id),
                                        MediumImageUrl = string.Format("https://az389013.vo.msecnd.net/{0}?size=4", deal.Id),
                                        Remaining = this.GetRemainingStr(deal.EndTime),
                                        StoreName = deal.Business != null ? deal.Business.Name : string.Empty,
                                        Title = deal.Title
                                    };
                dealsModel.Add(dealModel);
            }

            var model =
             new DealsEmailModel
             {
                 TopDeal = dealsModel.First(),
                 RestDeals = dealsModel.Skip(1).Take(3)
             };
            return model;
             **/
            return null;
        }

        /// <summary>
        /// The get remaining str.
        /// </summary>
        /// <param name="endTime">
        /// The end time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRemainingStr(string endTime)
        {
            return "TODO";
        }
    }

    /// <summary>
    /// The mock deals selector.
    /// </summary>
    public class MockDealsSelector : IDealsSelector
    {
        /// <summary>
        /// The get deals.
        /// </summary>
        /// <param name="emailJob">
        /// The email job.
        /// </param>
        /// <param name="dealsToExclude">
        /// The deals to exclude.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Deal> GetDeals(DealsEmailCargo emailJob, IEnumerable<Guid> dealsToExclude)
        {
            Random rnd = new Random();
            int numberOfDeals = 5;
            List<Deal> deals = new List<Deal>();
            for (int i = 0; i < numberOfDeals; i++)
            {
                Deal deal = new Deal();
                deal.Title = rnd.Next(25, 75) + "% Off For $" + rnd.Next(100, 500);
                deal.Description = "This is the deal description" + Guid.NewGuid();
                deal.TransactionUrl = @"http://www.yelp.com/seattle";
                deal.ImageUrl = @"http://a.abcnews.com/images/Technology/ht_microsoft_cc_120823_wg.jpg";
                deals.Add(deal);
            }

            return deals;
        }
    }
}