//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The web api application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Lomo.Authorization;
    using Lomo.Logging;
    using LoMo.UserServices.DealsMailing;
    using Microsoft.Azure;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Newtonsoft.Json.Converters;
    using Users.Dal;
    using UserServices.FrondEnd.Controllers;
    using UserServices.FrondEnd.Email;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    /// <summary>
    /// The web api application.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        #region Constants
        
        /// <summary>
        ///     The message queue name.
        /// </summary>
        private const string EmailJobsQueueNameSetting = "LoMo.EmailJobs.Queue";

        /// <summary>
        /// The emails base path setting.
        /// </summary>
        private const string EmailsBasePathSetting = "LoMo.EmailJob.BlobBasePath";

        /// <summary>
        ///     The storage setting.
        /// </summary>
        private const string StorageSetting = "LoMo.UserServices.ConnectionString";

        /// <summary>
        ///     The Users DAL connection string
        /// </summary>
        private const string UsersDalConnectionStringSetting = "LoMo.UsersDal.ConnectionString";

        /// <summary>
        /// The debug security provider enabled.
        /// </summary>
        private const string DebugSecurityProviderEnabled = "LoMo.DebugSerucityProvider.Enabled";

        /// <summary>
        /// The bing offers base pat uri setting.
        /// </summary>
        private const string BingOffersBasePathUriSetting = "LoMo.BingOffers.BasePath";
                
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the emails base path.
        /// </summary>
        public static string EmailsBasePath { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The application_ start.
        /// </summary>
        protected void Application_Start()
        {
            // Init Logger
            var listeners = new List<TraceListener>();
            if (RoleEnvironment.IsAvailable)
            {
                listeners.Add(new DiagnosticMonitorTraceListener { Name = "AzureDiagnostics" });
            }

            if (RoleEnvironment.IsEmulated)
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            Log.Instance = new TraceLog(listeners);

            Log.Info("Initialzing User Services FE");
            try
            {
                // TODO - add authentication
                IUnityContainer container = new UnityContainer();
                this.ConfigureApiControllers(GlobalConfiguration.Configuration, container);
                this.ConfigurationControllers(container);
                this.ConfigurationFormatters();
                this.RegisterServices(container);
                
                AreaRegistration.RegisterAllAreas();
                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                // Adding Unhandled execption filter for web apis 
                GlobalConfiguration.Configuration.Filters.Add(new UnhandledExceptionFilter());

                Log.Info(EventCode.UserServicesInitialized, "User Services FE Initialization Completed ");
            }
            catch (Exception e)
            {
                Log.Critical(EventCode.UserServicesInitializedError, e, "Couldn't initialize User Services FE");

                // Sleep in order to make sure that the exception is being written to the logs
                Thread.Sleep(TimeSpan.FromSeconds(5));
                throw;
            }
        }

        /// <summary>
        /// The configuration controllers.
        /// </summary>
        /// <param name="unity">
        /// The unity.
        /// </param>
        private void ConfigurationControllers(IUnityContainer unity)
        {
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(unity));
            this.RegisterControllers(unity);
        }

        /// <summary>
        /// The configuration formatters.
        /// </summary>
        private void ConfigurationFormatters()
        {
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            var enumConverter = new StringEnumConverter();
            jsonFormatter.SerializerSettings.Converters.Add(enumConverter);
        }

        /// <summary>
        /// The configure api controllers.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="unity">
        /// The unity.
        /// </param>
        private void ConfigureApiControllers(HttpConfiguration config, IUnityContainer unity)
        {
            config.DependencyResolver = new UnityDependencyResolver(unity);
            this.RegisterApiControllers(unity);
        }

        /// <summary>
        /// The register api controllers.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private void RegisterApiControllers(IUnityContainer container)
        {
            container.RegisterType<EmailController>();
            container.RegisterType<SubscriptionsController>();
            container.RegisterType<UserInfoController>();
            container.RegisterType<MerchantController>();
            container.RegisterType<ConfirmApiController>();
            container.RegisterType<MembershipController>();
        }

        /// <summary>
        /// The register controllers.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private void RegisterControllers(IUnityContainer container)
        {
            container.RegisterType<IController, HomeController>("home");
            container.RegisterType<IController, ConfirmController>("confirm");
        }

        /// <summary>
        /// The register services.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private void RegisterServices(IUnityContainer container)
        {
            EmailsBasePath = this.GetMandatorySetting(EmailsBasePathSetting);
            string storageAccount = this.GetMandatorySetting(StorageSetting);
            string emailJobsQueueName = this.GetMandatorySetting(EmailJobsQueueNameSetting);
            string userDalConnectionString = CloudConfigurationManager.GetSetting(UsersDalConnectionStringSetting);
            bool isDebugSecurityProviderEnabled = bool.Parse(this.GetMandatorySetting(DebugSecurityProviderEnabled));
            Uri bingOffersBaseUri = new Uri(this.GetMandatorySetting(BingOffersBasePathUriSetting));

            // Read Email Confirmation Settings
            EntityConfirmationSettings entityConfirmationSettings = new EntityConfirmationSettings
                                                                      {
                                                                          BingOffersBaseUri = bingOffersBaseUri
                                                                      };

            var emailJobsQueue = new JobsQueue<EmailCargo>(storageAccount, emailJobsQueueName);
            var priorityEmailJobsQueue = new PriorityEmailJobsQueue<PriorityEmailCargo>(storageAccount);
            UsersDal usersDal = new UsersDal(userDalConnectionString, queue: priorityEmailJobsQueue);
            container.RegisterInstance(entityConfirmationSettings);
            container.RegisterInstance<IJobsQueue<EmailCargo>>(emailJobsQueue);
            container.RegisterInstance<IPriorityEmailJobsQueue<PriorityEmailCargo>>(priorityEmailJobsQueue);
            container.RegisterInstance<IUsersDal>(usersDal);
            container.RegisterInstance<IEmailClient>(new EmailSendGridClient());

            // Authentication Modules
            // Add modules that perform authorization
            if (isDebugSecurityProviderEnabled)
            {
                Security.Add("user_debug", new UserDebugSecurityProvider(usersDal));
            }

            Security.Add("lomo", new LomoSecurityProvider(usersDal));
        }

        /// <summary>
        /// Get setting from configuration file. Throw an error if the configuration not found
        /// </summary>
        /// <param name="keyName"> the key name</param>
        /// <returns> the configuration value</returns>
        private string GetMandatorySetting(string keyName)
        {
            string value = CloudConfigurationManager.GetSetting(keyName);
            if (value == null)
            {
                throw new ConfigurationErrorsException(string.Format("Key: {0} is missing is configuration file", keyName));
            }

            return value;
        }

        #endregion
    }
}