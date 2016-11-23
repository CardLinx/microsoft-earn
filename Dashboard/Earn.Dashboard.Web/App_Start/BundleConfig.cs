//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Web.Optimization;
using Earn.Dashboard.Web.Utils;

namespace Earn.Dashboard.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Scripts
            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                "~/Scripts/jquery.signalR-2.2.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/layoutJs").Include(
                "~/admin-lte/js/app.js"));
                //"~/Scripts/app/chat.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportsJs").Include(
                "~/Scripts/etc/utils.js",
                "~/Scripts/PageModels/datatableDefinitions.js",
                "~/Scripts/widgets/extendedTableWidget.js",
                "~/Scripts/widgets/barChartWidget.js",
                "~/Scripts/widgets/pieChartWidget.js",
                "~/Scripts/widgets/lineChartWidget.js",
                "~/Scripts/PageModels/webAnalytics.js",
                "~/Scripts/Services/commerceService.js"));

            bundles.Add(new ScriptBundle("~/bundles/merchantReportJs").Include(
                "~/Scripts/etc/utils.js",
                "~/Scripts/etc/knockout.bindingHandlers.js",
                "~/Scripts/widgets/extendedTableWidget.js",
                "~/Scripts/widgets/tableWidget.js",
                "~/Scripts/jquery-deparam.js",
                "~/Scripts/PageModels/datatableDefinitions.js",
                "~/Scripts/PageModels/merchantReportViewModel.js",
                "~/Scripts/Services/commerceService.js"));

            bundles.Add(new ScriptBundle("~/bundles/supportJs").Include(
                "~/Scripts/etc/utils.js",
                "~/Scripts/etc/knockout.bindingHandlers.js",
                "~/Scripts/widgets/extendedTableWidget.js",
                "~/Scripts/widgets/tableWidget.js",
                "~/Scripts/jquery-deparam.js",
                "~/Scripts/inputmask.js",
                "~/Scripts/inputmask.extensions.js",
                "~/Scripts/inputmask.numeric.extensions.js",
                "~/Scripts/jquery.inputmask.js",
                "~/Scripts/PageModels/datatableDefinitions.js",
                "~/Scripts/PageModels/customerInfoViewModel.js",
                "~/Scripts/PageModels/notesTimelineViewModel.js",
                "~/Scripts/Services/lomoUsersService.js",
                "~/Scripts/Services/commerceService.js",
                "~/Scripts/etc/customjQueryValidators.js"));

            bundles.Add(new ScriptBundle("~/bundles/providerJs").Include(
                "~/Scripts/PageModels/datatableDefinitions.js",
                "~/Scripts/widgets/extendedTableWidget.js",
                "~/Scripts/PageModels/merchantViewModel.js",
                "~/Scripts/PageModels/providerViewModel.js",
                "~/Scripts/etc/knockout.bindingHandlers.js",
                "~/Scripts/etc/utils.js",
                "~/Scripts/app/merchantsView.js",
                "~/Scripts/etc/customjQueryValidators.js"));

            // Styles
            bundles.Add(new StyleBundle("~/bundles/layoutCss").Include(
                "~/admin-lte/css/AdminLTE.min.css",
                "~/admin-lte/css/skins/skin-blue.min.css",
                "~/Content/app.css"));

            bundles.Add(new StyleBundle("~/bundles/reportsCss").Include(
                "~/Content/webanalytics.min.css"));

            bundles.Add(new StyleBundle("~/bundles/supportCss").Include(
                "~/Content/Support.min.css"));

            bundles.Add(new StyleBundle("~/bundles/providersCss").Include(
                "~/Content/Providers.min.css",
                "~/Content/queryBuilder.min.css"));

            BundleTable.EnableOptimizations = Config.IsProduction;
        }
    }
}