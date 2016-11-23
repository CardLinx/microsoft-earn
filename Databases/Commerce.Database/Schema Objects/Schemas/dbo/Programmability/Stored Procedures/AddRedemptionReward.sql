--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE AddRedemptionReward
    @rewardId UNIQUEIDENTIFIER,
    @amount INT = 0,
    @explanation NVARCHAR(100) = NULL
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'AddRedemptionReward'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_AddRedemptionReward_Lock', 'exclusive'

		-- Add reward payout record.
		DECLARE @rewardPayoutId UNIQUEIDENTIFIER
		SET @rewardPayoutId = NEWID()
		INSERT INTO
			[dbo].[RewardPayouts]
		(
			Id,
			RewardId,
			RewardReasonId,
			PayeeId,
			PayeeTypeId,
			RewardPayoutStatusId,
			Amount,
			Explanation
		)
		VALUES
		(
			@rewardPayoutId,
			@rewardId,
			1, --Redemption
			'00000000-0000-0000-0000-000000000000',
			0, -- User
			0, -- Unprocessed
			@amount,
			@explanation
		)

		-- Send the reward payout ID back to the caller.
		SELECT @rewardPayoutId AS RewardPayoutId

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