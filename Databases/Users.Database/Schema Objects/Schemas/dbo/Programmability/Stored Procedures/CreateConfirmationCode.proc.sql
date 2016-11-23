--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('CreateConfirmationCode') IS NOT NULL DROP PROCEDURE CreateConfirmationCode
CREATE PROCEDURE CreateConfirmationCode
	@UserIdHash			nvarchar(128),
	@PartitionId		int,
	@EntityId			nvarchar(100),
	@EntityType			tinyint,
	@UserId				uniqueidentifier,
    @MaxRetryCount		int,			
	@ExpiredDate		datetime2
AS

set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'ConfirmEntity_'+@UserIdHash;

DECLARE @dateNow datetime2 = getUTCdate();

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on entity id

BEGIN 
	IF NOT EXISTS (SELECT * FROM 
	               (SELECT TOP(1) *	FROM ConfirmationCodes 
						WHERE ConfirmationCodes.UserIdHash = @UserIdHash 
							AND ConfirmationCodes.PartitionId = @PartitionId 
							AND ConfirmationCodes.EntityType = @EntityType	
						ORDER BY CreatedDate desc) AS r
                	Where r.EntityId = @EntityId AND (r.RetryCount > -1 AND r.ExpiredDate > @dateNow))
	BEGIN
		INSERT INTO ConfirmationCodes (	 EntityId,  PartitionId,  EntityType,  UserIdHash,  UserId,	   					     Code,     RetryCount,  ExpiredDate)
		                       VALUES (	@EntityId, @PartitionId, @EntityType, @UserIdHash, @UserId, CAST(RAND() * 1000000 AS INT), @MaxRetryCount, @ExpiredDate)
	END				
END
SELECT TOP 1 * FROM ConfirmationCodes
			   WHERE EntityId = @EntityId 
					AND PartitionId = @PartitionId 
					AND EntityType = @EntityType 
					AND UserId = @UserId 
					AND RetryCount > -1 
					AND ExpiredDate > @dateNow 
			   ORDER BY CreatedDate desc

COMMIT TRANSACTION