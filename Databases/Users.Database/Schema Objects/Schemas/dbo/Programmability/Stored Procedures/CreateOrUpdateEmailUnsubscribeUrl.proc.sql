--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('EmailUnsubscribeUrls') IS NOT NULL DROP PROCEDURE EmailUnsubscribeUrls
GO
CREATE PROCEDURE CreateOrUpdateEmailUnsubscribeUrl
	@UserId uniqueidentifier, @PartitionId int, @UnsubscribeUrl nvarchar(max)
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

BEGIN TRANSACTION
MERGE dbo.EmailUnsubscribeUrls T
			USING (SELECT UserId = @UserId, PartitionId = @PartitionId, UnsubscribeUrl = @UnsubscribeUrl) S
				ON T.UserId = S.UserId AND T.PartitionId = S.PartitionId
			WHEN NOT MATCHED THEN 
				INSERT   (   UserId,   PartitionId,   UnsubscribeUrl) 
				VALUES   ( S.UserId, S.PartitionId, S.UnsubscribeUrl)
			WHEN MATCHED THEN
			   UPDATE SET UnsubscribeUrl = S.UnsubscribeUrl, UpdatedDate = getUTCdate();

COMMIT TRANSACTION
SELECT * FROM dbo.EmailUnsubscribeUrls WHERE UserId = @UserId AND PartitionId = @PartitionId
GO