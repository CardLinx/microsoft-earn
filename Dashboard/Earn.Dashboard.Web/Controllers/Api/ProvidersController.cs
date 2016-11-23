//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.HolMon.Security;
using OfferManagement.DataModel;

namespace Earn.Dashboard.Web.Controllers.Api
{
    public class ProvidersController : ApiController
    {
        private const string MerchantApi = "api/merchant/";

        [HttpGet]
        public async Task<HttpResponseMessage> ExportMerchants(string id, PaymentProcessor paymentProcessor, string query, string providerName = null)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            using (var client = GetHttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(string.Format("{0}export?providerId={1}&paymentProcessor={2}&query={3}", MerchantApi, id, paymentProcessor, HttpUtility.UrlEncode(query)));
                if (result.IsSuccessStatusCode)
                {
                    var stream = await result.Content.ReadAsByteArrayAsync();

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(stream)
                    };

                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = string.Format("{0}-{1}.xlsx", providerName ?? "Merchants", paymentProcessor)
                    };

                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    return response;
                }
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Unknown error");
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            string primaryKey = ConfigurationManager.AppSettings["SwtPrimaryKey"];
            string secondaryKey = ConfigurationManager.AppSettings["SwtSecondaryKey"];
            KeyPair keyPair = new KeyPair(primaryKey, secondaryKey);

            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["OfferManagementApi"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    BearerAuthenticationHandler.AuthScheme,
                    SimpleWebTokenFactory.CreateToken(
                        TokenIssuer.EarnDashboard,
                        TokenAudience.EarnService,
                        DateTime.UtcNow.AddMinutes(30),
                        new List<Claim> { new Claim("User", User.Identity.Name) },
                        keyPair.PrimaryKeyToByteArray()).ToString());

            return client;
        }
    }
}