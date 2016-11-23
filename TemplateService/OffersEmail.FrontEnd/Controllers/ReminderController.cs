//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary></summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.FrontEnd.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using DataContracts;
    using Newtonsoft.Json;

    /// <summary>
    /// Reminder emails controller
    /// </summary>
    public class ReminderController : Controller
    {
        /// <summary>
        /// The referrer
        /// </summary>
        private const string Referrer = "BO_EMAIL";

        /// <summary>
        /// Signup action
        /// </summary>
        /// <returns>Return the view</returns>
        public ActionResult Signup()
        {
            return View();
        }

        /// <summary>
        /// Activates the account.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// Return the view
        /// </returns>
        [HttpPost]
        public ActionResult ActivateAccount(string campaign, string referrer)
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Content) && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl))
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Content or UnsubscribeUrl is missing");
        }

        /// <summary>
        /// Activate account template B.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The city.</param>
        /// <returns>The activate account email template view</returns>
        [HttpPost]
        public ActionResult ActivateAccountB(string campaign = "", string referrer = "S")
        {
            ViewBag.campaign = campaign;
            ViewBag.referrer = Referrer;

            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.Content) && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl))
                    {
                        switch (referrer.ToUpperInvariant())
                        {
                            case "B":
                                ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x180-Bos-05212014.jpg";
                                break;
                            case "P":
                                ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x185-Phx-05212014.jpg";
                                break;
                            default:
                                ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x180-Sea-05212014.jpg";
                                break;
                        }

                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Content or UnsubscribeUrl is missing");
        }

        /// <summary>
        /// Deprecating Face-book login support message.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// Return the view
        /// </returns>
        [HttpPost]
        public ActionResult DeprecateFacebook(string campaign, string referrer)
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl))
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UnsubscribeUrl is missing");
        }

        /// <summary>
        /// Outlooks the mass email.
        /// </summary>
        /// <param name="city">The city.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        /// The outlook mass email view
        /// </returns>
        [HttpGet]
        public ActionResult OutlookMassEmail(string city, string version)
        {
            ViewBag.GiftCardIncentive = false;
            var model = new CampaignDataContract();
            var businesses = new List<string>();

            switch (city.ToUpperInvariant())
            {
                case "B":
                    model.Location = "Boston Area";
                    model.PostalCode = "Boston, MA";
                    businesses.Add("Jose's Mexican Food Restaurant");
                    businesses.Add("Angelina's Italian Restaurant and Pizzeria");
                    businesses.Add("Manhattan Sandwich Company");
                    businesses.Add("Midwest Grill");
                    businesses.Add("Alexander Salon");
                    businesses.Add("Danish Pastry House");
                    businesses.Add("Frances Ray Jules Salon");
                    businesses.Add("Teddy Shoes");
                    businesses.Add("Golden Temple");
                    businesses.Add("Women's Fitness of Boston");
                    businesses.Add("Slice, Pizza & More");
                    businesses.Add("Taqueria La Tapatia");
                    businesses.Add("Tomasso Trattoria");
                    businesses.Add("Odessa Instant Shoe Repair");
                    businesses.Add("Happy Feet Spa");
                    businesses.Add("Halisi Day Spa & Salon");
                    businesses.Add("Mozi Foot Spa");
                    businesses.Add("Beantown Taqueria");
                    businesses.Add("Timber Lanes");
                    businesses.Add("Super Duck Tours LLC");
                    businesses.Add("Golden Temple");
                    businesses.Add("Twentieth Century Ltd Goods");
                    businesses.Add("Newbury Yarns");
                    businesses.Add("Jordan & Company");
                    businesses.Add("THE GARMENT DISTRICT");
                    businesses.Add("Five Bites Cupcakes");
                    businesses.Add("Debbies Kitchen");
                    ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x180-Bos-05212014.jpg";
                    break;
                case "P":
                    model.Location = "Phoenix Area";
                    model.PostalCode = "Phoenix, AZ";
                    businesses.Add("The Wild Side Grill");
                    businesses.Add("Famous Ray's Pizza");
                    businesses.Add("Cold Stone Creamery");
                    businesses.Add("Smoothie King Phoenix");
                    businesses.Add("Manny's Mexican Restaurant");
                    businesses.Add("Tart & Toppings");
                    businesses.Add("Cafe North");
                    businesses.Add("Pizza Heaven Bistro");
                    businesses.Add("Sweetie Peaz Frozen Yogurt");
                    businesses.Add("Fu-Fu Cuisine");
                    businesses.Add("Al Hamra");
                    businesses.Add("Swirl It Self Serve Froyo");
                    businesses.Add("Active Lifestyle Clinic");
                    businesses.Add("SpaMassage For You");
                    businesses.Add("Anna's Spa Salon");
                    businesses.Add("Water 'n Ice");
                    businesses.Add("The Salon Spot");
                    businesses.Add("Tranquil Massage LLC");
                    businesses.Add("Bicycles of Phoenix");
                    businesses.Add("2b mod");
                    businesses.Add("Arizona Archery Club");
                    businesses.Add("Me The Artist");
                    businesses.Add("Nomad Self Storage");
                    businesses.Add("Juba");
                    ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x185-Phx-05212014.jpg";
                    break;
                default:
                    model.Location = "Seattle Area";
                    model.PostalCode = "Seattle, WA";
                    businesses.Add("Pizza hut");
                    businesses.Add("Willie's Taste of Soul");
                    businesses.Add("Bengal Tiger Indian Restaurant");
                    businesses.Add("Garam Masala");
                    businesses.Add("JJ's Gourmet Burgers");
                    businesses.Add("Amante Pizza and Pasta");
                    businesses.Add("Kylie's Chicago Pizza");
                    businesses.Add("Indian Curry Corner");
                    businesses.Add("Choi's Mongolian Grill");
                    businesses.Add("Papaya Vietnamese Café");
                    businesses.Add("Old Country Bakery");
                    businesses.Add("Blue Ginger Korean Grill & Sushi");
                    businesses.Add("Blue Martini");
                    businesses.Add("Wingstop");
                    businesses.Add("Gyros House");
                    businesses.Add("Bernard's On Seneca");
                    businesses.Add("Cactus Club Tanning Salon");
                    businesses.Add("Salon Remeek");
                    businesses.Add("Salon Domani");
                    businesses.Add("Newport Massage Therapy");
                    businesses.Add("Pro Fitness");
                    businesses.Add("Barefoot Yoga");
                    businesses.Add("Bethel Hair Salon");
                    businesses.Add("Lucca Espresso");
                    businesses.Add("Buca Di Beppo");
                    businesses.Add("Roberts Music Institute");
                    businesses.Add("Metropolitan Music");
                    businesses.Add("Seattle Fudge");
                    businesses.Add("Federal Way Custom Jewelers");
                    businesses.Add("Merchant's Paper & Design");
                    businesses.Add("Allstar Safe and Lock");
                    businesses.Add("Automotive Expert");
                    businesses.Add("Hawk's Prairie Automotive");
                    businesses.Add("Fairwood Cleaners");
                    businesses.Add("Mallory Paint Store");
                    businesses.Add("Shoe Express");
                    ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x180-Sea-05212014.jpg";
                    city = "S";
                    break;
            }

            if (version.Equals("GC", StringComparison.InvariantCultureIgnoreCase))
            {
                ViewBag.Heading = "Activate your debit or credit card for exclusive savings at local businesses.";
                ViewBag.Image = "https://az414848.vo.msecnd.net/assets/270x185-Tango-06022014.png";
                ViewBag.GiftCardIncentive = true;
            }
            else if (version.Equals("B", StringComparison.InvariantCultureIgnoreCase))
            {
                ViewBag.Heading = "Activate your credit or debit card for exclusive savings. Membership is FREE!";
            }
            else
            {
                ViewBag.Heading = "Become a Card-Linked member to access exclusive discounts at local businesses.";
                version = "A";
            }

            ViewBag.referrer = string.Concat("OL_", version.ToUpperInvariant(), "_", city.ToUpperInvariant());
            model.Content = string.Concat("http://www.bing.com/offers/card-linked-signup/?boab=5&bor=", ViewBag.referrer);
            ViewBag.Businesses = businesses;
            return View(model);
        }

        /// <summary>
        /// TestBingOffersUpdateNotice email template.
        /// </summary>
        /// <param name="campaign">The campaign.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>
        /// Return the view
        /// </returns>
        [HttpPost]
        public ActionResult BingOffersUpdateNotice(string campaign, string referrer)
        {
            if (Request.ContentLength == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nothing was submitted");
            }

            ViewBag.campaign = campaign;
            ViewBag.referrer = referrer;
            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var model = JsonConvert.DeserializeObject<CampaignDataContract>(content);
                    if (model != null && !string.IsNullOrWhiteSpace(model.UnsubscribeUrl))
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "UnsubscribeUrl is missing");
        }
    }
}