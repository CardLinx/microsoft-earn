--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create view [dbo].[AuthorizedMasterCardTransactions] with schemabinding as

 select UserId = Cards.UserId,
        TransactionType = Offers.OfferType,
        BankCustomerNumber = Cards.PartnerToken,
        BankNetRefNumber = Authorizations.TransactionScopeId,
        TransactionDate = Authorizations.TransactionDate,
        TransactionId = Authorizations.Id,
        EarnCredit = case when Offers.OfferType = 0 then Authorizations.DiscountAmount else 0 end,
        BurnDebit = case when Offers.OfferType = 1 then Authorizations.DiscountAmount else 0 end,
        DealSummary = case when Offers.OfferType = 0 then 'Earn ' + convert(varchar(3), cast(Offers.PercentBack as int)) + '%' else 'Save up to 100%' end,
        DealPercent = Offers.PercentBack,
        MerchantName = Merchants.Name,
        TransactionAmount = Authorizations.TransactionAmount,
        Reversed = 0, -- deprecated. Will be removed later.
        TransactionStatusId = 0, -- AuthorizationReceived.
        LastFourDigits = Cards.LastFourDigits,
        CardBrand = Cards.CardBrand,
        PermaPending = Authorizations.PermaPending,
        ReviewStatusId = Authorizations.ReviewStatusId
 from dbo.Authorizations
  join dbo.TransactionLinks on TransactionLinks.Id = Authorizations.ClaimedDealId
  join dbo.Cards on Cards.Id = TransactionLinks.CardId
  join dbo.Offers on Offers.Id = TransactionLinks.Dealid
  join dbo.Merchants on Merchants.Id = Authorizations.MerchantNameId
 where Authorizations.PartnerId = 4 and TransactionLinks.PartnerId = 4 and Cards.CardBrand = 5 and Offers.OfferType in (0, 1)
   and Authorizations.TransactionScopeId is not null;

GO

create unique clustered index AuthorizedMasterCardTransactions_Clustered
on dbo.AuthorizedMasterCardTransactions
 (UserId, TransactionType, BankCustomerNumber, BankNetRefNumber, TransactionDate, TransactionId);

GO