//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Earn.Dashboard.Web.Attributes;
using Earn.Dashboard.Web.Utils;
using Microsoft.HolMon.Security;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using OfferManagement.DataModel;

namespace Earn.Dashboard.Web.Controllers
{
    [AuthorizeSG(Roles = "Admin,User")]
    public class ProvidersController : Controller
    {
        private const string MerchantApi = "api/merchant/";
        private const string ProviderApi = "api/provider/";
        private const string OfferApi = "api/offer/";

        // Index page
        public async Task<ActionResult> Index()
        {
            List<Provider> providers = null;
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ProviderApi);
                if (response.IsSuccessStatusCode)
                {
                    providers = await response.Content.ReadAsAsync<List<Provider>>();
                }
            }

            return View(providers);
        }

        // Edit page
        public async Task<ActionResult> Edit(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            Provider provider = await GetProviderAsync(id);
            if (provider == null || string.IsNullOrWhiteSpace(provider.Id))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                throw new HttpException((int)HttpStatusCode.NotFound, "Provider not found");
            }

            return View(provider);
        }

        public async Task<ActionResult> GetProviderById(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            Provider result = await GetProviderAsync(id);
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public async Task<ActionResult> AddProvider(Provider provider)
        {
            if (provider == null || string.IsNullOrWhiteSpace(provider.Name))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider");
            }

            Provider result = null;
            using (var client = GetHttpClient())
            {
                provider.Name = provider.Name.Trim();
                provider.Author = User.Identity.Name;
                HttpResponseMessage response = await client.PostAsJsonAsync(ProviderApi, provider);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<Provider>();
                }
            }

            if (result == null || string.IsNullOrWhiteSpace(result.Id))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                throw new HttpException((int)HttpStatusCode.InternalServerError, "Unknown error");
            }

            return RedirectToAction("edit", new { id = result.Id });
        }

        public async Task<ActionResult> DeleteProvider(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(ProviderApi + id);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Unknown error");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProviderStatus(string id, bool status)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            Provider provider = await GetProviderAsync(id);
            provider.Author = User.Identity.Name;
            provider.IsActive = status;
            Provider result = null;
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(ProviderApi, provider);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<Provider>();
                }
            }

            if (result == null || string.IsNullOrWhiteSpace(result.Id))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                throw new HttpException((int)HttpStatusCode.InternalServerError, "Unknown error");
            }

            if (status)
            {
                //register the deal with the commerce
            }
            else
            {
                //unregister the deal with commerce
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<ActionResult> GetOffers(string id)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            List<Offer> offers = null;
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(OfferApi + "?providerId=" + id);
                if (response.IsSuccessStatusCode)
                {
                    offers = await response.Content.ReadAsAsync<List<Offer>>();
                }
            }

            //return Json(providerDetails, JsonRequestBehavior.AllowGet);
            return Content(JsonConvert.SerializeObject(offers));
        }

        [HttpPost]
        public async Task<ActionResult> AddOffer(bool active, Offer offer)
        {
            if (offer == null || string.IsNullOrWhiteSpace(offer.Title))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Offer");
            }

            Offer result = null;
            using (var client = GetHttpClient())
            {
                offer.Author = User.Identity.Name;
                HttpResponseMessage response = await client.PostAsJsonAsync(OfferApi + "?active=" + active, offer);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<Offer>();
                }
            }

            if (result == null || string.IsNullOrWhiteSpace(result.Id))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                throw new HttpException((int)HttpStatusCode.InternalServerError, "Unknown error");
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        public async Task<ActionResult> GetMerchants(string id, string query)
        {
            Guid guid;
            if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out guid))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Provider Id");
            }

            List<Merchant> merchants = null;
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(string.Format("{0}?providerId={1}&query={2}", MerchantApi, id, HttpUtility.UrlEncode(query)));
                if (response.IsSuccessStatusCode)
                {
                    merchants = await response.Content.ReadAsAsync<List<Merchant>>();
                }
            }

            return Content(JsonConvert.SerializeObject(merchants));
        }

        [HttpPost]
        public async Task<ActionResult> AddMerchants(string id, MerchantFileType merchantFileType, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase) || file.FileName.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    //string fileName = Path.GetFileName(file.FileName);
                    string fileName = string.Concat(Path.GetFileNameWithoutExtension(file.FileName),
                        DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                        Path.GetExtension(file.FileName));

                    Log.Info("uploading file '{0}' with filename '{1}'", Path.GetFileName(file.FileName), fileName);
                    CloudBlobContainer cloudBlobContainer = CloudBlobHelper.GetBlobContainer(ConfigurationManager.AppSettings["IngestionStorageConnectionString"], ConfigurationManager.AppSettings["IngestionStorageContainerName"]);
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = file.ContentType;
                    await cloudBlockBlob.UploadFromStreamAsync(file.InputStream);

                    string result = null;
                    using (var client = GetHttpClient())
                    {
                        HttpResponseMessage response = await client.PostAsJsonAsync(MerchantApi + "import", new { ProviderId = id, FileName = fileName, MerchantFileType = merchantFileType, Author = User.Identity.Name });
                        if (response.IsSuccessStatusCode)
                        {
                            result = await response.Content.ReadAsAsync<string>();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(result))
                    {
                        throw new HttpException((int)HttpStatusCode.InternalServerError, "Scheduling file processor job failed");
                    }

                    return Content("Scheduled JobId: " + result);
                }
            }

            throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid file");
        }

        [HttpPost]
        public async Task<ActionResult> AddMerchant(Merchant merchant)
        {
            if (merchant == null || string.IsNullOrWhiteSpace(merchant.Name))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid Merchant");
            }

            Merchant result = null;
            using (var client = GetHttpClient())
            {
                merchant.Author = User.Identity.Name;
                if (merchant.Payments != null)
                {
                    foreach (var payment in merchant.Payments)
                    {
                        payment.Id = Guid.NewGuid().ToString();
                        payment.IsActive = true;
                        payment.LastUpdate = DateTime.UtcNow;
                    }
                }

                HttpResponseMessage response = await client.PostAsJsonAsync(MerchantApi, merchant);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<Merchant>();
                }
            }

            if (result == null || string.IsNullOrWhiteSpace(result.Id))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                throw new HttpException((int)HttpStatusCode.InternalServerError, "Unknown error");
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        private async Task<Provider> GetProviderAsync(string id)
        {
            Provider provider = null;
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ProviderApi + id);
                if (response.IsSuccessStatusCode)
                {
                    provider = await response.Content.ReadAsAsync<Provider>();
                }
            }

            if (provider == null || string.IsNullOrWhiteSpace(provider.Id))
            {
                //return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                throw new HttpException((int)HttpStatusCode.NotFound, "Provider not found");
            }

            return provider;
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