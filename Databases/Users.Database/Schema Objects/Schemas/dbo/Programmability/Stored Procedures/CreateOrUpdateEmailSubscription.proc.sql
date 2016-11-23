--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('CreateOrUpdateEmailSubscription') IS NOT NULL DROP PROCEDURE CreateOrUpdateEmailSubscription
GO
CREATE PROCEDURE CreateOrUpdateEmailSubscription
  @UserId uniqueidentifier, @PartitionId int, @LocationId nvarchar(200), @IsActive bit, @SubscriptionType nvarchar(50)
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'EmailSubscription_'+convert(varchar(50), @UserId)

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on id

		MERGE dbo.EmailSubscriptions T
			USING (SELECT UserId = @UserId, PartitionId = @PartitionId, SubscriptionType = @SubscriptionType, LocationId = @LocationId, IsActive = @IsActive) S
				ON T.UserId = S.UserId AND T.PartitionId = S.PartitionId AND T.LocationId = S.LocationId AND T.SubscriptionType = S.SubscriptionType
			WHEN NOT MATCHED THEN 
				INSERT   (   UserId,   PartitionId,   LocationId,	  IsActive, SubscriptionType) 
				VALUES   ( S.UserId, S.PartitionId, S.LocationId,   S.IsActive, S.SubscriptionType)
			WHEN MATCHED THEN
			   UPDATE 
				SET T.IsActive = S.IsActive,
					T.UpdatedDate = getUTCdate();

COMMIT TRANSACTION

SELECT es.*, Version = 0 FROM dbo.EmailSubscriptions es  WHERE UserId = @UserId AND PartitionId = @PartitionId AND LocationId = @LocationId AND SubscriptionType = @SubscriptionType

GO
