--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create view [dbo].[RedeemedMasterCardTransactions] with schemabinding as

 select UserId = Cards.UserId,
        TransactionType = Offers.OfferType,
        BankCustomerNumber = Cards.PartnerToken,
        BankNetRefNumber = PartnerRedeemedDeals.PartnerRedeemedDealScopeId,
        TransactionDate = RedeemedDeals.PurchaseDateTime,
        TransactionId = RedeemedDeals.Id,
        EarnCredit = case when Offers.OfferType = 0 then RedeemedDeals.DiscountAmount else 0 end,
        BurnDebit = case when Offers.OfferType = 1 then RedeemedDeals.DiscountAmount else 0 end,
        DealSummary = case when Offers.OfferType = 0 then 'Earn ' + convert(varchar(3), cast(Offers.PercentBack as int)) + '%' else 'Save up to 100%' end,
        DealPercent = Offers.PercentBack,
        MerchantName = Merchants.Name,
        TransactionAmount = RedeemedDeals.SettlementAmount,
        Reversed = RedeemedDeals.Reversed,
        TransactionStatusId = RedeemedDeals.CreditStatusId,
        LastFourDigits = Cards.LastFourDigits,
        CardBrand = Cards.CardBrand,
        PermaPending = RedeemedDeals.PermaPending,
        ReviewStatusId = RedeemedDeals.ReviewStatusId
 from dbo.RedeemedDeals
  join dbo.PartnerRedeemedDeals on PartnerRedeemedDeals.RedeemedDealId = RedeemedDeals.Id
  join dbo.TransactionLinks on TransactionLinks.Id = RedeemedDeals.ClaimedDealId
  join dbo.Cards on Cards.Id = TransactionLinks.CardId
  join dbo.Offers on Offers.Id = TransactionLinks.DealId
  join dbo.Merchants on Merchants.Id = RedeemedDeals.MerchantNameId
 where TransactionLinks.PartnerId = 4 and Cards.CardBrand = 5 and Offers.OfferType in (0, 1)
   and PartnerRedeemedDeals.PartnerRedeemedDealScopeId is not null;

GO

create unique clustered index RedeemedMasterCardTransactions_Clustered
on dbo.RedeemedMasterCardTransactions
(UserId, TransactionType, BankCustomerNumber, BankNetRefNumber, TransactionDate, TransactionId);

GO