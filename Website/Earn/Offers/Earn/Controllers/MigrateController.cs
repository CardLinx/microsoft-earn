//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Earn.Offers.Earn.Attributes;
using Lomo.Logging;
using ProfileService.Utility;

namespace Earn.Offers.Earn.Controllers
{
    [MicrosoftAccountAuthentication]
    public class MigrateController : Controller
    {
        /// <summary>
        /// Ingestion certificate
        /// </summary>
        private const string CertThumbprint = "9E257175BE4FD9AED4BF7903A4E9E3F2184484A0";

        /// <summary>
        /// enrolls the user registered card brand to the earn program
        /// defaults to mastercard
        /// </summary>
        /// <param name="ut">The user token</param>
        /// <returns>The view</returns>
        public async Task<ActionResult> Index(string ut)
        {
            if (string.IsNullOrWhiteSpace(ut))
            {
                Log.Error("MigrateController: invalid user token, redirecting to home page");
            }
            else
            {
                try
                {
                    LiveIdAuthResult liveIdAuthResult = HttpContext.Items["liveauthstate"] as LiveIdAuthResult;

                    if (liveIdAuthResult != null)
                    {
                        if (liveIdAuthResult.IsUserAuthenticated)
                        {
                            ViewBag.ProfileName = liveIdAuthResult.ProfileName;
                            ViewBag.LoginHtmlLink = liveIdAuthResult.SignOutHtmlLink;
                        }
                        else
                        {
                            ViewBag.LoginHtmlLink = liveIdAuthResult.SignInHtmlLink;
                        }
                    }

                    var content = new { user_id = Cipher.Decrypt(ut, WebConfigurationManager.AppSettings["MigrateUserDecryptKey"]), card_brands = new[] { "MasterCard" }, reward_programs = new[] { "EarnBurn" } };

                    using (var handler = new WebRequestHandler())
                    {
                        X509Certificate2 cert = TryGetCertificate(CertThumbprint);
                        if (cert != null)
                        {
                            handler.ClientCertificates.Add(cert);

                            using (HttpClient client = new HttpClient(handler))
                            {
                                HttpResponseMessage response = await client.PutAsJsonAsync(WebConfigurationManager.AppSettings["MigrateUserUri"], content);
                                response.EnsureSuccessStatusCode();
                                Log.Info("MigrateController: user {0} enrolled successfully.", ut);
                                return View("~/offers/earn/views/migrate/index.cshtml");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "MigrateController: Decrypting the token {0} failed.", ut);
                }
            }

            return RedirectToRoute("Default");
        }

        public static X509Certificate2 TryGetCertificate(string certificateThumbprint)
        {
            X509Certificate2 result = null;
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var foundCerts = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
            if (foundCerts.Count > 0)
            {
                result = foundCerts[0];
            }

            store.Close();
            return result;
        }
    }
}