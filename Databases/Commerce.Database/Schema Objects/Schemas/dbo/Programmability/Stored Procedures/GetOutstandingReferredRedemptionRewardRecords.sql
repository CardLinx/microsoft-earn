--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetOutstandingReferredRedemptionRewardRecords
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetOutstandingReferredRedemptionRewardRecords'

SELECT TRR.Id AS TrackedRedemptionRewardsId
      ,TRR.RewardPayoutId
      ,TRR.RedeemedDealId
	  ,C.FDCToken AS PartnerCardId
	  ,R.Properties
	  ,RP.PayoutScheduledDateUtc
FROM dbo.TrackedRedemptionRewards TRR
JOIN dbo.RewardPayouts RP ON RP.Id = TRR.RewardPayoutId
JOIN dbo.Rewards R ON R.Id = RP.RewardId
JOIN dbo.RedeemedDeals RD ON RD.Id = TRR.RedeemedDealId
JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = TRR.RedeemedDealId
JOIN dbo.Users U ON U.GlobalId = RP.PayeeId
JOIN dbo.Cards C ON C.UserId = U.Id
WHERE C.Active = 1
  AND C.FDCToken is not null
  AND RP.RewardPayoutStatusId = 0 -- Unprocessed
ORDER BY C.UtcAdded DESC

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO