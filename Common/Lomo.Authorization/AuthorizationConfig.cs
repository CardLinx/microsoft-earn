//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Authorization
{
    using System.Configuration;
    using System.Web.Configuration;

    /// <summary>
    ///     Represents Lomo authorization configuration.
    /// </summary>
    public class AuthorizationConfig : ConfigurationSection
    {
        #region Constants

        /// <summary>
        ///     The authorization section name.
        /// </summary>
        private const string LomoAuthorization = "lomoAuthorization";

        /// <summary>
        ///     The certificate map subsection name.
        /// </summary>
        private const string CertificateMapAttribute = "CertificateMap";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the single instance of the Lomo authorization configuration.
        /// </summary>
        public static AuthorizationConfig Instance
        {
            get
            {
                return instance;
            }
        }
        private static AuthorizationConfig instance =
                                        (AuthorizationConfig)WebConfigurationManager.GetWebApplicationSection(LomoAuthorization);

        /// <summary>
        ///     Gets the certificate map.
        /// </summary>
        [ConfigurationProperty(CertificateMapAttribute, IsRequired = false)]
        public KeyValueConfigurationCollection CertificateMap
        {
            get
            {
                return (KeyValueConfigurationCollection)this[CertificateMapAttribute];
            }
        }

        #endregion
    }
}