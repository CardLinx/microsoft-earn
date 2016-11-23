//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataContracts
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents Amex Auth Request.
    /// </summary>
    [XmlRoot(Namespace = "", ElementName = "AuthorizationData", DataType = "string", IsNullable = true)]
    public class AmexAuthRequest
    {
        /// <summary>
        /// Gets or sets the time of the transaction; format : YYYY-MM-DD HH:MM:SS.
        /// </summary>
        [XmlElement(ElementName = "TransactionTime")]
        public string TransactionTime { get; set; }

        /// <summary>
        /// Gets or sets transaction id.
        /// </summary>
        [XmlElement(ElementName = "TransactionID")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets Offer id.
        /// </summary>
        [XmlElement(ElementName = "OfferId")]
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets Card Token
        /// </summary>
        [XmlElement(ElementName = "CMAlias")]
        public string CardToken { get; set; }

        /// <summary>
        /// Gets or sets Transaction Amount
        /// </summary>
        [XmlElement(ElementName = "TransactionAmount")]
        public string TransactionAmount { get; set; }

        /// <summary>
        /// Gets or sets Merchant Number
        /// </summary>
        [XmlElement(ElementName = "MerchantNumber")]
        public string MerchantNumber { get; set; }
    }
}