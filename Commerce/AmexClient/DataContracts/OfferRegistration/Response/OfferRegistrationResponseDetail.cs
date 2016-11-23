//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.AmexClient
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Represents Amex Merchant Registration Detail Response Record
    /// </summary>
    public class OfferRegistrationResponseDetail
    {
        /// <summary>
        /// Create new instance of Response Detail record from response file line item
        /// </summary>
        /// <param name="record">
        /// String record
        /// </param>
        public OfferRegistrationResponseDetail(string record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "record cannot be null");
            }

            char[] charSeparators = { '|' };
            string[] tokens = record.Split(charSeparators, StringSplitOptions.None);

            if (tokens.Length < 29)
            {
                throw new InvalidDataException("Response Detail Record is not in expected format");
            }

            // token[0] is constant detail identifier D
            ActionCode = tokens[1];
            PartnerId = tokens[2];
            // token[3] is constant VersionNumber (1.0 non offer based partners)
            // token[4] is SellerId (Opt Blue)
            // token[5] is TPA SE (Opt Blue)
            MerchantNumber = tokens[6];
            OfferId = tokens[7];
            OfferName = tokens[8];
            // token[9] is Offer Start Date (empty as non offer based)
            // token[10] is Offer End Date (empty as non offer based)
            // tokens[11] is Source System Id
            MerchantName = tokens[12];
            // tokens[13] is Merchant Legal Name           
            // start date can be empty in case of update
            if (!string.IsNullOrEmpty(tokens[14]))
            {
                OfferStartDate = DateTime.ParseExact(tokens[14], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            OfferEndDate = DateTime.ParseExact(tokens[15], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            // tokens[16] is Merchant Address Line 1      
            // tokens[17] is Merchant Address Line 2  
            // tokens[18] is Merchant Address Line 3      
            // tokens[19] is Merchant Address Line 4     
            // tokens[20] is Merchant Address Line 5     
            // tokens[21] is Merchant City  
            // tokens[22] is Merchant State    
            // tokens[23] is Merchant Postal Code 
            // tokens[24] is Merchant Country    
            // tokens[25] is Merchant Latitude 
            // tokens[26] is Merchant Longitude 
            ResponseCode = tokens[27];
            ResponseCodeMessage = tokens[28];
            CustomField1 = tokens[29];
            CustomField2 = tokens[30];
            // tokens[31] is Filler
        }

        /// <summary>
        /// Gets or sets Action Code A=Add, U=Update D=Delete
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// Gets or sets Partner Id 
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets OfferID
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets Offer Name
        /// </summary>
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets OfferStartDate
        /// </summary>
        public DateTime OfferStartDate { get; set; }

        /// <summary>
        /// Gets or sets OfferEndDate
        /// </summary>
        public DateTime OfferEndDate { get; set; }

        /// <summary>
        /// Gets or sets MerchantNumber
        /// </summary>
        public string MerchantNumber { get; set; }

        /// <summary>
        /// Gets or sets MerchantName
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets Action Code A=Accepted, R=Rejected
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets ResponseCode Message
        /// </summary>
        public string ResponseCodeMessage { get; set; }

        /// <summary>
        /// Gets or sets CustomField1
        /// </summary>
        public string CustomField1 { get; set; }

        /// <summary>
        /// Gets or sets CustomField2
        /// </summary>
        public string CustomField2 { get; set; }

        /// <summary>
        /// Gets MerchantId
        /// </summary>
        public string MerchantId { get { return string.Format("{0}{1}", CustomField1, CustomField2).Trim(); } }
    }
}