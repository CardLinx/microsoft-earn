--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetEmailUnsubscribeUrlByUserId') IS NOT NULL DROP PROCEDURE GetEmailUnsubscribeUrlByUserId
GO
CREATE PROCEDURE GetEmailUnsubscribeUrlByUserId
	 @UserId uniqueidentifier, @PartitionId int
AS
set nocount on
SELECT unsub.UserId, unsub.UnsubscribeUrl, unsub.UpdatedDate, usr.Email
	FROM dbo.EmailUnsubscribeUrls unsub 
		INNER JOIN dbo.ValidUsersView usr ON unsub.UserId = usr.Id AND unsub.PartitionId = usr.PartitionId
	WHERE unsub.UserId = @UserId AND unsub.PartitionId = @PartitionId
GO