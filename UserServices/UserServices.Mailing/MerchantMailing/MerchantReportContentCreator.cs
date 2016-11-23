//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   Creates the content for Merchant transaction report email
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.UserServices.DealsMailing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Analytics.API.Contract;
    using Lomo.Logging;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure;
    using Newtonsoft.Json.Linq;
    using OffersEmail.DataContracts;

    /// <summary>
    /// Creates the content for Merchant transaction report email
    /// </summary>
    public class MerchantReportContentCreator : IEmailContentCreator
    {
        #region Const
       
        /// <summary>
        /// Settings Key for Analytics Api endpoint
        /// </summary>
        private const string AnalyticsApiAddress = "LoMo.AnalyticsApi.Address";

        /// <summary>
        /// Settings Key for Analytics Api user name
        /// </summary>
        private const string AnalyticsApiUserName = "LoMo.AnalyticsApi.UserName";

        /// <summary>
        /// Settings Key for Analytics Api password
        /// </summary>
        private const string AnalyticsApiPassword = "LoMo.AnalyticsApi.Password";

        /// <summary>
        /// Authorization scheme used in analytics api
        /// </summary>
        private const string AnalyticsAuthorizationScheme = "Bearer";

        /// <summary>
        /// Settings Key for Merchant portal URL
        /// </summary>
        private const string MerchantPortalUrl = "LoMo.MerchantMailing.MerchantPortalUrl";

        /// <summary>
        /// Settings Key for Merchant template URL
        /// </summary>
        private const string MerchantTemplateUrl = "LoMo.MerchantMailing.MerchantTemplateUrl";

        #endregion

        #region Members

        /// <summary>
        /// The template model creator.
        /// </summary>
        private MerchantReportTemplateCreator _templateModelCreator;

        /// <summary>
        /// Analytics Api endpoint
        /// </summary>
        private Uri _analyticsApiEndpoint;

        /// <summary>
        /// Analytics Api user name
        /// </summary>
        private string _analyticsUserName;

        /// <summary>
        /// Analytics Api Password
        /// </summary>
        private string _analyticsPassword;

        /// <summary>
        /// URL for merchant portal
        /// </summary>
        private string _merchantPortalUrl;

        /// <summary>
        /// URL for merchant template
        /// </summary>
        private string _merchantTemplateUrl;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the merchant report content creator
        /// </summary>
        public void Initialize()
        {
            Log.Verbose("Initializing {0}", this.GetType().Name);

            _analyticsApiEndpoint = new Uri(CloudConfigurationManager.GetSetting(AnalyticsApiAddress));
            _analyticsUserName = CloudConfigurationManager.GetSetting(AnalyticsApiUserName);
            _analyticsPassword = CloudConfigurationManager.GetSetting(AnalyticsApiPassword);
            _merchantPortalUrl = CloudConfigurationManager.GetSetting(MerchantPortalUrl);
            _merchantTemplateUrl = CloudConfigurationManager.GetSetting(MerchantTemplateUrl);
            _templateModelCreator = new MerchantReportTemplateCreator();

            Log.Verbose("Initialized {0}", this.GetType().Name);
        }

        /// <summary>
        /// Creates the content for merchant report
        /// </summary>
        /// <param name="emailCargo"></param>
        /// <returns></returns>
        public EmailData GetContent(object emailCargo)
        {
            EmailData emailData = null;
            string accessToken = GetAccessToken();
            Log.Verbose("Successfully got the access token from analytics");

            if (!string.IsNullOrEmpty(accessToken) && emailCargo is MerchantReportEmailCargo)
            {
                MerchantReportEmailCargo merchantReportEmailCargo = emailCargo as MerchantReportEmailCargo;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AnalyticsAuthorizationScheme, accessToken);
                string relativeUri = string.Format("api/redemptions/getbymerchantuser?fromDateTime={0}&toDateTime={1}&userid={2}", merchantReportEmailCargo.FromDate, merchantReportEmailCargo.ToDate, merchantReportEmailCargo.UserId);
                var redemptionsUri = new Uri(_analyticsApiEndpoint, relativeUri);

                Log.Verbose("Getting transaction data from analytics for the job : {0}", emailCargo.ToString());
                HttpResponseMessage response = client.GetAsync(redemptionsUri).Result;
                string analyticsReponse = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.Warn("Unbale to generate report for the merchant user [Job Id : {0}, User Id : {1}]. Reason : {2}", merchantReportEmailCargo.Id, merchantReportEmailCargo.UserId, response.ReasonPhrase);
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
                else
                {
                    Log.Verbose("Got transaction data from analytics for the job : {0}", emailCargo.ToString());
                    emailData = GetEmailData(merchantReportEmailCargo, analyticsReponse);
                }
            }

            return emailData;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the emaildata for merchant transaction report
        /// </summary>
        /// <param name="merchantReportEmailCargo">The Merchant report email cargo</param>
        /// <param name="analyticsReponse">Response from analytics for merchant transaction call</param>
        /// <returns>Emaildata for merchant transaction report</returns>
        private EmailData GetEmailData(MerchantReportEmailCargo merchantReportEmailCargo,string analyticsReponse)
        {
            MerchantReportContract merchantReportContract;
            Dictionary<string, List<RedemptionContract>> redemptionsByMerchant = new Dictionary<string, List<RedemptionContract>>();

            if (!string.IsNullOrEmpty(analyticsReponse))
            {
                JArray jresponse = JArray.Parse(analyticsReponse);
                if (jresponse.Count > 0)
                {
                    RedemptionContract[] redemptions = new RedemptionContract[jresponse.Count];
                    for (int i = 0; i < jresponse.Count; i++)
                    {
                        var redemption = jresponse[i].ToObject<RedemptionContract>();
                        if (redemption != null)
                        {
                            redemptions[i] = redemption;
                        }
                    }
                    var groupedItems = redemptions.GroupBy(redemption => redemption.MerchantId);
                    foreach (var groupItem in groupedItems)
                    {
                        redemptionsByMerchant[groupItem.Key.ToString()] = groupItem.OrderBy(redemption => redemption.AuthorizationDateTimeLocal).ToList();
                    }
                }
            }

            MerchantTemplateData merchantTemplateData = new MerchantTemplateData
            {
                EmailAddress = merchantReportEmailCargo.EmailAddress,
                FromDate = merchantReportEmailCargo.FromDate,
                ToDate = merchantReportEmailCargo.ToDate,
                ScheduleType = merchantReportEmailCargo.ScheduleType,
                RedemptionsByMerchant = redemptionsByMerchant,
                MerchantPortalUrl = _merchantPortalUrl,
                UnsubscribeUrl = merchantReportEmailCargo.UnsubscribeUrl
            };

            Log.Verbose("Calling model creator to create the data contract for the job : {0}", merchantReportEmailCargo.ToString());
            merchantReportContract = _templateModelCreator.GenerateModel(merchantTemplateData);
            EmailRenderingClient<MerchantReportContract> emailRenderingClient = new EmailRenderingClient<MerchantReportContract>
            {
                EmailRenderingServiceUrl = _merchantTemplateUrl
            };

            EmailData emailData = new EmailData
            {
                Subject = merchantReportEmailCargo.Subject,
                HtmlBody = emailRenderingClient.RenderHtml(merchantReportContract),
                TextBody = string.Empty
            };

            Log.Verbose("Ready to send the email for the job : {0}", merchantReportEmailCargo.ToString());

            return emailData;
        }

        /// <summary>
        /// Returns the access token for invoking analytics api 
        /// </summary>
        /// <returns>Access token for the analytics api</returns>
        private string GetAccessToken()
        {
            var client = new HttpClient();

            var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", _analyticsUserName),
                    new KeyValuePair<string, string>("password", _analyticsPassword)
                };
            HttpContent content = new FormUrlEncodedContent(requestParams);
            var tokenUri = new Uri(_analyticsApiEndpoint, "token");
            HttpResponseMessage responseMessage = client.PostAsync(tokenUri, content).Result;
            string response = responseMessage.Content.ReadAsStringAsync().Result;
            JObject jresponse = JObject.Parse(response);
            string accessToken = jresponse["access_token"].ToString();

            return accessToken;
        }

        #endregion
    }
}