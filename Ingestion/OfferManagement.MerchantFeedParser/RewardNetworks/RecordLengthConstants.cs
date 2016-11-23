//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace OfferManagement.MerchantFeedParser.RewardNetworks
{
    public class RecordLengthConstants
    {
        /// <summary>
        /// Length of the reward network header record
        /// </summary>
        internal const int HeaderRecordLength = 300;

        /// <summary>
        /// Length of the reward network detail record
        /// </summary>
        internal const int DetailRecordLength = 300;

        /// <summary>
        /// Length of the reward network trailer record
        /// </summary>
        internal const int TrailerRecordLength = 300;

        /// <summary>
        /// The length of the record type field.
        /// </summary>
        internal const int RecordTypeLength = 1;

        /// <summary>
        /// Length of the reward network file description
        /// </summary>
        internal const int FileDescriptionLength = 15;

        /// <summary>
        /// Length of the reward network file sequence number
        /// </summary>
        internal const int FileSequenceNumberLength = 5;

        /// <summary>
        /// Length of the file creation date field
        /// </summary>
        internal const int FileCreationLength = 8;

        /// <summary>
        /// Length of the merchant id field
        /// </summary>
        internal const int MerchantIdLength = 9;

        /// <summary>
        /// Length of the merchant name field
        /// </summary>
        internal const int MerchantNameLength = 68;

        /// <summary>
        /// Length of the merchant address field
        /// </summary>
        internal const int MerchantAddressLength = 30;

        /// <summary>
        /// Length of the merchant city field
        /// </summary>
        internal const int MerchantCityLength = 20;

        /// <summary>
        /// Length of the merchant state field
        /// </summary>
        internal const int MerchantStateLength = 2;

        /// <summary>
        /// Length of the merchant zip field
        /// </summary>
        internal const int MerchantZipLength = 5;

        /// <summary>
        /// Length of the merchant url field
        /// </summary>
        internal const int MerchantUrlLength = 75;

        /// <summary>
        /// The length of the record count field.
        /// </summary>
        internal const int RecordCountFieldLength = 7;      
    }
}