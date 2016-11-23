--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('ConfirmEntity') IS NOT NULL DROP PROCEDURE ConfirmEntity
CREATE PROCEDURE ConfirmEntity
	@UserIdHash			nvarchar(128),
	@PartitionId		int,
	@EntityType			tinyint,
	@Code				int
AS

set nocount on

DECLARE @lock varchar(100) = 'ConfirmEntity_'+ @UserIdHash;

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on entity id

IF EXISTS (SELECT * FROM ConfirmationCodes WHERE UserIdHash = @UserIdHash AND PartitionId = @PartitionId AND EntityType = @EntityType)
BEGIN
	WITH RowToUpdate AS
	(
		SELECT TOP(1) *
		FROM ConfirmationCodes
		WHERE
			ConfirmationCodes.UserIdHash = @UserIdHash AND 
			ConfirmationCodes.PartitionId = @PartitionId AND 
			ConfirmationCodes.EntityType = @EntityType 
		ORDER BY CreatedDate desc
	)
	UPDATE RowToUpdate
		SET RetryCount = RetryCount - 1,
			UpdatedDate = getUTCdate()
	
	SELECT TOP 1
		EntityId,
		PartitionId,
		EntityType,
		UserId,
		CASE 
            WHEN (RetryCount > -1 AND ExpiredDate > getUTCdate())
               THEN 1 
               ELSE 0 
		END as IsValid,
		CASE 
            WHEN (Code = @Code)
               THEN 1 
               ELSE 0 
		END as IsConfirmed	 
	FROM ConfirmationCodes
	WHERE
		UserIdHash = @UserIdHash AND
		PartitionId = @PartitionId AND 
		EntityType = @EntityType  
	ORDER BY CreatedDate desc
END
ELSE
BEGIN
	SELECT NULL AS EntityId, @PartitionId AS PartitionId, @EntityType AS EntityType, NULL AS UserId, 0 AS IsValid, 0 AS IsConfirmed
END

COMMIT TRANSACTION