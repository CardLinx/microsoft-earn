//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataWorker
{
    /// <summary>
    /// Contains constants used throughout the extract parsing classes.
    /// </summary>
    internal static class ExtractConstants
    {
        /// <summary>
        /// The length of the acquirer reference number field.
        /// </summary>
        internal const int AcquirerReferenceNumberLength = 23;

        /// <summary>
        /// The length of the amount sign fields.
        /// </summary>
        internal const int AmountSignLength = 1;

        /// <summary>
        /// The length of the bank marker field.
        /// </summary>
        internal const int BankMarkerLength = 3;

        /// <summary>
        /// The length of the card suffix field.
        /// </summary>
        internal const int CardSuffixLength = 2;

        /// <summary>
        /// The length of the chain ID field.
        /// </summary>
        internal const int ChainIdLength = 20;

        /// <summary>
        /// The length of the consumer ID field.
        /// </summary>
        internal const int ConsumerIdLength = 50;

        /// <summary>
        /// The length of the corporate ID field.
        /// </summary>
        internal const int CorporateIdLength = 20;

        /// <summary>
        /// The length of the currency code field.
        /// </summary>
        internal const int CurrencyCodeLength = 3;

        /// <summary>
        /// The length of the discount amount field.
        /// </summary>
        internal const int DiscountAmountLength = 21;

        /// <summary>
        /// The length of the discount percentage field.
        /// </summary>
        internal const int DiscountPercentageLength = 8;

        /// <summary>
        /// The length of the file creation date field.
        /// </summary>
        internal const int FileCreationDateLength = 8;

        /// <summary>
        /// The length of the invoice field.
        /// </summary>
        internal const int InvoiceIdLength = 20;

        /// <summary>
        /// The length of the location MID field.
        /// </summary>
        internal const int LocationMidLength = 20;

        /// <summary>
        /// The length of the minimum transaction amount field.
        /// </summary>
        internal const int MinimumTransactionAmountLength = 15;

        /// <summary>
        /// The length of the number of header records field.
        /// </summary>
        internal const int NumberOfFooterRecordsLength = 12;

        /// <summary>
        /// The length of the number of header records field.
        /// </summary>
        internal const int NumberOfHeaderRecordsLength = 12;

        /// <summary>
        /// The length of the number of redemption records field.
        /// </summary>
        internal const int NumberOfRedemptionRecordsLength = 12;

        /// <summary>
        /// The length of the number of settlement records field.
        /// </summary>
        internal const int NumberOfSettlementRecordsLength = 12;

        /// <summary>
        /// The length of the number of transaction notification records field.
        /// </summary>
        internal const int NumberOfTransactionNotificationRecordsLength = 12;

        /// <summary>
        /// The length of the offer acceptance ID field.
        /// </summary>
        internal const int OfferAcceptanceIdLength = 30;

        /// <summary>
        /// The length of the offer ID field.
        /// </summary>
        internal const int OfferIdLength = 18;

        /// <summary>
        /// The length of the offer type field.
        /// </summary>
        internal const int OfferTypeLength = 50;

        /// <summary>
        /// The length of the provider ID field.
        /// </summary>
        internal const int ProviderIdLength = 20;

        /// <summary>
        /// The level number for the provider to which the extract file belongs.
        /// </summary>
        internal const string ProviderLevelNumber = "00000";

        /// <summary>
        /// The length of the provider level number field.
        /// </summary>
        internal const int ProviderLevelNumberLength = 5;

        /// <summary>
        /// The length of the provider name field.
        /// </summary>
        internal const int ProviderNameLength = 65;

        /// <summary>
        /// The length of the publisher ID field.
        /// </summary>
        internal const int PublisherIdLength = 4;

        /// <summary>
        /// The length of the publisher name field.
        /// </summary>
        internal const int PublisherNameLength = 20;

        /// <summary>
        /// The length of the purchase price field.
        /// </summary>
        internal const int PurchasePriceLength = 15;

        /// <summary>
        /// The length of the record type field.
        /// </summary>
        internal const int RecordTypeLength = 3;

        /// <summary>
        /// The length of the redemption discount amount field.
        /// </summary>
        internal const int RedemptionDiscountAmountLength = 21;

        /// <summary>
        /// The length of the redemption MID field.
        /// </summary>
        internal const int RedemptionMidLength = 20;

        /// <summary>
        /// The length of the registration MID field.
        /// </summary>
        internal const int RegistrationMidLength = 20;

        /// <summary>
        /// The length of the terminal ID field.
        /// </summary>
        internal const int TerminalIdLength = 15;

        /// <summary>
        /// The length of the time field.
        /// </summary>
        internal const int TimeFieldLength = 10;

        /// <summary>
        /// The length of an amount field.
        /// </summary>
        internal const int AmountLength = 19;

        /// <summary>
        /// The length of the total transaction amount field.
        /// </summary>
        internal const int TotalTransactionAmountLength = 15;

        /// The length of the transaction amount field.
        /// </summary>
        internal const int TransactionAmountLength = 15;

        /// <summary>
        /// The length of the transaction ID field.
        /// </summary>
        internal const int TransactionIdLength = 30;

        /// <summary>
        /// The length of the transaction notification type field.
        /// </summary>
        internal const int TransactionNotificationTypeLength = 50;

        /// <summary>
        /// The length of the transaction type field.
        /// </summary>
        internal const int TransactionTypeLength = 50;
    }
}