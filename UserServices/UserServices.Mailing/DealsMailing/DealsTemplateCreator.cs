//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
//   The model content creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Net;
    using System.Globalization;
    using System.Linq;
    using DotM.DataContracts;

    using Lomo.Logging;
    using Lomo.GeoSpatial;

    using OffersEmail.DataContracts;

    using LocationType = Users.Dal.DataModel.LocationType;

    /// <summary>
    /// The model content creator.
    /// </summary>
    public class DealsTemplateCreator : ITemplateModelCreator<DailyDealsContract>
    {
        /// <summary>
        /// Generate Email Model method
        /// </summary>
        /// <returns> The <see cref="DealsEmailModel"/>. </returns>
        /// <exception cref="ModelContentException"> The input is insufficient for email model creation </exception>
        public DailyDealsContract GenerateModel(EmailTemplateData modelData)
        {
            DailyDealsContract dailyDealsContract = null;
            DealsTemplateData dailyDealsModelData = modelData as DealsTemplateData;
            if (dailyDealsModelData != null)
            {
                var dealsList = dailyDealsModelData.Deals == null ? null : dailyDealsModelData.Deals.ToList();
                DealContract[] dealsContract = null;
                if (dealsList != null)
                {
                    if (dailyDealsModelData.DealEmailType == DealEmailType.WeeklyDeal && dealsList.Count() <= 3)
                    {
                        throw new ModelContentException(string.Format("Number of deals is: {0}. This is insufficient for email model creation", dealsList.Count()));
                    }

                    dealsContract = new DealContract[dealsList.Count];
                    for (int i = 0; i < dealsList.Count; ++i)
                    {
                        DealContract dealContract = ConvertDeal(dealsList[i]);
                        if (dealContract != null)
                        {
                            dealsContract[i] = dealContract;
                        }
                    }

                    if (dailyDealsModelData.DealEmailType == DealEmailType.WeeklyDeal && dealsContract.Length <= 3)
                    {
                        throw new ModelContentException(string.Format("Number of deals is: {0}. This is insufficient for email model creation", dealsList.Count()));
                    }
                }

                var location = Users.Dal.DataModel.Location.Parse(dailyDealsModelData.LocationId);
                var locationStr = string.Empty;
                if (location.Type == LocationType.Postal || location.Type == LocationType.City)
                {
                    Log.Info("Getting Location for user: {0}, locationId: {1}", dailyDealsModelData.EmailAddress, dailyDealsModelData.LocationId);
                    var geoCodePoint = GeoSpatial.GetGeoData(WebUtility.HtmlEncode(string.Format("{0} {1} {2}", location.CountryCode, location.AdminDistrict, location.Value)), GeoSpatial.GeoSource.VirtualEarth);
                    if (geoCodePoint != null && geoCodePoint.Location != null)
                    {
                        locationStr = geoCodePoint.Location.Locality;
                        Log.Info("Retrieved Location info : {0} for user: {1}, locationId: {2}", locationStr, dailyDealsModelData.EmailAddress, dailyDealsModelData.LocationId);
                    }
                    else
                    {
                        Log.Warn("Couldn't fetch location data for user: {0}, locationId: {1}", dailyDealsModelData.EmailAddress, location);
                    }
                }

                dailyDealsContract = new DailyDealsContract { UnsubscribeUrl = dailyDealsModelData.UnsubscribeUrl, Deals = dealsContract, Location = locationStr };
            }
            
            return dailyDealsContract;
        }

        /// <summary>
        /// The convert deal.
        /// </summary>
        /// <param name="deal">
        /// The deal.
        /// </param>
        /// <returns>
        /// The <see cref="DealContract"/>.
        /// </returns>
        private DealContract ConvertDeal(Deal deal)
        {
            DealContract dealContract = new DealContract
                                            {
                                                Id = deal.Id,
                                                BusinessName = deal.Business == null ? string.Empty : deal.Business.Name,
                                                Description = deal.Description,
                                                DealType = deal.DealType == 7 ? DealType.CardLinked : DealType.Prepaid,

                                                // TODO - fix when email will be international
                                                Price = deal.Price <= 0 ? string.Empty : Math.Ceiling(deal.Price).ToString("C0", CultureInfo.CreateSpecificCulture("en-US")),
                                                OriginalPrice = deal.DealInfo == null || deal.DealInfo.VoucherValue <= 0 ? string.Empty : Math.Ceiling(deal.DealInfo.VoucherValue).ToString("C0", CultureInfo.CreateSpecificCulture("en-US")),
                                                Title = deal.Title,
                                                TransactionUrl = deal.TransactionUrl,
                                                LargeImageUrl = deal.ImageUrl + "&size=12",
                                                MediumImageUrl = deal.ImageUrl + "&size=4",

                                                // TODO - add this information when we are getting it from the ingestion
                                                Website = string.Empty
                                            };

            if (dealContract.DealType == DealType.CardLinked && deal.CardLinkInfos != null && deal.CardLinkInfos.Any())
            {
                dealContract.Attribution = null; //omit the attribution for clo deals for now
                dealContract.CardLinkInfos = new CardLinkInfo[deal.CardLinkInfos.Count];

                for (int i = 0; i < deal.CardLinkInfos.Count; i++)
                {
                    dealContract.CardLinkInfos[i] = new CardLinkInfo
                        {
                            MinimumSpend =
                                deal.CardLinkInfos[i].MinimumPurchase <= 0
                                    ? string.Empty
                                    : string.Format("{0}+",
                                                    Math.Ceiling(deal.CardLinkInfos[i].MinimumPurchase)
                                                        .ToString("C0", CultureInfo.CreateSpecificCulture("en-US"))),
                            DiscountAmount =
                                deal.CardLinkInfos[i].DiscountAmount <= 0
                                    ? string.Empty
                                    : string.Format("{0}",
                                                    Math.Ceiling(deal.CardLinkInfos[i].DiscountAmount)
                                                        .ToString("C0", CultureInfo.CreateSpecificCulture("en-US"))),
                            Discount =
                                deal.CardLinkInfos[i].DiscountPercent <= 0
                                    ? string.Empty
                                    : string.Format("{0}%",
                                                    deal.CardLinkInfos[i].DiscountPercent.ToString("F0",
                                                                                                   CultureInfo
                                                                                                       .InvariantCulture))
                        };
                }

                //If it's a CLO, check to see if there's either a discount or discountamount
                bool validDeal = dealContract.CardLinkInfos.Any(cardlinkInfo => !string.IsNullOrEmpty(cardlinkInfo.Discount) || !string.IsNullOrEmpty(cardlinkInfo.DiscountAmount));

                //If it's not a valid deal, return null so that this deal is not included in the deals email.
                if (!validDeal)
                {
                    dealContract = null;
                }
            }
            else if (dealContract.DealType == DealType.Prepaid && deal.DealInfo != null)
            {
                dealContract.Attribution = deal.Attribution;
                dealContract.Discount = deal.DealInfo.VoucherDiscountPercent <= 0 ? string.Empty : string.Format("{0}%", deal.DealInfo.VoucherDiscountPercent.ToString("F0", CultureInfo.InvariantCulture));
                
                //If the discount is missing for the PrePaid deal, return null so that this deal is not included in the deals email.
                if (string.IsNullOrEmpty(dealContract.Discount))
                {
                    dealContract = null;
                }
            }
         
            return dealContract;
        }
    }
}