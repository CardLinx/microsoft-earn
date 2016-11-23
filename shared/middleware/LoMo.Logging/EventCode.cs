//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   LoMo event codes. Each logical component have a range of up to 100 event ids
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Logging
{
    /// <summary>
    /// LoMo event codes. Each logical component have a range of up to 100 event ids
    /// </summary>
    public static class EventCode
    {
        /// <summary>
        /// Non Monitored event code
        /// Allocated range 1-100
        /// </summary>
        public const int NotMonitored = 0;

        // Allocate range 1-100
        #region Users DAL

        /// <summary>
        /// The user federation refresh error.
        /// </summary>
        public const int UserFederationRefreshError = 1;

        #endregion

        // Allocate range 101-200
        #region User Services

        // Allocate range 101-110
        #region Emailing Worker Role

        /// <summary>
        /// The email agent started. 
        /// </summary>
        public const int DealsEmailingStarted = 101;

        /// <summary>
        /// The email agent stopped.
        /// </summary>
        public const int DealsEmailingStopped = 102;

        /// <summary>
        /// The email agent storage access error.
        /// </summary>
        public const int DealsEmailingEntryPoint = 103;

        /// <summary>
        /// Emailing template rendering error.
        /// </summary>
        public const int DealsEmailingUnexpectedError = 104;

        /// <summary>
        /// email role initialize error
        /// </summary>
        public const int DealsEmailingInitializeError = 105;

        #endregion

        // Allocate range 111-120
        #region Email Agent

        /// <summary>
        /// email agent started
        /// </summary>
        public const int EmailAgentStarted = 111;

        /// <summary>
        /// email agent stopped
        /// </summary>
        public const int EmailAgentStopped = 112;

        /// <summary>
        /// email agent storage access error
        /// </summary>
        public const int EmailAgentStorageAccessError = 113;

        /// <summary>
        /// email agent template rendering error
        /// </summary>
        public const int EmailAgentTemplateRenderingError = 114;

        /// <summary>
        /// json Serialization error
        /// </summary>
        public const int EmailAgentJsonSerializationError = 115;

        /// <summary>
        /// email agent unexpected error
        /// </summary>
        public const int EmailAgentUnexpectedError = 116;

        #endregion

        // Allocate range 121-130
        #region User Service Frondend

        /// <summary>
        /// The user services initialized.
        /// </summary>
        public const int UserServicesInitialized = 121;

        /// <summary>
        /// The user services initialized error.
        /// </summary>
        public const int UserServicesInitializedError = 122;

        #endregion

        // Allocate range 131-140
        #region Unsubscribe link generator

        /// <summary>
        /// Unsubscribe link generation entry point.
        /// </summary>
        public const int UnsubscribeLinkGeneratorEntryPoint = 131;

        /// <summary>
        /// Unsubscribe link generation initialize error.
        /// </summary>
        public const int UnsubscribeLinkGeneratorInitializeError = 132;

        /// <summary>
        /// Unsubscribe link generation  agent started
        /// </summary>
        public const int UnsubscribeLinkGeneratorStarted = 133;

        /// <summary>
        /// Unsubscribe link generation  agent stopped
        /// </summary>
        public const int UnsubscribeLinkGeneratorStopped = 134;

        /// <summary>
        /// Error while generating a link to a user
        /// </summary>
        public const int UnsubscribeLinkGenerationError = 135;

        /// <summary>
        /// Unsubscribe link generation  unexpected error
        /// </summary>
        public const int UnsubscribeLinkGeneratorUnexpectedError = 136;

        #endregion

        // Allocate range 141-150
        #region HCP Email Subscription

        /// <summary>
        /// The email subscription entry point.
        /// </summary>
        public const int EmailSubscriptionWorkerEntryPoint = 141;

        /// <summary>
        /// The email subscription initialize error.
        /// </summary>
        public const int EmailSubscriptionInitializeError = 142;

        /// <summary>
        /// The email subscription unexpected error.
        /// </summary>
        public const int EmailSubscriptionUnexpectedError = 143;

        /// <summary>
        /// The email subscription storage error.
        /// </summary>
        public const int EmailSubscriptionStorageError = 144;

        /// <summary>
        /// The email subscription hcp command not processed.
        /// </summary>
        public const int EmailSubscriptionHcpCommandNotProcessed = 145;

        /// <summary>
        /// The email subcription hcp processor started.
        /// </summary>
        public const int EmailSubscriptionHcpProcessorStarted = 146;

        /// <summary>
        /// The email subcription hcp commands processor stopped.
        /// </summary>
        public const int EmailSubscriptionHcpCommandsProcessorStopped = 147;

        #endregion

        #endregion

        // Allocate range 201-300
        #region Deals Server

        /// <summary>
        /// deals server buy redirect unexpected error
        /// </summary>
        public const int DealsServerBuyRedirectionUnexpectedError = 201;

        #endregion

        // Allocate range 301-400
        #region Deals Server Analytics Worker

        /// <summary>
        /// Analytics worker started event id
        /// </summary>
        public const int AnalyticsWorkerStarted = 301;

        /// <summary>
        /// Unexpected error event id
        /// </summary>
        public const int AnalyticsWorkerUnexpectedError = 302;

        /// <summary>
        /// storage exception error event id
        /// </summary>
        public const int AnalyticsWorkerStorageError = 303;

        /// <summary>
        /// The analytics worker find parent event error.
        /// </summary>
        public const int AnalyticsWorkerFindParentEventError = 304;

        /// <summary>
        /// The analytics worker find parent event error.
        /// </summary>
        public const int HolMonReportGeneratorWorkerUnexpectedError = 305;

        /// <summary>
        /// Unexpected error event id for cosmos worker
        /// </summary>
        public const int CosmosWorkerUnexpectedError = 306;

        #endregion

        #region Analytics Aggregation

        /// <summary>
        /// The analytics aggregation role started.
        /// </summary>
        public const int AnalyticsAggregationRoleStarted = 401;

        /// <summary>
        /// The analytics aggregation role entry point.
        /// </summary>
        public const int AnalyticsAggregationRoleEntryPoint = 402;

        /// <summary>
        /// The analytics aggregation agent started.
        /// </summary>
        public const int AnalyticsAggregationAgentStarted = 403;

        /// <summary>
        /// The analytics aggregation agent stopped.
        /// </summary>
        public const int AnalyticsAggregationAgentStopped = 404;

        /// <summary>
        /// The analytics aggregation sql error.
        /// </summary>
        public const int AnalyticsAggregationSqlError = 405;

        /// <summary>
        /// The analytics aggregation unexpected error.
        /// </summary>
        public const int AnalyticsAggregationStorageError = 406;

        /// <summary>
        /// The analytics aggregation unexpected error.
        /// </summary>
        public const int AnalyticsAggregationUnexpectedError = 407;

        /// <summary>
        /// The analytics aggregation initialize error.
        /// </summary>
        public const int AnalyticsAggregationInitializeError = 408;

        #endregion

        // Allocate range 501-600
        #region Deals Server Logs Worker

        /// <summary>
        /// Logs worker started event id
        /// </summary>
        public const int DealsServerLogsWorkerRoleStarted = 501;

        /// <summary>
        /// Logs worker ended event id
        /// </summary>
        public const int DealsServerLogsWorkerRoleEnded = 502;

        /// <summary>
        /// Logs worker unexpected error event id
        /// </summary>
        public const int DealsServerLogsWorkerRoleUnexpectedError = 503;

        /// <summary>
        /// The deals server WAD logs worker started
        /// </summary>
        public const int DealsServerWADLogsWorkerStarted = 504;

        /// <summary>
        /// The deals server WAD logs worker ended
        /// </summary>
        public const int DealsServerWADLogsWorkerEnded = 505;

        /// <summary>
        /// The deals server WAD logs worker unexpected error
        /// </summary>
        public const int DealsServerWADLogsWorkerUnexpectedError = 506;

        #endregion

        // Allocate range 601-700
        #region Analytics Client

        /// <summary>
        /// The analytics client unexpected error.
        /// </summary>
        public const int AnalyticsClientUnexpectedError = 601;

        /// <summary>
        /// The analytics client storage error
        /// </summary>
        public const int AnalyticsClientStorageError = 602;

        public const int HolMonAnalytics = 603;

        #endregion

        // Allocate range 701-800
        #region Conversion Report Processor

        /// <summary>
        /// The analytics conversion role started.
        /// </summary>
        public const int AnalyticsConversionReportProcessorStarted = 701;

        /// <summary>
        /// The analytics conversion report processor unexpected error.
        /// </summary>
        public const int AnalyticsConversionReportProcessorUnexpectedError = 702;

        /// <summary>
        /// The analytics conversion report agent started.
        /// </summary>
        public const int AnalyticsConversionReportAgentStarted = 703;

        /// <summary>
        /// The analytics conversion report agent stopped.
        /// </summary>
        public const int AnalyticsConversionReportAgentStopped = 704;

        #endregion

        // Allocate range 801-900
        #region Hadoop Data Uploader

        /// <summary>
        /// The analytics conversion role started.
        /// </summary>
        public const int AnalyticsHadoopDataUploaderStarted = 801;

        /// <summary>
        /// The analytics conversion report processor unexpected error.
        /// </summary>
        public const int AnalyticsHadoopDataUploaderUnexpectedError = 802;

        #endregion

        // Allocate range 901-1000
        #region Bing Offers WebSite

        /// <summary>
        /// The Bing Offers client error.
        /// </summary>
        public const int BingOffersClientError = 901;

        /// <summary>
        /// The bing offers controller error
        /// </summary>
        public const int BingOffersControllerError = 902;

        /// <summary>
        /// The bing offers API controller error
        /// </summary>
        public const int BingOffersApiControllerError = 903;

        /// <summary>
        /// The bing offers attribute error
        /// </summary>
        public const int BingOffersAttributeError = 904;

        /// <summary>
        /// The bing offers core service error
        /// </summary>
        public const int BingOffersCoreServiceError = 905;

        /// <summary>
        /// The bing offers operation error
        /// </summary>
        public const int BingOffersOperationError = 906;

        #endregion

        // Allocate range 1001-2000
        #region HolmonService

        /// <summary>
        /// The HolMon SelfHeal Error.
        /// </summary>
        public const int HolMonSelfHealError = 1001;

        #endregion
    }
}