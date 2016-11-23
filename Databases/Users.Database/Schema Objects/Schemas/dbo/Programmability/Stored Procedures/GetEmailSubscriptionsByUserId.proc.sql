--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetEmailSubscriptionsByUserId') IS NOT NULL DROP PROCEDURE GetEmailSubscriptionsByUser
GO
CREATE PROCEDURE GetEmailSubscriptionsByUserId
  @UserId uniqueidentifier, @PartitionId int, @IsActive bit = NULL, @SubscriptionType nvarchar(50) = NULL
AS
set nocount on	
IF (@IsActive IS NULL)
BEGIN
		SELECT *
		FROM dbo.EmailSubscriptionsCurrentView sub 
		WHERE sub.UserId = @UserId AND sub.PartitionId = @PartitionId AND (@SubscriptionType IS NULL OR sub.SubscriptionType = @SubscriptionType)
END
ELSE
BEGIN
	SELECT *
	FROM dbo.EmailSubscriptionsCurrentView sub
	WHERE sub.UserId = @UserId AND sub.PartitionId = @PartitionId AND IsActive = @IsActive AND (@SubscriptionType IS NULL OR sub.SubscriptionType = @SubscriptionType)
END
GO