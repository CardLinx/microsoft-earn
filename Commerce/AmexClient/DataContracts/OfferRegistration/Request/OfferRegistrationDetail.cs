//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents Amex Merchant Registration Detail Record
    /// </summary>
    public class OfferRegistrationDetail
    {
        /// <summary>
        /// Gets or sets Action Code A=Add, U=Update D=Delete
        /// </summary>
        public string ActionCode { get; set; }
        
        /// <summary>
        /// Gets or sets Offer Name
        /// </summary>
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets OfferStartDate
        /// </summary>
        public DateTime MerchantStartDate { get; set; }

        /// <summary>
        /// Gets or sets OfferEndDate
        /// </summary>
        public DateTime MerchantEndDate { get; set; }

        /// <summary>
        /// Gets or sets MerchantNumber
        /// </summary>
        public string MerchantNumber { get; set; }

        /// <summary>
        /// Gets or sets MerchantName
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets MerchantId
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Construct a line record for merchant registration file
        /// </summary>
        /// <returns>
        /// String representation of the detail record
        /// </returns>
        public string BuildFileDetailRecord()
        {
            StringBuilder result = new StringBuilder();
            result.Append(AmexConstants.DetailIdentifier); // constant detail identifier D
            result.Append(AmexConstants.Delimiter);
            result.Append(ActionCode); // Add | Update | Delete
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.PartnerId); // Partner Id
            result.Append(AmexConstants.Delimiter);
            result.Append(VersionNumber); // Non Offer based partners 1.0
            result.Append(AmexConstants.Delimiter);
            result.Append("                              "); // SellerId 30 chars filler
            result.Append(AmexConstants.Delimiter);
            result.Append("               "); // TPA SE 15 chars filler
            result.Append(AmexConstants.Delimiter);
            result.Append(PaddedMerchantNumber); // Merchant SE 15 chars
            result.Append(AmexConstants.Delimiter);
            result.Append(OfferId); // non offer based partner 000000000
            result.Append(AmexConstants.Delimiter);
            result.Append(OfferName); // offer name
            result.Append(AmexConstants.Delimiter);
            // offer start date
            result.Append(AmexConstants.Delimiter);
            // offer end date
            result.Append(AmexConstants.Delimiter);
            result.Append(AmexConstants.Currency); // USD
            result.Append(AmexConstants.Delimiter);
            result.Append(MerchantName); // Merchant Name
            result.Append(AmexConstants.Delimiter);
            // legal name
            result.Append(AmexConstants.Delimiter);
            result.Append(MerchantStartDate.ToString("MM/dd/yyyy")); // merchant reg start date 
            result.Append(AmexConstants.Delimiter);
            result.Append(MerchantEndDate.ToString("MM/dd/yyyy")); // merchant reg end date
            result.Append(AmexConstants.Delimiter);
            // --- BELOW FIELDS ARE OPTIONAL
            // --- BUT DELIMITER IS NEEDED
            // merchant address line 1
            result.Append(AmexConstants.Delimiter);
            // merchant address line 2
            result.Append(AmexConstants.Delimiter);
            // merchant address line 3
            result.Append(AmexConstants.Delimiter);
            // merchant address line 4
            result.Append(AmexConstants.Delimiter);
            // merchant address line 5
            result.Append(AmexConstants.Delimiter);
            // merchant city
            result.Append(AmexConstants.Delimiter);
            // merchant state
            result.Append(AmexConstants.Delimiter);
            // merchant postal code
            result.Append(AmexConstants.Delimiter);
            // merchant country
            result.Append(AmexConstants.Delimiter);
            // latitude
            result.Append(AmexConstants.Delimiter);
            // longitude
            result.Append(AmexConstants.Delimiter);
            result.Append(CustomField1); // custom field 1
            result.Append(AmexConstants.Delimiter);
            result.Append(CustomField2); // custom field 2
            result.Append(AmexConstants.Delimiter);
            return result.ToString();
        }

        private string PaddedMerchantNumber { get { return MerchantNumber + "     "; } }


        private string CustomField1
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MerchantId))
                {
                    var merchantId = MerchantId.Trim();
                    return merchantId.Length <= 20 ? merchantId : merchantId.Substring(0, 20);
                }

                return string.Empty;
            }
        }

        private string CustomField2
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MerchantId))
                {
                    var merchantId = MerchantId.Trim();
                    return merchantId.Length > 20 ? merchantId.Substring(20, merchantId.Length - 20) : string.Empty;
                }

                return string.Empty;
            }
        }
        
        /// <summary>
        /// Version Number
        /// </summary>
        private string VersionNumber = "1.0";

        /// <summary>
        /// Offer Id
        /// </summary>
        private string OfferId = "000000000";
        
    }
}