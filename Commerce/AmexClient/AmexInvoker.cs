//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Lomo.Commerce.Logging;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using Newtonsoft.Json;
    using System.Web;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides invocation of Amex service APIs.
    /// </summary>
    public class AmexInvoker : IAmexInvoker
    {
        /// <summary>
        /// Gets or sets the object through which logs can be added and obtained.
        /// </summary>
        public CommerceLog CommerceLog { get; set; }

        /// <summary>
        /// Gets or sets the object through which performance information can be added and obtained.
        /// </summary>
        public PerformanceInformation PerformanceInformation { get; set; }

        /// <summary>
        /// Adds the described card to Amex.
        /// </summary>
        /// <param name="amexCardSyncRequest">
        /// Description of the card to add.
        /// </param>
        /// <returns>
        /// The response from Amex for the add card attempt.
        /// </returns>
        public async Task<AmexCardResponse> AddCardAsync(AmexCardSyncRequest amexCardSyncRequest)
        {
            AmexOAuthResponse amexOAuthResponse = await GetOAuthTokenAsync();
            return await AddOrRemoveCardAsync(amexCardSyncRequest, ConfigurationManager.AppSettings[AmexConstants.AmexAddCardUri], amexOAuthResponse);
        }

        /// <summary>
        /// Adds the described card to Amex.
        /// </summary>
        /// <param name="amexCardSyncRequest">
        /// Description of the card to remove.
        /// </param>
        /// <returns>
        /// The response from Amex for the remove card attempt.
        /// </returns>
        public async Task<AmexCardResponse> RemoveCardAsync(AmexCardUnSyncRequest amexCardUnSyncRequest)
        {
            AmexOAuthResponse amexOAuthResponse = await GetOAuthTokenAsync();
            return await AddOrRemoveCardAsync(amexCardUnSyncRequest, ConfigurationManager.AppSettings[AmexConstants.AmexRemoveCardUri], amexOAuthResponse);
        }        

        private async Task<AmexOAuthResponse> GetOAuthTokenAsync()
        {
            // unix epoch format
            string timestamp = ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
            string nonce = string.Concat(timestamp, ":AMEX");
            string clientId = ConfigurationManager.AppSettings[AmexConstants.AmexOAuthClientId];

            // Format -> client_id \n timestamp \n timestamp:AMEX \n grant
            string message = string.Format("{0}\n{1}\n{2}\nclient_credentials\n", clientId, timestamp, nonce);

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings[AmexConstants.AmexOAuthClientSecret])))
            {
                string mac = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
                // Format -> MAC id="client_id",ts="timestamp",nonce="timestamp:AMEX",mac="HMACSHA256 hash"
                string authenticationHeader = string.Format("MAC id=\"{0}\",ts=\"{1}\",nonce=\"{2}\",mac=\"{3}\"", clientId, timestamp, nonce, mac);

                Stopwatch timer = Stopwatch.StartNew();
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Add("Authentication", authenticationHeader);
                        httpClient.DefaultRequestHeaders.Add("X-AMEX-API-KEY", clientId);
                        HttpContent httpContent = new StringContent("grant_type=client_credentials&app_spec_info=Apigee&guid_type=privateguid");
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        HttpResponseMessage response = await httpClient
                            .PostAsync(ConfigurationManager.AppSettings[AmexConstants.AmexOAuthUri], httpContent)
                            .ConfigureAwait(false);

                        string content = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            CommerceLog.Verbose("Received Amex OAuth token. \r\nContent: {0}", content);
                            return JsonConvert.DeserializeObject<AmexOAuthResponse>(content);
                        }
                        else
                        {
                            CommerceLog.Critical("Unable to get Amex OAuth token. \r\nStatus: {0} \r\nContent: {1}", null, response.StatusCode, content);
                        }
                    }
                }
                finally
                {
                    timer.Stop();
                    PerformanceInformation.Add("Amex OAuth API", string.Format("{0} ms", timer.ElapsedMilliseconds));
                }
            }

            return null;
        }

        private async Task<AmexCardResponse> AddOrRemoveCardAsync<T>(T requestPayload, string requestUri, AmexOAuthResponse amexOAuthResponse)
        {
            if (amexOAuthResponse != null)
            {
                string timestamp = ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                string nonce = string.Concat(timestamp, ":AMEX");
                UriBuilder uri = new UriBuilder(requestUri);

                //Format = timestamp \n timestamp:AMEX \n method \n path \n port \n\n
                string message = string.Format("{0}\n{1}\nPOST\n{2}\n{3}\n{4}\n\n", timestamp, nonce, Regex.Replace(HttpUtility.UrlEncode(uri.Path.ToLowerInvariant()), @"%[a-f0-9]{2}", c => c.Value.ToUpper()), uri.Host.ToLowerInvariant(), uri.Port);

                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(amexOAuthResponse.MacKey)))
                {
                    string mac = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
                    // Format -> MAC id="access_token",ts="timestamp",nonce="timestamp:AMEX",mac="HMACSHA256 hash"
                    string authorizationHeader = string.Format("MAC id=\"{0}\",ts=\"{1}\",nonce=\"{2}\",mac=\"{3}\"", amexOAuthResponse.AccessToken, timestamp, nonce, mac);
                    Stopwatch timer = Stopwatch.StartNew();

                    try
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Accept.Clear();
                            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("MAC", authorizationHeader.ToString());
                            httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader.ToString());
                            httpClient.DefaultRequestHeaders.Add("X-AMEX-API-KEY", ConfigurationManager.AppSettings[AmexConstants.AmexOAuthClientId]);
                            httpClient.DefaultRequestHeaders.Add("X-AMEX-MSG-ID", CommerceLog.ActivityId.ToString("N"));

                            HttpResponseMessage response = await httpClient
                                .PostAsJsonAsync(requestUri, requestPayload)
                                .ConfigureAwait(false);
                            string content = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                CommerceLog.Verbose("Received Amex AddOrRemoveCardAsync response. \r\nUri: {0} \r\nContent: {1}", requestUri, content);
                                return JsonConvert.DeserializeObject<AmexCardResponse>(content);
                            }
                            else
                            {
                                CommerceLog.Critical("Unable to get Amex AddOrRemoveCardAsync response. \r\nUri: {0} \r\nStatus: {1} \r\nContent: {2}", null, requestUri, response.StatusCode, content);
                            }
                        }
                    }
                    finally
                    {
                        timer.Stop();
                        PerformanceInformation.Add(requestUri, string.Format("{0} ms", timer.ElapsedMilliseconds));
                    }
                }
            }

            return null;
        }
    }
}