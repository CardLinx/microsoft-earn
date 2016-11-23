--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE AddReferredRedemptionReward
  @redeemedDealId uniqueidentifier,
  @referredUserId uniqueidentifier
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'AddReferredRedemptionReward'
       ,@Mode varchar(100) = ' RD='+convert(char(36), @redeemedDealId)+' RU='+convert(char(36), @referredUserId)

BEGIN TRY

  -- Given a referred user ID, find the person whose referral led to signing up.
  DECLARE @referrerId uniqueidentifier
  SELECT @referrerId = RT.ReferrerId
  FROM dbo.Referrals R
       JOIN dbo.ReferralEventRewards RER ON RER.Id = R.ReferralEventRewardId
       JOIN dbo.ReferralTypes RT ON RT.Id = RER.ReferralTypeId
  WHERE RER.ReferralEventId = 1
    AND R.referredUserId = @referredUserId
    AND RT.ReferrerTypeId = 0

  -- If the user had been referred into the system, reward the referrer when applicable.
  IF @referrerId IS NOT NULL
  BEGIN
    -- Find the applicable ReferralEventRewards record.
    DECLARE @referralEventRewardId uniqueidentifier
    DECLARE @rewardId uniqueidentifier
    DECLARE @perUserLimit int
    SELECT @referralEventRewardId = RER.Id
          ,@rewardId = RER.RewardId
          ,@perUserLimit = RER.PerUserLimit
    FROM dbo.ReferralEventRewards RER
         JOIN dbo.ReferralTypes RT ON RT.Id = RER.ReferralTypeId
    WHERE RT.ReferrerId = @referrerId
      AND RT.ReferrerTypeId = 0
      AND RER.ReferralEventId = 2

    -- If the referrer has not reached the maximum number of payouts and has not already received a payout for redemptions from this user, schedule a payout.
    DECLARE @Lock varchar(100) = db_name()+'_'+@SP+'_'+convert(char(36),@referrerId)
    BEGIN TRANSACTION
    EXECUTE sp_getapplock @Lock, 'exclusive'
    IF (SELECT COUNT(*) FROM RewardPayouts WHERE RewardId = @rewardId AND PayeeId = @referrerId AND PayeeTypeId = 0) < @perUserLimit AND
	   (SELECT COUNT(*) FROM RewardPayouts WHERE
			RewardId = @rewardId AND PayeeId = @referrerId AND PayeeTypeId = 0 AND AgentId = @referredUserId AND AgentTypeId = 0) = 0
    BEGIN
      -- Add the RewardPayouts record.
      DECLARE @rewardPayoutId uniqueidentifier
      SET @rewardPayoutId = newid()
      INSERT INTO dbo.RewardPayouts
               ( Id,			  RewardId,  RewardReasonId, PayeeId,     PayeeTypeId, RewardPayoutStatusId, AgentId,			AgentTypeId )
        VALUES ( @rewardPayoutId, @rewardId, 0,              @referrerId, 0,           0,					 @referredUserId,   0)
               --                            Referrer                     User         Unprocessed								User

      -- Add the TrackedRedemptionRewards record.
      INSERT INTO dbo.TrackedRedemptionRewards
               ( RedeemedDealId,  RewardPayoutId )
        VALUES ( @redeemedDealId, @rewardPayoutId )
    END
    COMMIT TRANSACTION
  END

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO