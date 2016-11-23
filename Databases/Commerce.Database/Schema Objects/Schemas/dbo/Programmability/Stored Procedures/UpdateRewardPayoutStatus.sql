--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE UpdateRewardPayoutStatus
	@rewardPayoutId UNIQUEIDENTIFIER,
	@payoutStatusId INT
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'UpdateRewardPayoutStatus'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_UpdateRewardPayoutStatus_Lock', 'exclusive'

		-- If the current payout status is more advanced than the expected new payout status, throw an exception.
		IF (SELECT RewardPayoutStatusId FROM [dbo].[RewardPayouts] WHERE Id = @rewardPayoutId) >= @payoutStatusId
		BEGIN
			DECLARE @payoutStatusTooAdvancedMessage CHAR(23)
			SET @payoutStatusTooAdvancedMessage = 'PayoutStatusTooAdvanced'
			RAISERROR (@payoutStatusTooAdvancedMessage, 16, 1)
		END

		-- Store the new reward payout status.
		UPDATE
			[dbo].[RewardPayouts]
		SET
			RewardPayoutStatusId = @payoutStatusId,
			PayoutFinalizedDateUtc =
			(CASE
				 WHEN
					 @payoutStatusId > 1 -- Don't set a finalized payout date for pending transactions.
				 THEN
					 GETUTCDATE()
				 ELSE
					 NULL
			 END)
		WHERE
			Id = @rewardPayoutId
 
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