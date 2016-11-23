--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetEmailSubscriptionsByLocationId') IS NOT NULL DROP PROCEDURE GetEmailSubscriptionsByLocationId
GO
CREATE PROCEDURE GetEmailSubscriptionsByLocationId
  @LocationId nvarchar(200), @IsActive bit = NULL, @SubscriptionType nvarchar(50) = null
AS
set nocount on
IF (@IsActive IS NULL)
BEGIN
	SELECT *
	FROM dbo.EmailSubscriptionsCurrentView sub
	WHERE sub.LocationId = @LocationId AND (@SubscriptionType IS NULL OR sub.SubscriptionType = @SubscriptionType)
END
ELSE
BEGIN
	SELECT *
	FROM dbo.EmailSubscriptionsCurrentView sub 
	WHERE sub.LocationId = @LocationId AND IsActive = @IsActive AND (@SubscriptionType IS NULL OR sub.SubscriptionType = @SubscriptionType)
END
GO