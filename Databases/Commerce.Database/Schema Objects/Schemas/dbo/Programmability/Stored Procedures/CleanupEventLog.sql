--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--DROP PROCEDURE dbo.CleanupEventLog
GO
-- This sp keeps EventLog table small
CREATE PROCEDURE dbo.CleanupEventLog
  (                                              -- EventID = Change Sequence Number
     @WorkTest                char(1)  =  'W'  
  )
AS
set nocount on
DECLARE @SP                    varchar(100)
       ,@Mode                  varchar(100)
       ,@msg                   varchar(255)
       ,@Rows                  int
       ,@WaitTimeSecond        int
       ,@WaitTime              datetime
       ,@MaxDeleteRows         int
       ,@MaxAllowedRows        bigint
       ,@RetentionPeriodMinute int
       ,@DeletedEventID        bigint
       ,@CurrentEventID        bigint
       ,@EventDate             datetime
       ,@DeletedRows           int

SELECT @SP = 'CleanupEventLog'
      ,@Mode = 'W='+isnull(@WorkTest,'NULL')

SELECT @msg = 'Procedure '+@SP+' Starting in '''+@Mode+''' mode at '+ convert(varchar(22), getdate(),113)
RAISERROR(@msg,0,1) WITH NOWAIT
EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Start'

BEGIN TRY
  SET @DeletedRows = 1
  
  WHILE NOT EXISTS (SELECT * FROM dbo.Parameters WHERE Id = 'CleanupEventLogStatus' AND Char <> 'Go')
        AND @DeletedRows > 0
  BEGIN
    SET @MaxDeleteRows = (SELECT Number FROM dbo.Parameters WHERE Id = 'CleanupEventLogDeleteRows')
    IF @MaxDeleteRows IS NULL
      RAISERROR('Cannot get Parameter.CleanupEventLogDeleteRows',18,127)
    
    SET @MaxAllowedRows = (SELECT Number FROM dbo.Parameters WHERE Id = 'CleanupEventLogAllowedRows')
    IF @MaxAllowedRows IS NULL
      RAISERROR('Cannot get Parameter.CleanupEventLogAllowedRows',18,127)
    
    SET @RetentionPeriodMinute = (SELECT Number*24*60 FROM dbo.Parameters WHERE Id = 'CleanupEventLogRetentionDay')
    IF @RetentionPeriodMinute IS NULL
      RAISERROR('Cannot get Parameter.CleanupEventLogRetentionDay',18,127)
    
    SET @DeletedEventID = isnull((SELECT min(EventID) FROM dbo.EventLog),0) - 1
    IF @WorkTest = 'T' EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Run',@Target='@DeletedEventID',@Action='Set',@Text=@DeletedEventID
    
    -- Get Current EventID
    SET @CurrentEventID = isnull((SELECT max(EventID) FROM dbo.EventLog),0)
    IF @WorkTest = 'T' EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Run',@Target='@CurrentEventID',@Action='Set',@Text=@CurrentEventID
    
    SET @DeletedRows = 0
    
    -- Do anything only if...
    IF @CurrentEventID - @DeletedEventID > @MaxAllowedRows -- row check
    BEGIN
      -- Check how far in time @DeletedEventID + @MaxDeleteRows is
      SET @EventDate = 
            (SELECT EventDate
               FROM dbo.EventLog
               WHERE EventID = (SELECT min(EventID) FROM dbo.EventLog WHERE EventID >= @DeletedEventID + @MaxDeleteRows)
            )
      SET @msg = convert(varchar,@EventDate,113)
      IF @WorkTest = 'T' EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Run',@Target='@EventDate',@Action='Set',@Text=@msg
      
      IF datediff(minute,@EventDate,getUTCdate()) > @RetentionPeriodMinute -- time check
      BEGIN
        DELETE FROM dbo.EventLog 
          WHERE EventDate <= @EventDate -- Do not exceed row limit. It is not accurate if row limit is a small number, but it is OK
        SELECT @DeletedRows = @@rowcount
              ,@msg = 'EventDate <= '+convert(varchar,@EventDate,113)
        EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Run',@Target='EventLog',@Action='Delete',@Rows=@DeletedRows,@Text=@msg
      END -- time check
    END -- row check
  END -- While
  
  SELECT @msg = 'Procedure '+@SP+' Ending in '''+@Mode+''' mode at '+convert(varchar(22), getdate(),113)  
  RAISERROR(@msg,0,1) WITH NOWAIT
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='End'
END TRY
BEGIN CATCH
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error'
END CATCH
GO
--DROP PROCEDURE CleanupEventLog
--SELECT TOP 1000 * FROM EventLog WHERE Process = 'CleanupEventLog' AND EventDate >= '2012-04-25 20:38:13.090' ORDER BY EventID DESC
--CleanupEventLog @WorkTest = 'T'
--SELECT count(*) FROM EventLog
--UPDATE Parameters SET Number = 100 WHERE Id = 'CleanupEventLogDeleteRows'
--UPDATE Parameters SET Number = 10 WHERE Id = 'CleanupEventLogAllowedRows'
--UPDATE Parameters SET Number = 1.0/24/60/10 WHERE Id = 'CleanupEventLogRetentionDay'
--UPDATE Parameters SET Number = 60 WHERE Id = 'CleanupEventLogRetentionDay'
--SELECT * FROM Parameters