--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE MergeReferralType
	@id UNIQUEIDENTIFIER,
	@referrerId UNIQUEIDENTIFIER,
	@referrerTypeId INT,
	@referralVectorId INT,
	@rewardRecipientId INT,
	@eventRewards dbo.EventRewards READONLY
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'MergeReferralType'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_MergeReferralType_Lock', 'exclusive'

		-- Merge the ReferralType.
		MERGE dbo.ReferralTypes AS Existing
		USING
		(
			SELECT
				@id,
				@referrerId,
				@referrerTypeId,
				@referralVectorId,
				@rewardRecipientId
		)
		AS Additional
		(
			Id,
			ReferrerId,
			ReferrerTypeId,
			ReferralVectorId,
			RewardRecipientId
		)
		ON
			Existing.ReferrerId = Additional.ReferrerId
		AND
			Existing.ReferrerTypeId = Additional.ReferrerTypeId
		AND
			Existing.ReferralVectorId = Additional.ReferralVectorId
		AND
			Existing.RewardRecipientId = Additional.RewardRecipientId
		WHEN NOT MATCHED THEN
			INSERT
			(
				Id,
				ReferrerId,
				ReferrerTypeId,
				ReferralVectorId,
				RewardRecipientId
			)
			VALUES
			(
				Additional.Id,
				Additional.ReferrerId,
				Additional.ReferrerTypeId,
				Additional.ReferralVectorId,
				Additional.RewardRecipientId
			);

		DECLARE @finalId UNIQUEIDENTIFIER
		DECLARE @sequence INT
		DECLARE @code NVARCHAR(255)
		SELECT
			@finalId = ReferralTypes.Id,
			@sequence = ReferralTypes.Sequence,
			@code = ReferralTypes.Code
		FROM
			[dbo].[ReferralTypes] AS ReferralTypes
		WHERE
			ReferralTypes.ReferrerId = @referrerId
		AND
			ReferralTypes.ReferrerTypeId = @referrerTypeId
		AND
			ReferralTypes.ReferralVectorId = @referralVectorId
		AND
			ReferralTypes.RewardRecipientId = @rewardRecipientId

		DECLARE @newCode BIT
		SET @newCode = 0

		-- Add the code for new referral types.
		IF @code IS NULL
		BEGIN
			SET @newCode = 1

			-- Determine the code.
			DECLARE @digits VARCHAR(36)
			SET @digits = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'
			SET @code = ''
			WHILE (@sequence > 0)
			BEGIN
				SET @code = SUBSTRING(@digits, @sequence % 36 + 1, 1) + @code
				SET @sequence = @sequence / 36
			END
			SET @code = 'r_' + @code

			-- Add the code to the referral type.
			UPDATE
				dbo.ReferralTypes
			SET
				Code = @code
			WHERE
				Id = @finalId
		END

		-- Merge applicable ReferralEventRewards.
		MERGE dbo.ReferralEventRewards AS Existing
		USING
		(
			SELECT
				Id,
				@finalId,
				ReferralEventId,
				RewardId,
				PerUserLimit
			FROM
				@eventRewards
		)
		AS Additional
		(
			Id,
			ReferralTypeId,
			ReferralEventId,
			RewardId,
			PerUserLimit
		)
		ON
			Existing.ReferralTypeId = Additional.ReferralTypeId
		AND
			Existing.ReferralEventId = Additional.ReferralEventId
		WHEN MATCHED THEN
			UPDATE
			SET
				RewardId = Additional.RewardId,
				PerUserLimit = Additional.PerUserLimit
		WHEN NOT MATCHED THEN
			INSERT
			(
				Id,
				ReferralTypeId,
				ReferralEventId,
				RewardId,
				PerUserLimit
			)
			VALUES
			(
				Additional.Id,
				Additional.ReferralTypeId,
				Additional.ReferralEventId,
				Additional.RewardId,
				Additional.PerUserLimit
			);

		-- Delete applicable ReferralEventRewards
		DELETE FROM dbo.ReferralEventRewards
		FROM
			dbo.ReferralEventRewards AS ReferralEventRewards
		LEFT OUTER JOIN
			@eventRewards AS EventRewards
		ON
			ReferralEventRewards.ReferralEventId = EventRewards.ReferralEventId
        LEFT OUTER JOIN
            dbo.Referrals
        ON
            Referrals.ReferralEventRewardId = ReferralEventRewards.Id
        WHERE
	        ReferralEventRewards.ReferralTypeId = @finalId
        AND
            EventRewards.Id IS NULL
        AND
            Referrals.Id IS NULL


		-- Send the referral type code back to the caller.
		SELECT
			@code AS Code,
			@newCode AS NewCode
  
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