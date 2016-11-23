//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OffersEmail.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    ///     The Deal Preview Model.
    /// </summary>
    public class DealPreviewModel
    {
        #region Public Properties

        /// <summary>
        ///  Gets or sets deal attribution
        /// </summary>
        [JsonProperty(PropertyName = "attribution")]
        public string Attribution { get; set; }

        /// <summary>
        ///     Gets or sets Business.
        /// </summary>
        [JsonProperty(PropertyName = "business")]
        public DealPreviewBusinessModel Business { get; set; }

        /// <summary>
        ///     Gets or sets DealInfo.
        /// </summary>
        [JsonProperty(PropertyName = "deal_info")]
        public DealPreviewVoucherInfoModel DealInfo { get; set; }

        /// <summary>
        ///     Gets or sets DealType.
        /// </summary>
        [JsonProperty(PropertyName = "deal_type")]
        public int DealType { get; set; }

        /// <summary>
        ///     Gets or sets Description.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets ImageUrl.
        /// </summary>
        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets currency symbol
        /// </summary>
        [JsonProperty(PropertyName = "currency_symbol")]
        public string CurrencySymbol { get; set; }

        /// <summary>
        ///     Gets or sets Title.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets CardLinkInfos.
        ///     Supporting multiple discount scenarios 
        /// </summary>
        [JsonProperty(PropertyName = "cardlink_dealinfos")]
        public List<DealPreviewCardlinkInfoModel> CardLinkInfos { get; set; }

        /// <summary>
        ///     Gets or sets the Deal Id assigned by the deal provider
        ///     This field is used during ingestion and should not be used for any other purpose.
        ///     The value in this field is a compound value comprising a provider prefix followed by the id of the deal, as given by the provider.
        /// </summary>
        [JsonProperty(PropertyName = "provider_deal_id")]
        public string ProviderDealId { get; set; }

        /// <summary>
        /// Gets the get discount.
        /// </summary>
        public string GetDiscount
        {
            get
            {
                if (DealType == 7)
                {
                    if (CardLinkInfos != null)
                    {
                        var cardlinkDealInfo = CardLinkInfos.FirstOrDefault();
                        if (cardlinkDealInfo != null)
                        {
                            if (cardlinkDealInfo.DiscountAmount != null)
                            {
                                return CurrencySymbol + cardlinkDealInfo.DiscountAmount;
                            }

                            if (cardlinkDealInfo.DiscountPercent != null)
                            {
                                return cardlinkDealInfo.DiscountPercent + "%";
                            }
                        }
                    }
                }
                else
                {
                    if (DealInfo != null)
                    {
                        if (DealInfo.DiscountPercentage != null)
                        {
                            return DealInfo.DiscountPercentage + "%";
                        }

                        if (DealInfo.DiscountValue != null)
                        {
                            return CurrencySymbol + DealInfo.DiscountValue;
                        }
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the get location.
        /// </summary>
        public string GetLocation
        {
            get
            {
                if (Business != null && Business.Locations != null && Business.Locations.Any())
                {
                    var firstLocation = Business.Locations.First();

                    var locationString = new StringBuilder();
                    locationString.Append(firstLocation.City);

                    if (!string.IsNullOrWhiteSpace(firstLocation.City) && !string.IsNullOrWhiteSpace(firstLocation.State))
                    {
                        locationString.Append(", ");
                    }

                    locationString.Append(firstLocation.State);
                    return locationString.ToString();
                }

                return string.Empty;
            }
        }

        #endregion
    }
}