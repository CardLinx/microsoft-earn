--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create view [dbo].[EarnRewardsAndGrants] with schemabinding as

 select UserId = Users.Id,
        GlobalUserId = Users.GlobalId,
        TransactionType = 0,
        TransactionId = RewardPayouts.Id,
        EarnCredit = RewardPayouts.Amount,
        BurnDebit = 0,
        DealSummary = Rewards.[Description],
        DealPercent = 0,
        MerchantName = RewardPayouts.Explanation,
        TransactionDate = RewardPayouts.PayoutFinalizedDateUtc,
        TransactionAmount = RewardPayouts.Amount,
        Reversed = 0,
        TransactionStatusId = 500, -- CreditGranted
        LastFourDigits = 'N/A',
        CardBrand = 0
 from dbo.RewardPayouts
   join dbo.Rewards on Rewards.Id = RewardPayouts.RewardId
   join dbo.Users on Users.GlobalId = RewardPayouts.PayeeId
 where Rewards.RewardTypeId = 3 and RewardPayouts.RewardPayoutStatusId = 2;

GO

create unique clustered index EarnRewardsAndGrants_Clustered
on dbo.EarnRewardsAndGrants
 (GlobalUserId, TransactionId);

GO