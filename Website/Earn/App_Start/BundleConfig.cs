//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Earn.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterCssBundles(bundles);
            RegisterJsBundles(bundles);
        }

        private static void RegisterCssBundles(BundleCollection bundles)
        {
            StyleBundle learnMoreBundle = new StyleBundle("~/earn/styles/learnmorestylebundle");
            learnMoreBundle.Include("~/offers/earn/content/css/reset.min.css");
            learnMoreBundle.Include("~/offers/earn/content/css/footer.min.css");
            learnMoreBundle.Include("~/offers/earn/content/css/learn.min.css");
            learnMoreBundle.Include("~/offers/earn/content/css/animate.css");
            learnMoreBundle.Include("~/offers/earn/content/css/socialbar.css");
            bundles.Add(learnMoreBundle);

            StyleBundle faqBundle = new StyleBundle("~/earn/styles/faqBundle");
            faqBundle.Include("~/offers/earn/content/css/reset.min.css");
            faqBundle.Include("~/offers/earn/content/css/footer.min.css");
            faqBundle.Include("~/offers/earn/content/css/faq.min.css");
            faqBundle.Include("~/offers/earn/content/css/socialbar.min.css");
            bundles.Add(faqBundle);

            StyleBundle accountsBundle = new StyleBundle("~/earn/styles/accountsBundle");
            accountsBundle.Include("~/offers/earn/content/css/reset.min.css");
            accountsBundle.Include("~/offers/earn/content/css/footer.min.css");
            accountsBundle.Include("~/offers/earn/content/css/accounts.min.css");
            accountsBundle.Include("~/offers/earn/content/css/socialbar.min.css");
            bundles.Add(accountsBundle);

            StyleBundle enrollBundle = new StyleBundle("~/earn/styles/enrollBundle");
            enrollBundle.Include("~/offers/earn/content/css/reset.min.css");
            enrollBundle.Include("~/offers/earn/content/css/footer.min.css");
            enrollBundle.Include("~/offers/earn/content/css/enroll.min.css");
            enrollBundle.Include("~/offers/earn/content/css/socialbar.css");
            bundles.Add(enrollBundle);
        }

        private static void RegisterJsBundles(BundleCollection bundles)
        {
            ScriptBundle accountsBundle = new ScriptBundle("~/earn/scripts/accountsscriptBundle");
            accountsBundle.Include("~/offers/earn/content/js/configuration.js");
            accountsBundle.Include("~/offers/earn/content/js/helper.js");
            accountsBundle.Include("~/offers/earn/content/js/cookie.js");
            accountsBundle.Include("~/offers/earn/content/js/messaging.js");
            accountsBundle.Include("~/offers/earn/content/js/bouxAnalytics.js");
            accountsBundle.Include("~/offers/earn/content/js/socialPlugin.js");
            accountsBundle.Include("~/offers/earn/content/js/userServices.js");
            accountsBundle.Include("~/offers/earn/content/js/cardInfo.js");
            accountsBundle.Include("~/offers/earn/content/js/login.js");
            accountsBundle.Include("~/offers/earn/content/js/referralCodeViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/cardsControllerProxy.js");
            accountsBundle.Include("~/offers/earn/content/js/businessViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/dealViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/contactInfoViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/registeredCardsViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/localDealsViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/supportViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/settingsViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/notification.js");
            accountsBundle.Include("~/offers/earn/content/js/notificationsViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/bingMapViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/referralsViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/transactionHistoryViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/accountMainViewModel.js");
            accountsBundle.Include("~/offers/earn/content/js/enrollViewModel.js");
            bundles.Add(accountsBundle);
        }
    }
}