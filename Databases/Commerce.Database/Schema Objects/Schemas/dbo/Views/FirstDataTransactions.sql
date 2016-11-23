--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create view [dbo].[FirstDataTransactions] with schemabinding as

 select UserId = Cards.UserId,
        TransactionType = Offers.OfferType,
        TransactionId = RedeemedDeals.Id,
        EarnCredit = case when Offers.OfferType = 0 then RedeemedDeals.DiscountAmount else 0 end,
        BurnDebit = case when Offers.OfferType = 1 then RedeemedDeals.DiscountAmount else 0 end,
        DealSummary = case when Offers.OfferType = 0 then 'Earn ' + convert(varchar(3), cast(Offers.PercentBack as int)) + '%' else 'Save up to 100%' end,
        DealPercent = Offers.PercentBack,
        MerchantName = Merchants.Name,
        TransactionDate = RedeemedDeals.PurchaseDateTime,
        TransactionAmount = case when RedeemedDeals.SettlementAmount is not null and RedeemedDeals.SettlementAmount <> 0 then RedeemedDeals.SettlementAmount else RedeemedDeals.AuthorizationAmount end,
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
  join dbo.Offers on Offers.Id = TransactionLinks.Dealid
  join dbo.Merchants on Merchants.Id = RedeemedDeals.MerchantNameId
 where TransactionLinks.PartnerId = 1 and Cards.FDCToken is not null and Offers.OfferType in (0, 1)

GO

create unique clustered index FirstDataTransactions_Clustered
on dbo.FirstDataTransactions
 (UserId, TransactionId);

GO