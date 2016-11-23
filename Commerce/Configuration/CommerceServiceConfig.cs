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
    /// Represents Lomo commerce service configuration.
    /// </summary>
    public class CommerceServiceConfig : CommerceConfig
    {
        /// <summary>
        /// Gets the single instance of the Lomo commerce service configuration.
        /// </summary>
        public static CommerceServiceConfig Instance
        {
            get
            {
                return instance;
            }
        }
        private static CommerceServiceConfig instance =
                                    (CommerceServiceConfig)WebConfigurationManager.GetWebApplicationSection(LomoCommerceService);

        /// <summary>
        ///  Gets the serial numbers of server certificates that can be used to authorize First Data service endpoints.
        /// </summary>
        public Collection<string> FirstDataServerCertificateSerialNumbers
        {
            get
            {
                if (firstDataServerCertificateSerialNumbers == null)
                {
                    firstDataServerCertificateSerialNumbers = new Collection<string>();
                    foreach (StringConfigurationElement stringInstance in FirstDataServerCertificateSerialNumberCollection)
                    {
                        firstDataServerCertificateSerialNumbers.Add(stringInstance.Value);
                    }
                }

                return firstDataServerCertificateSerialNumbers;
            }
        }
        internal Collection<string> firstDataServerCertificateSerialNumbers;

        /// <summary>
        /// Gets a collection of StringConfigurationElement objects containing the list of server certificate serial numbers that
        /// can be used to authorize First Data service endpoints.
        /// </summary>
        [ConfigurationProperty(FirstDataServerCertificateSerialNumbersAttribute, IsRequired = false)]
        private StringConfigurationCollection FirstDataServerCertificateSerialNumberCollection
        {
            get
            {
                return (StringConfigurationCollection)this[FirstDataServerCertificateSerialNumbersAttribute];
            }
        }

        /// <summary>
        ///  Gets the serial numbers of client certificates authorized to execute First Data service methods.
        /// </summary>
        public Collection<string> FirstDataClientCertificateSerialNumbers
        {
            get
            {
                if (firstDataClientCertificateSerialNumbers == null)
                {
                    firstDataClientCertificateSerialNumbers = new Collection<string>();
                    foreach (StringConfigurationElement stringInstance in FirstDataClientCertificateSerialNumberCollection)
                    {
                        firstDataClientCertificateSerialNumbers.Add(stringInstance.Value);
                    }
                }

                return firstDataClientCertificateSerialNumbers;
            }
        }
        internal Collection<string> firstDataClientCertificateSerialNumbers;

        /// <summary>
        /// Gets a collection of StringConfigurationElement objects containing the list of MasterCard IP addresses that are
        /// authorized to invoke the onAuthorization endpoint.
        /// </summary>
        [ConfigurationProperty(MasterCardAuthorizationIPAddressesAttribute, IsRequired = false)]
        private StringConfigurationCollection MasterCardAuthorizationIPAddressCollection
        {
            get
            {
                return (StringConfigurationCollection)this[MasterCardAuthorizationIPAddressesAttribute];
            }
        }

        /// <summary>
        ///  Gets the serial numbers of MasterCard authoriztion IP addresses.
        /// </summary>
        public Collection<string> MasterCardAuthorizationIPAddresses
        {
            get
            {
                if (masterCardAuthorizationIPAddresses == null)
                {
                    masterCardAuthorizationIPAddresses = new Collection<string>();
                    foreach (StringConfigurationElement stringInstance in MasterCardAuthorizationIPAddressCollection)
                    {
                        string ipAddress = stringInstance.Value;
                        if (String.IsNullOrWhiteSpace(stringInstance.Value) == true)
                        {
                            ipAddress = null;
                        }

                        masterCardAuthorizationIPAddresses.Add(ipAddress);
                    }
                }

                return masterCardAuthorizationIPAddresses;
            }
        }
        internal Collection<string> masterCardAuthorizationIPAddresses;

        /// <summary>
        /// Gets a collection of StringConfigurationElement objects containing the list of client certificate serial numbers that
        /// are authorized to execute First Data service methods.
        /// </summary>
        [ConfigurationProperty(FirstDataClientCertificateSerialNumbersAttribute, IsRequired = false)]
        private StringConfigurationCollection FirstDataClientCertificateSerialNumberCollection
        {
            get
            {
                return (StringConfigurationCollection)this[FirstDataClientCertificateSerialNumbersAttribute];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the debug security provider will be enabled.
        /// </summary>
        [ConfigurationProperty(EnableDebugSecurityProviderAttribute, DefaultValue = false, IsRequired = false)]
        public bool EnableDebugSecurityProvider
        {
            get
            {
                return (bool)this[EnableDebugSecurityProviderAttribute];
            }
        }

        /// <summary>
        /// Gets the key used to sign simple web tokens.
        /// </summary>
        [ConfigurationProperty(SimpleWebTokenKeyAttribute, DefaultValue = "", IsRequired = false)]
        public string SimpleWebTokenKey
        {
            get
            {
                return (string)this[SimpleWebTokenKeyAttribute];
            }
        }

        /// <summary>
        /// Gets the Mask for Rejecting cards. Mask can be specified as semi-colon seperated regex e.g "^37\d*;^[6-7]7\d*;^4444\d*8888"
        /// </summary>
        [ConfigurationProperty(CardProviderRejectionMaskAttribute, DefaultValue = "", IsRequired = false)]
        public string CardProviderRejectionMask
        {
            get
            {
                return (string)this[CardProviderRejectionMaskAttribute];
            }
        }

        /// <summary>
        /// Gets the ID of the reward to apply for user link referral events.
        /// </summary>
        [ConfigurationProperty(UserLinkReferralRewardIdAttribute, DefaultValue = "34246645-73EB-4D46-BED1-039C4447E22F", IsRequired = false)]
        public Guid UserLinkReferralRewardId
        {
            get
            {
                return (Guid)this[UserLinkReferralRewardIdAttribute];
            }
        }

        /// <summary>
        /// Gets the ID of the reward to apply for user link signup events.
        /// </summary>
        [ConfigurationProperty(UserSignupReferralRewardIdAttribute, DefaultValue = "34246645-73EB-4D46-BED1-039C4447E22F", IsRequired = false)]
        public Guid UserSignupReferralRewardId
        {
            get
            {
                return (Guid)this[UserSignupReferralRewardIdAttribute];
            }
        }

        /// <summary>
        /// Gets the amount of the reward to apply for user link referral events.
        /// </summary>
        [ConfigurationProperty(ReferredUserFirstEarnRewardAmountAttribute, DefaultValue = 500, IsRequired = false)]
        public int ReferredUserFirstEarnRewardAmount
        {
            get
            {
                return (int)this[ReferredUserFirstEarnRewardAmountAttribute];
            }
        }

        /// <summary>
        /// Gets the explanation for the reward to apply for user link referral events.
        /// </summary>
        [ConfigurationProperty(ReferredUserFirstEarnRewardExplanationAttribute, DefaultValue = "Earn Credit Referral Bonus", IsRequired = false)]
        public string ReferredUserFirstEarnRewardExplanation
        {
            get
            {
                return (string)this[ReferredUserFirstEarnRewardExplanationAttribute];
            }
        }

        /// <summary>
        /// The name of the Lomo commerce service configuration section.
        /// </summary>
        private const string LomoCommerceService = "lomoCommerceService";

        /// <summary>
        /// The commerce configuration element's FirstDataServerCertificateSerialNumbers attribute name.
        /// </summary>
        private const string FirstDataServerCertificateSerialNumbersAttribute = "firstDataServerCertificateSerialNumbers";

        /// <summary>
        /// The commerce configuration element's FirstDataClientCertificateSerialNumbers attribute name.
        /// </summary>
        private const string FirstDataClientCertificateSerialNumbersAttribute = "firstDataClientCertificateSerialNumbers";

        /// <summary>
        /// The commerce configuration element's MasterCardAuthorizationIPAddresses attribute name.
        /// </summary>
        private const string MasterCardAuthorizationIPAddressesAttribute = "masterCardAuthorizationIPAddresses";

        /// <summary>
        /// The commerce configuration element's EnableDebugSecurityProvider attribute name.
        /// </summary>
        private const string EnableDebugSecurityProviderAttribute = "enableDebugSecurityProvider";
        
        /// <summary>
        /// The commerce configuration element's SimpleWebTokenKey attribute name.
        /// </summary>
        private const string SimpleWebTokenKeyAttribute = "simpleWebTokenKey";

        /// <summary>
        /// The commerce configuration element's CardProviderRejectionMask attribute name.
        /// </summary>
        private const string CardProviderRejectionMaskAttribute = "cardProviderRejectionMask";

        /// <summary>
        /// The commerce configuration element's UserLinkReferralRewardId attribute name.
        /// </summary>
        private const string UserLinkReferralRewardIdAttribute = "userLinkReferralRewardId";

        /// <summary>
        /// The commerce configuration element's UserSignupReferralRewardId attribute name.
        /// </summary>
        private const string UserSignupReferralRewardIdAttribute = "userSignupReferralRewardId";

        /// <summary>
        /// The commerce configuration element's ReferredUserFirstEarnRewardAmount attribute name.
        /// </summary>
        private const string ReferredUserFirstEarnRewardAmountAttribute = "referredUserFirstEarnReward";

        /// <summary>
        /// The commerce configuration element's ReferredUserFirstEarnRewardExplanation attribute name.
        /// </summary>
        private const string ReferredUserFirstEarnRewardExplanationAttribute = "referredUserFirstEarnRewardExplanation";
    }
}