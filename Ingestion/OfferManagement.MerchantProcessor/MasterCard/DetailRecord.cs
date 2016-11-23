//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using OfferManagement.DataModel;
using Lomo.Logging;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace OfferManagement.MerchantFileParser.MasterCard
{
    public static class DetailRecord
    {
        internal const string ProvisioningRecordType = "20";
        internal const string ResponseFileRecordType = "25";
        const string ProvisioningActionCode = "A";
        const string MerchantMatchedActionCode = "M";
        const string MerchantNonMatchedActionCode = "N";
        const string ValidatedMatchActionCode = "V";
        const string MCFileDate = "MCFileDate";
        const string MCBeginDate = "MCBeginDate";
        const int MCIDLENGTH = 30;

        public static string Create(Merchant merchant, char delimiter, string provisioningDate)
        {
            string masterCardUniqueId = null;
            string siteId = null;
            string locationId = null;
            string beginDate = null;
            string fileDate = null;

            if (merchant.ExtendedAttributes != null)
            {
                masterCardUniqueId = merchant.ExtendedAttributes.ContainsKey(MerchantConstants.MCID)
                    ? merchant.ExtendedAttributes[MerchantConstants.MCID] : null;
                siteId = merchant.ExtendedAttributes.ContainsKey(MerchantConstants.MCSiteId)
                    ? merchant.ExtendedAttributes[MerchantConstants.MCSiteId] : null;
                beginDate = merchant.ExtendedAttributes.ContainsKey(MCBeginDate)
                    ? merchant.ExtendedAttributes[MCBeginDate] : null;

                if (merchant.ExtendedAttributes.ContainsKey(MCFileDate))
                {
                    fileDate = merchant.ExtendedAttributes[MCFileDate];
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(provisioningDate))
                    {
                        DateTime dt = DateTime.MinValue;
                        if (DateTime.TryParseExact(provisioningDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            fileDate = dt.ToString("M/d/yyyy");
                        }
                    }
                }

            }
            if (merchant.Payments != null)
            {
                foreach (var payment in merchant.Payments)
                {
                    if (payment.Processor == PaymentProcessor.MasterCard && payment.PaymentMids != null &&
                        payment.PaymentMids.ContainsKey(MerchantConstants.MCLocationId))
                    {
                        locationId = payment.PaymentMids[MerchantConstants.MCLocationId];
                    }
                }
            }

            return string.Join(delimiter.ToString(), ProvisioningRecordType, MerchantConstants.MasterCardProjectID, ProvisioningActionCode, siteId,
                null, locationId, null, merchant.Name, null, null, merchant.Location.Address, null,
                merchant.Location.City, null, merchant.Location.State, null, merchant.Location.Zip, null,
                "USA", null, merchant.PhoneNumber, null, null, null, null, null, null, null, null, beginDate, null, null, null,
                masterCardUniqueId, null, null, null, null, fileDate);
        }

        public static Merchant Parse(string record, char delimiter, MerchantFileType merchantFileType, int lineNumber)
        {
            Merchant merchant = null;
            if (string.IsNullOrWhiteSpace(record))
            {
                Log.Error($"Invalid {merchantFileType.ToString()} detail record. Empty record at line numnber {lineNumber}");
                return null;
            }

            string[] recordParts = record.Split(new char[] { delimiter });
            if (recordParts.Length != 39)
            {
                Log.Error($"Invalid {merchantFileType.ToString()} detail record. Number of fields did not equate to 39 at line numnber {lineNumber}");
                return null;
            }

            string actionCode = recordParts[2];
            if (merchantFileType == MerchantFileType.MasterCardProvisioning && actionCode != "A")
            {
                Log.Error($"Invalid {merchantFileType.ToString()} detail record at line number {lineNumber}.Detail record has an action code other than Add. This cannot be handled");
                return null;
            }

            string mcId = recordParts[33];
            string mcSiteId = recordParts[3];
            if (merchantFileType == MerchantFileType.MasterCardAuth || merchantFileType == MerchantFileType.MasterCardClearing)
            {
                if (actionCode != MerchantMatchedActionCode && actionCode != MerchantNonMatchedActionCode
                     && actionCode != ValidatedMatchActionCode)
                {
                    Log.Error($"Invalid {merchantFileType.ToString()} detail record at line number {lineNumber}. Detail record has an action code other than Match/NonMatch/ValidatedMatch. This cannot be handled");
                    return null;
                }
                if (actionCode == MerchantNonMatchedActionCode)
                {
                    Log.Warn($"No match found for MID in {merchantFileType.ToString()} at line number {lineNumber}");
                    return null;
                }
                if (!string.IsNullOrEmpty(mcId) && mcId.Length != MCIDLENGTH)
                {
                    Log.Error($"Invalid {merchantFileType.ToString()} detail record at line number {lineNumber}. Invalid length for MasterCard Microsoft Unique Identifier");
                    return null;
                }
                if (string.IsNullOrEmpty(mcId) && string.IsNullOrEmpty(mcSiteId))
                {
                    Log.Error($"Invalid {merchantFileType.ToString()} detail record at line number {lineNumber}. Both MCID and MCSiteId is missing");
                    return null;
                }
            }

            Payment payment = ParseForPayment(recordParts, merchantFileType, lineNumber);
            if (payment != null)
            {
                merchant = new Merchant
                {
                    IsActive = true,
                    PhoneNumber = recordParts[20],
                    Location = new Location
                    {
                        Address = !string.IsNullOrWhiteSpace(recordParts[10]) ? recordParts[10] : recordParts[11],
                        City = !string.IsNullOrWhiteSpace(recordParts[12]) ? recordParts[12] : recordParts[13],
                        State = !string.IsNullOrWhiteSpace(recordParts[14]) ? recordParts[14] : recordParts[15],
                        Zip = !string.IsNullOrWhiteSpace(recordParts[16]) ? recordParts[16] : recordParts[17]
                    },
                    Payments = new List<Payment> { payment },
                    ExtendedAttributes = new Dictionary<string, string>
                {
                    { MerchantConstants.MCSiteId, mcSiteId },
                    { MCBeginDate, recordParts[29] },
                    { MCFileDate, recordParts[38] }
                }
                };
                merchant.Name = ParseForMerchantName(recordParts, merchantFileType);
                if (!string.IsNullOrWhiteSpace(mcId))
                {
                    merchant.ExtendedAttributes.Add(MerchantConstants.MCID, mcId);
                }
            }

            return merchant;
        }

        private static Payment ParseForPayment(string[] recordParts, MerchantFileType merchantFileType, int lineNumber)
        {
            string locationId = recordParts[5];
            string acquirerMerchantId = recordParts[25];
            string acquirerIca = recordParts[26];
            if (merchantFileType == MerchantFileType.MasterCardProvisioning || merchantFileType == MerchantFileType.MasterCardClearing)
            {
                if (string.IsNullOrWhiteSpace(locationId))
                {
                    Log.Error($"LocationID is missing for {merchantFileType.ToString()} detail record at line number {lineNumber}");
                    return null;
                }
                return new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    Processor = PaymentProcessor.MasterCard,
                    IsActive = true,
                    SyncedWithCommerce = false,
                    PaymentMids = new Dictionary<string, string>
                        {
                            { MerchantConstants.MCLocationId, locationId }
                        }
                };
            }
            else if (merchantFileType == MerchantFileType.MasterCardAuth)
            {
                if (string.IsNullOrWhiteSpace(acquirerIca))
                {
                    Log.Error($"Auth AcquirerICA is missing for {merchantFileType.ToString()} detail record at line number {lineNumber}");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(acquirerMerchantId))
                {
                    Log.Error($"Auth Acquirer MerchantID is missing for {merchantFileType.ToString()} detail record at line number {lineNumber}");
                    return null;
                }
                return new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    Processor = PaymentProcessor.MasterCard,
                    IsActive = true,
                    SyncedWithCommerce = false,
                    PaymentMids = new Dictionary<string, string>
                        {
                            { MerchantConstants.MCAcquiringICA, acquirerIca },
                            { MerchantConstants.MCAcquiringMid, acquirerMerchantId }
                        }
                };
            }

            return null;

        }

        private static string ParseForMerchantName(string[] recordParts, MerchantFileType merchantFileType)
        {
            string merchantName = recordParts[7];
            string mcMerchantName = recordParts[8];

            if ((merchantFileType == MerchantFileType.MasterCardAuth || merchantFileType == MerchantFileType.MasterCardClearing)
                && (!string.IsNullOrWhiteSpace(mcMerchantName)))
            {
                return mcMerchantName;
            }
            else
            {
                return merchantName;
            }
        }
    }
}