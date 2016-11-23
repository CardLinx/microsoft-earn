//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Lomo.Logging;

namespace Lomo.Commerce.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using Lomo.Authorization;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.Logging;
    using Lomo.Commerce.Utilities;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure;
    using Users.Dal;

    /// <summary>
    /// Performs startup actions for this Web API application.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        private static readonly Collection<string> ValidVisaQaCertificateSerialNumbers = new Collection<string>() {"0EE7905FC4FDFC8AA5B7CE6A6B7022F5"};
        /// <summary>
        /// Initializes static members of the WebApiApplication class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
         Justification = "This isn't a static field initialization for this class.")]
        static WebApiApplication()
        {
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }

        /// <summary>
        /// Performs application startup actions.
        /// </summary>
        /// <remarks>
        /// Post-conditions:
        /// * Application startup actions have been performed.
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
         Justification = "This method cannot be static, because MVC requires this signature.")]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterExternalServices();
        }

        /// <summary>
        /// Registers external services for commerce like MVC constructs, security providers and Analytics
        /// </summary>
        internal static void RegisterExternalServices()
        {
            // Use only for debugging
            // TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("APPINSIGHTS_INSTRUMENTATIONKEY");
            
            // Register log.
            LogInitializer.CreateLogInstance(CommerceServiceConfig.Instance.LogVerbosity,
                                             CommerceServiceConfig.Instance.ForceEventLog,
                                             General.CommerceLogSource,
                                             CommerceServiceConfig.Instance);

            Log.Info("Started Commerce MBI service.");
             
            // Register MVC constructs.
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Bing FrontDoor and user debug security providers.
            IUsersDal usersDal = PartnerFactory.UsersDal(CommerceServiceConfig.Instance);
            if (CommerceServiceConfig.Instance.EnableDebugSecurityProvider == true)
            {
                Security.Add("user_debug", new UserDebugSecurityProvider(usersDal));
            }

            Security.Add("lomo", new LomoSecurityProvider(usersDal));

            // Register the mutual SSL security provider.
            Security.Add(MutualSslSecurityProvider.Name, new MutualSslSecurityProvider());

            // Register the Simple Web Token security provider.
            string environment = string.Concat("commerce-", CommerceServiceConfig.Instance.Environment);
            string resourceTemplate = string.Concat(string.Format("https://{0}.TODO_INSERT_YOUR_DOMAIN_HERE/api/commerce/service/",
                                                                  environment), "{0}");
            Security.Add(SimpleWebTokenSecurityProvider.Name,
                         new SimpleWebTokenSecurityProvider(environment, resourceTemplate,
                                                            CommerceServiceConfig.Instance.SimpleWebTokenKey));

            // Amex payment authorization SWT security provider
            Security.Add("Bearer", new SimpleWebTokenSecurityProvider(environment,
                         string.Concat(string.Format("https://{0}.TODO_INSERT_YOUR_DOMAIN_HERE/api/commerce/amex/", environment), "{0}"),
                         CommerceServiceConfig.Instance.SimpleWebTokenKey));

            // Register Analytics Service
            Analytics.Initialize(CommerceServiceConfig.Instance);
        }

        /// <summary>
        /// Validates the certificate presented by the server.
        /// </summary>
        /// <param name="sender">
        /// The sender of the certificate validation event.
        /// </param>
        /// <param name="certificate">
        /// The certificate to validate.
        /// </param>
        /// <param name="chain">
        /// X509Chain presented with the certificate.
        /// </param>
        /// <param name="error">
        /// SSL policy error code.
        /// </param>
        /// <returns>
        /// * True if the presented certificate is valid.
        /// * Else returns false.
        /// </returns>
        internal static bool ValidateServerCertificate(object sender,
                                                       X509Certificate certificate,
                                                       X509Chain chain,
                                                       SslPolicyErrors error)
        {
            bool result = true;

            //TODO: Find a way to log info on the incoming cert.
            if (error != SslPolicyErrors.None)
            {
                Collection<string> validCertificateSerialNumbers = null;
                string host = ((HttpWebRequest)sender).Address.Host;
                switch (host)
                {
                    case "api-cat.offers.firstdata.com":
                    case "api.offers.firstdata.com":
                        validCertificateSerialNumbers = CommerceServiceConfig.Instance.FirstDataServerCertificateSerialNumbers;
                        break;
                    case "maiclqaservices.visa.com":
                        // TODO: We are unable to validate visa QA certificate. This is a workaround to validate it
                        validCertificateSerialNumbers = ValidVisaQaCertificateSerialNumbers;
                        break;
                }

                var certificate2 = (X509Certificate2) certificate;

                result = IsCertificateSerialNumberValid(certificate2.SerialNumber, validCertificateSerialNumbers);
            }

            return result;
        }

        /// <summary>
        /// Determines if the specified server certificate serial number is valid based on a list of valid serial numbers.
        /// </summary>
        /// <param name="serialNumber">
        /// The serial number of the certificate whose validity to check.
        /// </param>
        /// <param name="validSerialNumbers">
        /// The list of serial numbers for valid certificates.
        /// </param>
        /// <returns>
        /// * True if the specified certificate serial number is valid.
        /// * Else returns false.
        /// </returns>
        internal static bool IsCertificateSerialNumberValid(string serialNumber,
                                                            Collection<string> validSerialNumbers)
        {
            bool result = false;

            if (validSerialNumbers == null)
            {
                return false;
            }

            // Certificate is valid if its serial number matches one of the listed valid serial numbers, regardless of case, or
            // if it is null, empty, or whitespace and one of the valid serial numbers is also null, empty, or whitespace.
            foreach (string validSerialNumber in validSerialNumbers)
            {
                if (String.Equals(serialNumber, validSerialNumber, StringComparison.OrdinalIgnoreCase) == true ||
                    (String.IsNullOrWhiteSpace(serialNumber) == true &&
                     String.IsNullOrWhiteSpace(validSerialNumber) == true))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}