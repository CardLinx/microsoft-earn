//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.ViewModels
{
    using System;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using Resources;

    /// <summary>
    /// Identifies the type of the deal
    /// </summary>
    public enum DealType
    {
        /// <summary>
        /// Prepaid deal
        /// </summary>
        Prepaid = 0,

        /// <summary>
        /// CardLink offer
        /// </summary>
        CardLinked = 1
    }

    /// <summary>
    /// deal View Model
    /// </summary>
    public class DealVM
    {
        #region Fields

        /// <summary>
        /// The max title characters
        /// </summary>
        private const int MaxTitleCharacters = 96;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DealVM" /> class.
        /// </summary>
        /// <param name="deal">the deal</param>
        public DealVM(JObject deal)
        {
            this.Id = deal["id"].Value<string>();
            this.DealType = (DealType)deal["deal_type"].Value<int>();
            this.TransactionUrl = deal["transaction_url"].Value<string>();
            this.Price = deal["price"].Value<string>();
            this.OriginalPrice = deal["original_price"].Value<string>();
            this.Attribution = deal["attribution"].Value<string>();
            this.BusinessName = deal["business_name"].Value<string>();
            this.Description = deal["description"].Value<string>();
            this.Website = deal["website"].Value<string>();
            this.LargeImageUrl = deal["hero_image_url"].Value<string>();
            this.MediumImageUrl = deal["square_image_url"].Value<string>();
            this.Title = deal["title"].Value<string>();
            if (DealType == DealType.Prepaid)
            {
                this.Discount = deal["discount"].Value<string>();
            }
            else
            {
                if (deal["cardlink_dealinfos"] != null)
                {
                    var cardLinkedInfo = deal["cardlink_dealinfos"].First;
                    if (cardLinkedInfo != null)
                    {
                        this.Discount = cardLinkedInfo["discount"].Value<string>();
                        this.DiscountAmmount = cardLinkedInfo["discount_amount"].Value<string>();
                        this.MinimumSpend = cardLinkedInfo["minimum_spend"].Value<string>();
                    }
                }
            }

            this.CardLinkUrl = string.Format("{0}?c=8", DailyDeals.LinkOfferUrl);

            // remove the additional discount info present within the parenthesis
            this.Title = Regex.Replace(this.Title, @"\(([^\)]*)\)", string.Empty);

            // ensure the title doesn't exceeds 96 characters
            if (this.Title.Length > MaxTitleCharacters)
            {
                this.Title = string.Format("{0} ...", this.Title.Remove(this.Title.LastIndexOf(" ", MaxTitleCharacters - 1, StringComparison.Ordinal)));
            }
        }

        /// <summary>
        /// Gets or sets the deal Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Transaction Url
        /// </summary>
        public string TransactionUrl { get; set; }

        /// <summary>
        /// Gets or sets Deal Price
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets Deal Original Price
        /// </summary>
        public string OriginalPrice { get; set; }

        /// <summary>
        /// Gets or sets Deal Discount Percentage
        /// </summary>
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets Deal Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Deal Brand
        /// </summary>
        public string Attribution { get; set; }

        /// <summary>
        /// Gets or sets Deal Store PropertyName
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// Gets or sets Deal Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Deal Url
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets Deal Large Image Url
        /// </summary>
        public string LargeImageUrl { get; set; }

        /// <summary>
        /// Gets or sets Deal Medium Image Url
        /// </summary>
        public string MediumImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the card link URL.
        /// </summary>
        /// <value>
        /// The card link URL.
        /// </value>
        public string CardLinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the DealType
        /// </summary>
        public DealType DealType { get; set; }

        /// <summary>
        /// Gets or sets the Discount amount on the deal - applies only for card link deals
        /// </summary>
        public string DiscountAmmount { get; set; }

        /// <summary>
        /// Gets or sets the Minimum amount to spend before gettingt a discount - applies only for card link deals
        /// </summary>op
        public string MinimumSpend { get; set; }
    }
}