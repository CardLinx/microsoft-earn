--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
--INSERT INTO Parameters (Id, Char) SELECT 'CreateOrUpdateUser', 'LogEvent'
GO
--IF object_id('CreateOrUpdateUser') IS NOT NULL DROP PROCEDURE CreateOrUpdateUser
GO
CREATE PROCEDURE CreateOrUpdateUser
  @Id uniqueidentifier, @PartitionId int, @MsId nvarchar(100) = null, @Email nvarchar(100) = null, @PhoneNumber nvarchar(50) = null, @Name nvarchar(50) = null, @Json nvarchar(max) = null,
  @Source nvarchar(50) = null, @IsEmailConfirmed bit = null
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @lock varchar(100) = 'Users_'+convert(varchar(50),@Id)
       ,@EmailChanged bit
       ,@st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'CreateOrUpdateUser'
       ,@Rows int = 0
       ,@Mode varchar(100) = convert(varchar(36),@id)
       ,@rc int
       ,@msg varchar(1000)
BEGIN TRY
  BEGIN TRANSACTION

  -- this prevents racing conditions on id
  -- wait not mpore than 1 second
  EXECUTE @rc = sp_getapplock @Resource = @lock, @LockMode = 'exclusive', @LockTimeout = 1000
  IF @rc < 0 
  BEGIN
    SET @msg = 'Error running sp_getapplock. Returned code = '+convert(varchar(10),@rc)
      RAISERROR(@msg, 18, 127)
  END

  IF EXISTS (SELECT * FROM dbo.Users t WHERE t.Email = isnull(@Email,'') AND t.IsSuppressed = 1) RAISERROR('Cant update a suppressed user',18,127)

  IF @Email IS NULL
     OR EXISTS (SELECT * FROM dbo.Users WHERE Id = isnull(@Id,'00000000-0000-0000-0000-000000000000') AND PartitionId = isnull(@PartitionId,-1) AND Email = isnull(@Email,''))
    SET @EmailChanged = 0
  ELSE
    SET @EmailChanged = 1
		
  MERGE dbo.Users T
    USING (SELECT Email = @Email, MsId = @MsId, Name = @Name, PhoneNumber = @PhoneNumber, Id = isnull(@Id,'00000000-0000-0000-0000-000000000000'), PartitionId = isnull(@PartitionId,-1), Json = @Json,
			IsEmailConfirmed = @IsEmailConfirmed) S
    ON T.Id = S.Id AND T.PartitionId = S.PartitionId
    WHEN NOT MATCHED THEN 
      INSERT   (   Id,   PartitionId,   MsId,	  Email,   PhoneNumber, IsSuppressed,   Json,  Source, IsEmailConfirmed ) 
	  --If IsEmailConfirmed is null - assign 0
        VALUES ( S.Id, S.PartitionId, S.MsId, S.Email, S.PhoneNumber,            0,	  S.Json, @Source,  COALESCE(S.IsEmailConfirmed,0))
    WHEN MATCHED 
         AND ((S.Email IS NOT NULL AND (T.Email IS NULL OR NOT S.Email = T.Email))
              OR (S.MsId IS NOT NULL AND (T.MsId IS NULL OR NOT S.MsId = T.MsId))
              OR (S.Json IS NOT NULL AND (T.Json IS NULL OR NOT S.Json = T.Json))
              OR (S.PhoneNumber IS NOT NULL AND (T.PhoneNumber IS NULL OR NOT S.PhoneNumber = T.PhoneNumber)
			  OR (S.IsEmailConfirmed IS NOT NULL AND (T.IsEmailConfirmed IS NULL OR NOT S.IsEmailConfirmed = T.IsEmailConfirmed)))
             ) THEN 
      UPDATE 
        SET Email  = COALESCE(S.Email, T.Email)
           ,MsId   = COALESCE(S.MsId, T.MsId)
           ,PhoneNumber = COALESCE(S.PhoneNumber, T.PhoneNumber)
           ,Json   = COALESCE(S.Json, T.Json)
		   --if the email changed or the not yet confirmed we use the input parameter (if not null) otherwise we ignore it.
		   ,IsEmailConfirmed = CASE WHEN @EmailChanged = 1 OR NOT T.IsEmailConfirmed = 1  THEN COALESCE(S.IsEmailConfirmed, T.IsEmailConfirmed) ELSE 1 END
           ,UpdatedDate = getUTCdate()
  ;
  SET @Rows = @Rows + @@rowcount

  MERGE dbo.EmailUnsubscribeUrls T
    USING (SELECT Id = isnull(@Id,'00000000-0000-0000-0000-000000000000'), PartitionId = isnull(@PartitionId,-1), Email = @Email) S
    ON T.UserId = S.Id AND T.PartitionId = S.PartitionId
    -- user's email is potentially updated. reset the unsubscribe url value
    WHEN MATCHED AND (@EmailChanged = 1) THEN
      UPDATE
        SET T.UnsubscribeUrl = NULL
           ,T.UpdatedDate = getUTCdate()
        --add entry to the EmailUnsubscribeUrls if neeed 
    WHEN NOT MATCHED THEN
      INSERT ( UserId,   PartitionId, UnsubscribeUrl)
        VALUES  (S.Id, S.PartitionId,           NULL)
  ;
  SET @Rows = @Rows + @@rowcount
		
  COMMIT TRANSACTION

  SELECT * FROM dbo.Users WHERE Id = @Id
  SET @Rows = @Rows + @@rowcount

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO
--SELECT * FROM EventLog ORDER BY EventDate DESC