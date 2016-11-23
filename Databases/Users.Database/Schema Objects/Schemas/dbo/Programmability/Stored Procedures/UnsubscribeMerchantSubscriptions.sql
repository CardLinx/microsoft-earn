--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE [dbo].[UnsubscribeMerchantSubscriptions]
	@UserId uniqueidentifier, @PartitionId int, @SubscriptionType nvarchar(50)
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'MerchantSubscriptions_'+convert(varchar(50), @UserId)

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' 

IF EXISTS (SELECT * FROM MerchantSubscriptions WHERE UserId = @UserId AND PartitionId = @PartitionId AND SubscriptionType = @SubscriptionType)
	BEGIN
	Update 
		dbo.MerchantSubscriptions 
	SET 
		MerchantSubscriptions.IsActive = 0,
		MerchantSubscriptions.Preferences = NULL,		
        MerchantSubscriptions.ScheduleType = NULL,
		MerchantSubscriptions.UpdatedDate = getUTCdate()
	WHERE 	
		UserId = @UserId AND PartitionId = @PartitionId AND SubscriptionType = @SubscriptionType

	Update
		dbo.EmailJobs
	SET
		EmailJobs.NextRunTime = NULL,
		EmailJobs.UpdatedDate = getUTCDate()
	WHERE 	
		UserId = @UserId AND PartitionId = @PartitionId AND SubscriptionType = @SubscriptionType

	END

COMMIT TRANSACTION
GO