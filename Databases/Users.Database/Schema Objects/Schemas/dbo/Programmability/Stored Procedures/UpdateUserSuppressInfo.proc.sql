--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('UpdateUserSuppressInfo') IS NOT NULL DROP PROCEDURE SuppressUser
GO
CREATE PROCEDURE UpdateUserSuppressInfo
  @Id uniqueidentifier, @PartitionId int, @Email nvarchar(100), @IsSuppressed bit
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'Users_'+convert(varchar(50),@Id)

BEGIN TRANSACTION

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on id

IF EXISTS(SELECT 1 From dbo.Users Where Email = @Email AND  Id = @Id AND PartitionId = @PartitionId) 
BEGIN 
	UPDATE dbo.Users
		SET IsSuppressed = @IsSuppressed,
			UpdatedDate = getUTCdate()
		FROM dbo.Users 
		WHERE Email = @Email AND Id = @Id AND PartitionId = @PartitionId
END
ELSE
    -- Insert new user record with suppressed flag
	IF @IsSuppressed = 1
	BEGIN
		INSERT INTO dbo.Users (   Id,   PartitionId,   Email,	IsSuppressed) 
					   VALUES (  @Id,  @PartitionId,  @Email,	           1)
		INSERT INTO dbo.EmailUnsubscribeUrls (UserId,  PartitionId, UnsubscribeUrl)
							    	VALUES   (   @Id, @PartitionId,           NULL)
	END
   -- otherwise don't do anything
COMMIT TRANSACTION

SELECT * FROM dbo.Users WHERE Id = @Id AND PartitionId = @PartitionId AND Email = @Email
GO