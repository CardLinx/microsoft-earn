//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Service.HighSecurity
{
    using System;
    using System.Web.Optimization;
    
    public static class BundleConfig
    {
        /// <summary>
        /// Registers script and style files into bundles.
        /// </summary>
        /// <param name="bundles">
        /// The BundleCollection in which to register the files.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Parameter bundles cannot be null.
        /// </exception>
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles == null)
            {
                throw new ArgumentNullException("bundles", "Parameter bundles cannot be null.");
            }

            bundles.Add(new ScriptBundle("~/client/jsBundles/addCardV2")
                            .Include("~/Client/Scripts/cardsControllerProxy.js")
                            .Include("~/Client/Scripts/logger.js")
                            .Include("~/Client/Scripts/configuration.js")
                            .Include("~/Client/Scripts/addCardMainV2.js")
                            .Include("~/Client/Scripts/messaging.js"));

            bundles.Add(new ScriptBundle("~/client/jsBundles/addCardV3")
                         .Include("~/Client/Scripts/cardsControllerProxy.js")
                         .Include("~/Client/Scripts/logger.js")
                         .Include("~/Client/Scripts/configuration.js")
                         .Include("~/Client/Scripts/addCardMainV3.js")
                         .Include("~/Client/Scripts/messaging.js"));

            bundles.Add(new ScriptBundle("~/client/jsBundles/addCardSingleStep")
                       .Include("~/Client/Scripts/cardsControllerProxy.js")
                       .Include("~/Client/Scripts/logger.js")
                       .Include("~/Client/Scripts/configuration.js")
                       .Include("~/Client/Scripts/UnAuthenticated/singleStep.js")
                       .Include("~/Client/Scripts/messaging.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}