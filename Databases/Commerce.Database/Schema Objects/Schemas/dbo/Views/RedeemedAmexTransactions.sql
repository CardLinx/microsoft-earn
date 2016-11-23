--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create view [dbo].[RedeemedAmexTransactions] with schemabinding as

 select UserId = Cards.UserId,
        TransactionType = Offers.OfferType,
        TransactionId = RedeemedDeals.Id,
        PartnerTransactionId = case when PartnerRedeemedDeals.PartnerRedeemedDealScopeId is not null then PartnerRedeemedDeals.PartnerRedeemedDealScopeId
                                    when PartnerRedeemedDeals.PartnerRedeemedDealId is not null then PartnerRedeemedDeals.PartnerRedeemedDealId end,
        EarnCredit = case when Offers.OfferType = 0 then RedeemedDeals.DiscountAmount else 0 end,
        BurnDebit = case when Offers.OfferType = 1 then RedeemedDeals.DiscountAmount else 0 end,
        DealSummary = case when Offers.OfferType = 0 then 'Earn ' + convert(varchar(3), cast(Offers.PercentBack as int)) + '%' else 'Save up to 100%' end,
        DealPercent = Offers.PercentBack,
        MerchantName = Merchants.Name,
        TransactionDate = RedeemedDeals.PurchaseDateTime,
        TransactionAmount = RedeemedDeals.SettlementAmount,
        Reversed = RedeemedDeals.Reversed,
        TransactionStatusId = RedeemedDeals.CreditStatusId,
        LastFourDigits = Cards.LastFourDigits,
        CardBrand = Cards.CardBrand,
        PermaPending = RedeemedDeals.PermaPending,
        ReviewStatusId = RedeemedDeals.ReviewStatusId,
		CardId = Cards.Id
 from dbo.RedeemedDeals
  join dbo.PartnerRedeemedDeals on PartnerRedeemedDeals.RedeemedDealId = RedeemedDeals.Id
  join dbo.TransactionLinks on TransactionLinks.Id = RedeemedDeals.ClaimedDealId
  join dbo.Cards on Cards.Id = TransactionLinks.CardId
  join dbo.Offers on Offers.Id = TransactionLinks.Dealid
  join dbo.Merchants on Merchants.Id = RedeemedDeals.MerchantNameId
 where TransactionLinks.PartnerId = 2 and Cards.CardBrand = 3 and Offers.OfferType in (0, 1)
   and (PartnerRedeemedDeals.PartnerRedeemedDealScopeId is not null or PartnerRedeemedDeals.PartnerRedeemedDealId is not null);

GO

create unique clustered index RedeemedAmexTransactions_Clustered
on dbo.RedeemedAmexTransactions
 (UserId, TransactionType, PartnerTransactionId, TransactionId);

GO