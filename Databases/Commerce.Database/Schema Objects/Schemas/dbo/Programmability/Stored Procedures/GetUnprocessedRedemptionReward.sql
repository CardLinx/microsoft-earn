--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetUnprocessedRedemptionReward
  @rewardPayoutId uniqueidentifier,
  @partnerCardId varchar(255),
  @partnerRedeemedDealId nvarchar(255),
  @rewardId uniqueidentifier = NULL -- Remove NULL after F&F deployment.
AS
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetUnprocessedRedemptionReward'

-- First, determine if the transaction was reversed.
DECLARE @rescinded bit
SELECT @rescinded = RD.Reversed
  FROM dbo.RedeemedDeals RD
       JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
  WHERE PRD.PartnerRedeemedDealId = @partnerRedeemedDealId

-- Then, find the user eligible to receive the reward, if any.
DECLARE @payeeId UNIQUEIDENTIFIER = NULL;
WITH CandidateUserIds (UserId, Active, UtcAdded)
AS
(
  SELECT UserId = (SELECT GlobalId FROM dbo.Users U WHERE U.Id = C.UserId),
    C.Active,
    C.UtcAdded
  FROM dbo.Cards C
  WHERE  (C.PartnerToken = @partnerCardId OR C.FDCToken = @partnerCardId)
    AND UtcAdded IS NOT NULL
)
SELECT @payeeId =
(
  SELECT TOP 1 C.UserId
  FROM CandidateUserIds C
  WHERE NOT EXISTS(SELECT * FROM dbo.RewardPayouts RP WHERE RP.RewardId = @rewardId AND RP.PayeeTypeId = 0 AND RP.PayeeId = C.UserId)
  ORDER BY
    C.Active DESC,
    C.UtcAdded ASC
)
IF @payeeId IS NULL SET @payeeId = '00000000-0000-0000-0000-000000000000'

-- Update the database with the payee ID if an eligible user was found.
IF @payeeId <> '00000000-0000-0000-0000-000000000000'
BEGIN
    UPDATE dbo.RewardPayouts
    SET PayeeId = @payeeId
    WHERE Id = @rewardPayoutId
END

-- Finally, return information about the reward payout record.
SELECT R.RewardTypeId
      ,R.Properties
      ,UserId = @payeeId
      ,Rescinded = @rescinded 
  FROM dbo.Rewards R
       JOIN dbo.RewardPayouts RP ON RP.RewardId = R.Id
  WHERE RP.Id = @rewardPayoutId
    AND RP.RewardId = @rewardId
    AND RP.RewardPayoutStatusId = 0 -- Unprocessed

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO