--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetEmailSubscriptions') IS NOT NULL DROP PROCEDURE GetEmailSubscriptionsByInternalIdRange
GO
CREATE PROCEDURE GetEmailSubscriptions  @Take int = 50
										,@IsActive bit
										,@FromPartitionId int = null
										,@FromUserId uniqueidentifier = null
										,@FromLocationId nvarchar(200) = null
										,@SubscriptionType nvarchar(50) = null
AS
set nocount on
IF @FromPartitionId IS NULL OR @FromUserId IS NULL OR @FromLocationId IS NULL
BEGIN
	IF @FromPartitionId IS NULL AND @FromUserId IS NULL AND @FromLocationId IS NULL
		BEGIN
			SELECT TOP(@Take) *
			FROM dbo.EmailSubscriptionsCurrentView sub
			WHERE IsActive = @IsActive AND (@SubscriptionType IS NULL OR sub.SubscriptionType = @SubscriptionType)
			ORDER BY sub.UserId, sub.LocationId, sub.PartitionId
		END
	ELSE
		BEGIN 
			RAISERROR('FromPartitionId, FromUserId and FromLocationId all should be null or none should be null', 18, 127)
		END
END
ELSE
BEGIN
	SELECT TOP(@Take) *
	FROM dbo.EmailSubscriptionsCurrentView sub 
	WHERE IsActive = @IsActive
		AND
		(sub.UserId > @FromUserId 
			OR (sub.UserId = @FromUserId AND sub.LocationId > @FromLocationId)
			OR (sub.UserId = @FromUserId AND sub.LocationId = @FromLocationId AND sub.PartitionId > @FromPartitionId))
		AND (@SubscriptionType IS NULL OR sub.SubscriptionType = @SubscriptionType)
	ORDER BY sub.UserId, sub.LocationId, sub.PartitionId
END
GO