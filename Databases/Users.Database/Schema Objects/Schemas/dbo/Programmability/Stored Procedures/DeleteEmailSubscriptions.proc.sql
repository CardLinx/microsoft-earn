--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('DeleteEmailSubscriptions') IS NOT NULL DROP PROCEDURE UnsubscribeUser
CREATE PROCEDURE DeleteEmailSubscriptions
	@UserId uniqueidentifier, @PartitionId int, @LocationId nvarchar(200) = null, @SubscriptionType nvarchar(50) = null
AS
set nocount on
BEGIN
	Update 
		dbo.EmailSubscriptions 
	SET 
		EmailSubscriptions.IsActive = 0,
		EmailSubscriptions.UpdatedDate = getUTCdate()
	WHERE 	
		UserId = @UserId AND PartitionId = @PartitionId AND (@LocationId IS NULL OR LocationId = @LocationId) AND (@SubscriptionType IS NULL OR SubscriptionType = @SubscriptionType)
END
RETURN 0

