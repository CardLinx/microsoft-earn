--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('CreateNotExistingExternalUser') IS NOT NULL DROP PROCEDURE CreateExternalUser
GO
CREATE PROCEDURE CreateNotExistingExternalUser
  @UserId uniqueidentifier = null, @ExternalId nvarchar(100), @PartitionId int, @AuthProvider tinyint
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts
DECLARE @lock varchar(100) = 'ExternalUsers_'+@ExternalId+CONVERT(varchar, @AuthProvider);


BEGIN TRY
	BEGIN TRANSACTION
	EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on facebook id

	IF @UserId IS NULL
		BEGIN
			INSERT INTO dbo.ExternalUsers
				    ( ExternalId,  PartitionId,  AuthProvider )
			SELECT @ExternalId, @PartitionId, @AuthProvider
				WHERE NOT EXISTS (SELECT * FROM dbo.ExternalUsers WHERE ExternalId = isnull(@ExternalId,'') AND AuthProvider = isnull(@AuthProvider,'') AND PartitionId = isnull(@PartitionId,-1))
		END
	ELSE
		BEGIN	
			IF EXISTS (SELECT * FROM dbo.ExternalUsers WHERE ExternalId = isnull(@ExternalId,'') AND AuthProvider = isnull(@AuthProvider,'') AND PartitionId = isnull(@PartitionId,-1) AND NOT UserId = isnull(@UserId,'00000000-0000-0000-0000-000000000000')) 
				BEGIN
					DECLARE @orig_user_id varchar(70);
					set @orig_user_id = (SELECT top 1 UserId FROM dbo.ExternalUsers WHERE ExternalId = isnull(@ExternalId,'') AND AuthProvider = isnull(@AuthProvider,'') AND PartitionId = isnull(@PartitionId,-1))
					DECLARE @cur_time varchar(12) = convert(time,getutcdate())

					DECLARE @msg varchar(300) = 'Cant Create External User with ext id='+convert(varchar, @ExternalId)+
												' auth provider='+ convert(varchar,@AuthProvider) + 
												' user id='+ convert(varchar(40),@UserId) +
												' current time='+ @cur_time +
												' the provider external user already in the system with the id ' +@orig_user_id
					RAISERROR(@msg, 18, 127)
				END
			ELSE
				BEGIN
					INSERT INTO dbo.ExternalUsers
					( ExternalId,  PartitionId,  AuthProvider, UserId)
						SELECT @ExternalId, @PartitionId, @AuthProvider, @UserId
						WHERE NOT EXISTS (SELECT * FROM dbo.ExternalUsers WHERE ExternalId = isnull(@ExternalId,'') AND AuthProvider = isnull(@AuthProvider,'') AND PartitionId = isnull(@PartitionId,-1))
				END
		END
	COMMIT TRANSACTION
	SELECT * FROM dbo.ExternalUsers WHERE ExternalId = isnull(@ExternalId,'') AND AuthProvider = isnull(@AuthProvider,'') AND PartitionId = isnull(@PartitionId,-1)
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
        ROLLBACK TRAN --RollBack in case of Error
		
	DECLARE @ErrorMessage VARCHAR(1000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
	
	SELECT 
	    @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();
		
   RAISERROR(@ErrorMessage,@ErrorSeverity ,@ErrorState)
END CATCH

GO
