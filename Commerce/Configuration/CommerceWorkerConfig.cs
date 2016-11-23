//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Configuration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Web.Configuration;

    /// <summary>
    /// Represents Lomo commerce worker configuration.
    /// </summary>
    public class CommerceWorkerConfig : CommerceConfig
    {
        /// <summary>
        /// Gets the single instance of the Lomo commerce worker configuration.
        /// </summary>
        public static CommerceWorkerConfig Instance
        {
            get
            {
                return instance;
            }
        }
        private static CommerceWorkerConfig instance =
                                      (CommerceWorkerConfig)WebConfigurationManager.GetWebApplicationSection(LomoCommerceWorker);

        /// <summary>
        /// The timeout interval to wait for a processing task to complete.
        /// </summary>
        [ConfigurationProperty(ProcessingTimeoutAttribute, IsRequired = true)]
        public TimeSpan ProcessingTimeout
        {
            get
            {
                if (processingTimeoutSet == false)
                {
                    processingTimeout = (TimeSpan)this[ProcessingTimeoutAttribute];
                    processingTimeoutSet = true;
                }

                return processingTimeout;
            }
            internal set
            {
                lock (this)
                {
                    processingTimeout = value;
                    processingTimeoutSet = true;
                }
            }
        }
        private TimeSpan processingTimeout;
        private bool processingTimeoutSet = false;

        /// <summary>
        /// Gets the URI authority for the MBI service.
        /// </summary>
        [ConfigurationProperty(MbiServiceAuthorityAttribute, DefaultValue = "http://localhost", IsRequired = false)]
        public string MbiServiceAuthority
        {
            get
            {
                return (string)this[MbiServiceAuthorityAttribute];
            }
        }

        /// <summary>
        /// Gets the endpoint for the ServiceClaimedDealsController.
        /// </summary>
        [ConfigurationProperty(ServiceClaimedDealsControllerEndpointAttribute,
                               DefaultValue = "/api/commerce/service/claimeddeals", IsRequired = false)]
        public string ServiceClaimedDealsControllerEndpoint
        {
            get
            {
                return (string)this[ServiceClaimedDealsControllerEndpointAttribute];
            }
        }

        /// <summary>
        /// Gets the endpoint for the ServiceCardsController.
        /// </summary>
        [ConfigurationProperty(ServiceCardsControllerEndpointAttribute,
                               DefaultValue = "/api/commerce/service/cards", IsRequired = false)]
        public string ServiceCardsControllerEndpoint
        {
            get
            {
                return (string)this[ServiceCardsControllerEndpointAttribute];
            }
        }

        /// <summary>
        /// Gets the endpoint for the ServiceDiscountsController.
        /// </summary>
        [ConfigurationProperty(ServiceDiscountsControllerEndpointAttribute,
                               DefaultValue = "/api/commerce/service/discounts", IsRequired = false)]
        public string ServiceDiscountsControllerEndpoint
        {
            get
            {
                return (string)this[ServiceDiscountsControllerEndpointAttribute];
            }
        }

        /// <summary>
        /// Gets the ACS client credential.
        /// </summary>
        [ConfigurationProperty(AcsClientCredentialAttribute, DefaultValue = "", IsRequired = false)]
        public string AcsClientCredential
        {
            get
            {
                return (string)this[AcsClientCredentialAttribute];
            }
        }

        /// <summary>
        /// Gets the authorization set ID under which all MasterCard cards and merchants are associated with each other for authorization events.
        /// </summary>
        [ConfigurationProperty(MasterCardAuthorizationSetIdAttribute, DefaultValue = "6344646", IsRequired = false)]
        public string MasterCardAuthorizationSetId
        {
            get
            {
                return (string)this[MasterCardAuthorizationSetIdAttribute];
            }
        }

        /// <summary>
        /// Gets the clearing set ID under which all MasterCard cards and merchants are associated with each other for clearing events.
        /// </summary>
        [ConfigurationProperty(MasterCardClearingSetIdAttribute, DefaultValue = "6344647", IsRequired = false)]
        public string MasterCardClearingSetId
        {
            get
            {
                return (string)this[MasterCardClearingSetIdAttribute];
            }
        }

        /// <summary>
        /// Gets the expected number of days between a card being added to the Commerce system and the filtering within MasterCard's system to take effect.
        /// </summary>
        [ConfigurationProperty(MasterCardExpectedFilteringDaysDeltaAttribute, DefaultValue = 3, IsRequired = false)]
        public int MasterCardExpectedFilteringDaysDelta
        {
            get
            {
                return (int)this[MasterCardExpectedFilteringDaysDeltaAttribute];
            }
        }

        /// <summary>
        /// Gets the minimum transaction value for which MasterCard will send notifications to Commerce.
        /// </summary>
        [ConfigurationProperty(MasterCardTransactionNotificationThresholdAttribute, DefaultValue = 0.01, IsRequired = false)]
        public double MasterCardTransactionNotificationThreshold
        {
            get
            {
                return (double)this[MasterCardTransactionNotificationThresholdAttribute];
            }
        }

        /// <summary>
        /// The name of the Lomo commerce worker configuration section.
        /// </summary>
        private const string LomoCommerceWorker = "lomoCommerceWorker";

        /// <summary>
        /// The commerce configuration element's FirstDataExtractProcessingTime attribute name.
        /// </summary>
        private const string FirstDataExtractProcessingTimeAttribute = "firstDataExtractProcessingTime";

        /// <summary>
        /// The commerce configuration element's FirstDataPtsProcessingTime attribute name.
        /// </summary>
        private const string FirstDataPtsProcessingTimeAttribute = "firstDataPtsProcessingTime";

        /// <summary>
        /// The commerce configuration element's FirstDataAcknowledgmentProcessingTime attribute name.
        /// </summary>
        private const string FirstDataAcknowledgmentProcessingTimeAttribute = "firstDataAcknowledgmentProcessingTime";

        /// <summary>
        /// The commerce configuration element's ProcessingTimeout attribute name.
        /// </summary>
        private const string ProcessingTimeoutAttribute = "processingTimeout";

        /// <summary>
        /// The commerce configuration element's TaskThreadPollingInterval attribute name.
        /// </summary>
        private const string TaskThreadPollingIntervalAttribute = "taskThreadPollingInterval";

        /// <summary>
        /// The commerce configuration element's MbiServiceAuthority attribute name.
        /// </summary>
        private const string MbiServiceAuthorityAttribute = "mbiServiceAuthority";

        /// <summary>
        /// The commerce configuration element's ServiceClaimedDealsControllerEndpoint attribute name.
        /// </summary>
        private const string ServiceClaimedDealsControllerEndpointAttribute = "serviceClaimedDealsControllerEndpoint";

        /// <summary>
        /// The commerce configuration element's ServiceCardsControllerEndpoint attribute name.
        /// </summary>
        private const string ServiceCardsControllerEndpointAttribute = "serviceCardsControllerEndpoint";

        ///<summary>
        /// The commerce configuration element's ServiceDiscountsControllerEndpoint attribute name.
        /// </summary>
        private const string ServiceDiscountsControllerEndpointAttribute = "serviceDiscountsControllerEndpoint";

        /// <summary>
        /// The commerce configuration element's AcsClientCredential attribute name.
        /// </summary>
        private const string AcsClientCredentialAttribute = "acsClientCredential";

        /// <summary>
        /// The commerce configuration element's MasterCardAuthorizationSetID attribute name.
        /// </summary>
        private const string MasterCardAuthorizationSetIdAttribute = "masterCardAuthorizationSetId";

        /// <summary>
        /// The commerce configuration element's MasterCardClearingSetID attribute name.
        /// </summary>
        private const string MasterCardClearingSetIdAttribute = "masterCardClearingSetId";

        /// <summary>
        /// The commerce configuration element's MasterCardExpectedFilteringDaysDelta attribute name.
        /// </summary>
        private const string MasterCardExpectedFilteringDaysDeltaAttribute = "masterCardExpectedFilteringDaysDelta";

        /// <summary>
        /// The commerce configuration element's MasterCardTransactionNotificationThreshold attribute name.
        /// </summary>
        private const string MasterCardTransactionNotificationThresholdAttribute = "masterCardTransactionNotificationThreshold";
    }
}