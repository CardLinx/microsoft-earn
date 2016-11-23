--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetReferralCounts
	@referrerId UNIQUEIDENTIFIER,
	@referrerTypeId INT
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetReferralCounts'
	   
    select ReferralTypes.Code, ReferralEventRewards.ReferralEventId, RewardPayouts.RewardPayoutStatusId, count(*) as Count
     from dbo.Referrals
      join dbo.RewardPayouts on RewardPayouts.Id = Referrals.RewardPayoutId
      join dbo.Rewards on Rewards.Id = RewardPayouts.RewardId
      join dbo.ReferralEventRewards on ReferralEventRewards.Id = Referrals.ReferralEventRewardId
      join dbo.ReferralTypes on ReferralTypes.Id = ReferralEventRewards.ReferralTypeId
     where ReferralTypes.ReferrerId = @referrerId and referralTypes.ReferrerTypeId = @referrerTypeId and Rewards.Active = 1
     group by Code, ReferralEventId, RewardPayoutStatusId

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO