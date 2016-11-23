//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The model content creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DealsServerClient;
    using DotM.DataContracts;
    using Microsoft.WindowsAzure;
    using System.Net;
    using Lomo.GeoSpatial;
    using Lomo.Logging;
    using Users.Dal.DataModel;
    using OffersEmail.DataContracts;
    using Microsoft.Azure;

    /// <summary>
    /// The model content creator.
    /// </summary>
    public class CampaignTemplateCreator : ITemplateModelCreator<CampaignDataContract>
    {
        #region Constants
       
        /// <summary>
        /// The deals server address setting.
        /// </summary>
        private const string DealsServerAddressSetting = "LoMo.DealsServer.Address";

        /// <summary>
        /// The client name.
        /// </summary>
        private const string ClientName = "BO_EMAIL";

        /// <summary>
        /// Flight to request only CLO deals from deals server
        /// </summary>
        private const string CloFlight = "DealsServer.CloPlus";

        /// <summary>
        /// Total number of deals to request from deals server
        /// </summary>
        const int DealsCount = 200;

        #endregion

        #region Data Members

        private readonly DealsClient _dealsClient;

        #endregion

        public CampaignTemplateCreator()
        {
            Uri dealsServerBaseAddress = new Uri(CloudConfigurationManager.GetSetting(DealsServerAddressSetting));
            _dealsClient = new DealsClient(dealsServerBaseAddress, ClientName);
        }
        

        /// <summary>
        /// Generate Email Model method
        /// </summary>
        /// <returns> The <see cref="CampaignDataContract"/>. </returns>
        public CampaignDataContract GenerateModel(EmailTemplateData modelData)
        {
            CampaignDataContract campaignDataContract = null;
            CampaignTemplateData campaignTemplateData = modelData as CampaignTemplateData;
            if (campaignTemplateData != null)
            {
                var location = Users.Dal.DataModel.Location.Parse(campaignTemplateData.LocationId);
                const string locationStr = "Bellevue"; //Having a constant value here and commenting the below code since 
                                                    //this code doesn't make sense anymore in Earn program. Also sending of emails
                                                    //will now be moved to Epiphany service and this code won't be used anymore
                //if (location.Type == Users.Dal.DataModel.LocationType.Postal || location.Type == Users.Dal.DataModel.LocationType.City)
                //{
                //    Log.Info("Getting Location for user: {0}, locationId: {1}", campaignTemplateData.EmailAddress, campaignTemplateData.LocationId);
                //    var geoCodePoint = GeoSpatial.GetGeoData(WebUtility.HtmlEncode(string.Format("{0} {1} {2}", location.CountryCode, location.AdminDistrict, location.Value)), GeoSpatial.GeoSource.VirtualEarth);
                //    if (geoCodePoint != null && geoCodePoint.Location != null)
                //    {
                //        locationStr = geoCodePoint.Location.Locality;
                //        Log.Info("Retrieved Location info : {0} for user: {1}, locationId: {2}", locationStr, campaignTemplateData.EmailAddress, campaignTemplateData.LocationId);
                //    }
                //    else
                //    {
                //        Log.Warn("Couldn't fetch location data for user: {0}, locationId: {1}", campaignTemplateData.EmailAddress, location);
                //    }
                //}

               
                    var refinements = new Refinements
                        {
                            ResultsPerBusiness = 1,
                            Market = string.Format("en-{0}", location.CountryCode),
                            Flights = string.Format("{0},{1}", CloFlight, campaignTemplateData.Campaign)
                        };

                    if (campaignTemplateData.IncludeBusinessNames)
                    {
                        string regionCode = string.Format("{0} {1} {2}", location.CountryCode, location.AdminDistrict,location.Value);
                        Task<IEnumerable<Deal>> getDealsTask = this._dealsClient.GetDealsByRegion(regionCode, null, null, DealsCount, refinements);
                        var returnedDeals = getDealsTask.Result.ToList();
                        List<string> businesses = new List<string>();
                        foreach (var deal in returnedDeals)
                        {
                            if (!businesses.Contains(deal.Business.Name) && businesses.Count < 20)
                            {
                                businesses.Add(deal.Business.Name);
                            }
                        }
                        campaignDataContract = new CampaignDataContract { UnsubscribeUrl = campaignTemplateData.UnsubscribeUrl, Content = campaignTemplateData.Content, Location = locationStr, PostalCode = location.Value, Businesses = businesses };
                    }
                    else
                    {
                        campaignDataContract = new CampaignDataContract { UnsubscribeUrl = campaignTemplateData.UnsubscribeUrl, Content = campaignTemplateData.Content, Location = locationStr, PostalCode = location.Value };
                    }
                

            }

            return campaignDataContract;
        }
    }
}