--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE AddReferral
	@userId NVARCHAR(100),
	@referralTypeCode NVARCHAR(255),
	@referralEventId INT,
    @earnAmount INT,
    @earnExplanation NVARCHAR(100)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'AddReferral'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_AddReferral_Lock', 'exclusive'

		-- Find the applicable ReferralEventRewards record.
		DECLARE @referrerId UNIQUEIDENTIFIER
		DECLARE @referrerTypeId INT
		DECLARE @referralEventRewardId UNIQUEIDENTIFIER
		DECLARE @rewardId UNIQUEIDENTIFIER
		DECLARE @perUserLimit INT
		SELECT
			@referrerId = ReferralTypes.ReferrerId,
			@referrerTypeId = ReferralTypes.ReferrerTypeId,
			@referralEventRewardId = ReferralEventRewards.Id,
			@rewardId = ReferralEventRewards.RewardId,
			@perUserLimit = ReferralEventRewards.PerUserLimit
		FROM
			[dbo].[ReferralEventRewards] AS ReferralEventRewards
		INNER JOIN
			[dbo].[ReferralTypes] AS ReferralTypes
		ON
			ReferralTypes.Id = ReferralEventRewards.ReferralTypeId
		WHERE
			ReferralTypes.Code = @referralTypeCode
		AND
			ReferralEventRewards.ReferralEventId = @referralEventId

		-- If no applicable ReferralEventRewards record could be found, throw an exception.
		IF @referralEventRewardId IS NULL
		BEGIN
			DECLARE @invalidParameterMessage CHAR(16)
			SET @invalidParameterMessage = 'InvalidParameter'
			RAISERROR (@invalidParameterMessage, 16, 1)
		END

        -- Determine if the referral is still valid for the referrer.
        DECLARE @timesReferrerReferred INT
        SELECT
            @timesReferrerReferred = COUNT(*)
        FROM
            [dbo].[ReferralTypes] AS ReferralTypes
        INNER JOIN
            [dbo].[RewardPayouts] AS RewardPayouts
        ON
            RewardPayouts.PayeeId = ReferralTypes.ReferrerId
        INNER JOIN
            [dbo].[Referrals] AS Referrals
        ON
            Referrals.RewardPayoutId = RewardPayouts.Id
        WHERE
            ReferrerId = @referrerId
        AND
            Referrals.ReferralEventRewardId = @referralEventRewardId
        AND
            RewardPayouts.RewardPayoutStatusId = 2 -- Paid. Let the referrer continue referring until the number of payouts hits the limit.

		-- Determine if the referral is still valid for this user.
		DECLARE @timesUserReferred INT
		SELECT
			@timesUserReferred = COUNT(*)
		FROM
			[dbo].[Referrals] AS Referrals
		WHERE
			Referrals.ReferredUserId = @userId
		AND
			Referrals.ReferralEventRewardId = @referralEventRewardId

		-- If the referral is still valid for this referrer and user, record the referral event.
		IF @timesUserReferred = 0 and (@perUserLimit = 0 OR @timesReferrerReferred < @perUserLimit)
		BEGIN
			-- Insert reward payout record.
--TODO: Need to change behavior based on reward recipient. For now, only referrer is the recipient.
--TODO: Not sure if referrer type and payee type will always line up.
			DECLARE @rewardPayoutId UNIQUEIDENTIFIER
			SET @rewardPayoutId = NEWID()
			INSERT INTO [dbo].[RewardPayouts]
			(
				Id,
				RewardId,
				RewardReasonId,
				PayeeId,
				PayeeTypeId,
				RewardPayoutStatusId,
				AgentId,
				AgentTypeId,
                Amount,
                Explanation
			)
			VALUES
			(
				@rewardPayoutId,
				@rewardId,
				0, -- Referral
				@referrerId,
				@referrerTypeId,
				0, -- Unprocessed
				@userId,
				0, -- User
                @earnAmount,
                @earnExplanation
			)

			-- Insert referral record.
			INSERT INTO [dbo].[Referrals]
			(
				Id,
				ReferralEventRewardId,
				ReferredUserId,
				RewardPayoutId
			)
			VALUES
			(
				NEWID(),
				@referralEventRewardId,
				@userId,
				@rewardPayoutId
			)
		END

		-- Send back the affected row count.
		SELECT @@ROWCOUNT AS ReferralsAdded
  
  COMMIT TRANSACTION

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO