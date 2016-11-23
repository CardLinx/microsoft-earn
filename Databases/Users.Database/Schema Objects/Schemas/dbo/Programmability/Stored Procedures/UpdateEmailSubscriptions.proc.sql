--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('UpdateEmailSubscriptions') IS NOT NULL DROP PROCEDURE UpdateEmailSubscriptions
CREATE PROCEDURE UpdateEmailSubscriptions
  @UserId uniqueidentifier, @PartitionId int, @SubscriptionsList SubscriptionListType readonly, @SubscriptionType nvarchar(50)
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'EmailSubscription_'+convert(varchar(50), @UserId)

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on id


MERGE EmailSubscriptions T
			USING
			(SELECT UserId = @UserId, PartitionId = @PartitionId, SubscriptionType = @SubscriptionType,LocationId = subList.LocationId, IsActive = subList.IsActive FROM @SubscriptionsList as subList) S
				ON T.UserId = S.UserId AND T.PartitionId = S.PartitionId AND T.LocationId = S.LocationId AND T.SubscriptionType = S.SubscriptionType
			WHEN NOT MATCHED BY TARGET 
				THEN 
					INSERT   ( UserId, PartitionId, LocationId, IsActive, SubscriptionType) 
					VALUES   ( S.UserId, S.PartitionId, S.LocationId, S.IsActive, S.SubscriptionType)
			WHEN MATCHED 
				THEN UPDATE SET 
					UpdatedDate = getUTCdate(), 
					IsActive = S.IsActive
			WHEN NOT MATCHED BY SOURCE AND T.UserId = @UserId AND T.PartitionId = @PartitionId AND T.SubscriptionType = @SubscriptionType
				THEN UPDATE SET 
					UpdatedDate = getUTCdate(), 
					IsActive = 0;
			

COMMIT TRANSACTION

SELECT * FROM dbo.EmailSubscriptionsCurrentView WHERE UserId = @UserId AND PartitionId = @PartitionId 
GO
