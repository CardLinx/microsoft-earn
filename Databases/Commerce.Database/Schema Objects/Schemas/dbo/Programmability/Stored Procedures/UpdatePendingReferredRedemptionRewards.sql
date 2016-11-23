--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE UpdatePendingReferredRedemptionRewards
	@rewardPayoutStatusId INT,
	@rewardPayoutIds ListOfInts READONLY
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'UpdatePendingReferredRedemptionRewards'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_UpdatePendingReferredRedemptionRewards_Lock', 'exclusive'

  UPDATE dbo.RewardPayouts
  SET RewardPayoutStatusId = @rewardPayoutStatusId
     ,PayoutFinalizedDateUtc = (CASE WHEN @rewardPayoutStatusId <> 2 THEN RP.PayoutFinalizedDateUtc ELSE getutcdate() END)
  FROM dbo.RewardPayouts RP
  JOIN dbo.TrackedRedemptionRewards TRR ON TRR.RewardPayoutId = RP.Id
  JOIN @rewardPayoutIds RPIDS ON RPIDS.Id = TRR.Id
  
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