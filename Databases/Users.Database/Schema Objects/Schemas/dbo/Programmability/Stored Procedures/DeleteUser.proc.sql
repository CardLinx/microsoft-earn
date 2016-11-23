--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('DeleteUser') IS NOT NULL DROP PROCEDURE DeleteUser
GO
CREATE PROCEDURE DeleteUser
	@UserId uniqueidentifier, @PartitionId int
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

BEGIN TRANSACTION

	DELETE FROM EmailUnsubscribeUrls 
		WHERE UserId = @UserId AND PartitionId = @PartitionId;
	DELETE FROM EmailSubscriptions
		WHERE UserId = @UserId AND PartitionId = @PartitionId;
	DELETE FROM Users	
		WHERE Id = @UserId AND PartitionId = @PartitionId

COMMIT TRANSACTION
RETURN 0

