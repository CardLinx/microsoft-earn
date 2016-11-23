--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE [dbo].[CreateOrUpdateMerchantSubscriptions]
  @UserId uniqueidentifier, @PartitionId int, @SubscriptionType nvarchar(50), @ScheduleType nvarchar(50), @Preferences nvarchar(max) = null
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'MerchantSubscriptions_'+convert(varchar(50), @UserId)
DECLARE @NextRunTime datetime

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on id

MERGE MerchantSubscriptions T
			USING
			(SELECT UserId = @UserId, PartitionId = @PartitionId, SubscriptionType = @SubscriptionType, ScheduleType = @ScheduleType, Preferences = @Preferences) S
				ON T.UserId = S.UserId AND T.PartitionId = S.PartitionId AND T.SubscriptionType = S.SubscriptionType
			WHEN NOT MATCHED BY TARGET THEN 
					INSERT   ( UserId, PartitionId, SubscriptionType, IsActive, ScheduleType, Preferences) 
					VALUES   ( S.UserId, S.PartitionId, S.SubscriptionType, 1, S.ScheduleType, S.Preferences)
			WHEN MATCHED AND
				(NOT S.ScheduleType = T.ScheduleType) OR
				(S.Preferences IS NOT NULL AND (T.Preferences IS NULL OR NOT S.Preferences = T.Preferences)) THEN 
				UPDATE SET 
					UpdatedDate = getUTCdate(), 
					IsActive = 1,
					ScheduleType = S.ScheduleType,
					Preferences = COALESCE(S.Preferences,T.Preferences);

--Update the Email jobs table by setting the nextrun for this subscription
MERGE EmailJobs T
			USING
			(SELECT UserId = @UserId, PartitionId = @PartitionId, SubscriptionType = @SubscriptionType, NextRunTime = dbo.getNextEmailJobRunTime(@ScheduleType)) S
				ON T.UserId = S.UserId AND T.PartitionId = S.PartitionId AND T.SubscriptionType = S.SubscriptionType
			WHEN NOT MATCHED BY TARGET THEN 
					INSERT   ( UserId, PartitionId, SubscriptionType, NextRunTime) 
					VALUES   ( S.UserId, S.PartitionId, S.SubscriptionType, S.NextRunTime)
			WHEN MATCHED THEN 
				UPDATE SET 
				    UpdatedDate = getUTCdate(), 
					NextRunTime = S.NextRunTime;
			
COMMIT TRANSACTION

SELECT * FROM dbo.MerchantSubscriptions WHERE UserId = @UserId AND PartitionId = @PartitionId AND SubscriptionType = @SubscriptionType

GO
