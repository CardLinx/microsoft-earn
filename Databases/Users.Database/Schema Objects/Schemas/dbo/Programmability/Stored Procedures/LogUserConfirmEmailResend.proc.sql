--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('LogUserConfirmEmailResend') IS NOT NULL DROP PROCEDURE LogUserConfirmEmailResend
GO
CREATE PROCEDURE LogUserConfirmEmailResend
	@UserId uniqueidentifier, @PartitionId int, @EntityType tinyint
AS
	INSERT INTO UserConfirmEmailResendsHistory (UserId,  PartitionId,  EntityType)
								VALUES (@UserId, @PartitionId, @EntityType)
RETURN 0