--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetConfirmEmailResendCountByUser') IS NOT NULL DROP PROCEDURE GetConfirmEmailResendCountByUser
GO
CREATE PROCEDURE GetConfirmEmailResendCountByUser
	@UserId uniqueidentifier, @PartitionId int, @EntityType tinyint, @FromDateTime datetime
AS
	SELECT COUNT(*) 
	FROM UserConfirmEmailResendsHistory 
	WHERE UserId = @UserId 
	AND @PartitionId = PartitionId 
	AND @EntityType = EntityType 
	AND CreatedDate >= @FromDateTime
