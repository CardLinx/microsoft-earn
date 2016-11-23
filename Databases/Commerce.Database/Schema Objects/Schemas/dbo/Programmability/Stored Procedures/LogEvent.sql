--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- This stored procedure is used for base logging
--DROP PROCEDURE LogEvent    
GO
CREATE PROCEDURE LogEvent    
  (    
     @Process         varchar(100)
    ,@Status          varchar(10)
    ,@Mode            varchar(100)   = NULL    
    ,@Action          varchar(20)    = NULL    
    ,@Target          varchar(100)   = NULL    
    ,@Rows            bigint         = NULL    
    ,@Milliseconds    int            = NULL
    ,@Text            nvarchar(3500) = NULL
    ,@ParentEventID   bigint         = NULL   
    ,@EventID         bigint         = NULL    OUTPUT
    ,@ReRaisError     bit            = 1
  )
AS
set nocount on
DECLARE @ErrorNumber  int           = error_number()
       ,@ErrorMessage varchar(1000) = ''
       ,@TranCount    int           = @@trancount
       ,@DoWork       bit           = 0
       ,@ErrorLevel   int

IF @ErrorNumber IS NOT NULL OR @Status IN ('Warn','Error')
  SET @DoWork = 1

IF @DoWork = 0
  SET @DoWork = CASE WHEN EXISTS (SELECT * FROM dbo.Parameters WHERE Id = isnull(@Process,'') AND Char = 'LogEvent') THEN 1 ELSE 0 END

IF @DoWork = 0
  RETURN

IF @ErrorNumber IS NOT NULL 
BEGIN 
  SET @ErrorLevel = error_severity()
  SET @ErrorMessage =     
         CASE     
           WHEN error_procedure() <> 'LogEvent'     
             THEN error_message()
                + ': Error '+convert(varchar,@ErrorNumber)    
                + ', Level '+convert(varchar,@ErrorLevel)    
                + ', State '+convert(varchar,error_state())    
                + CASE WHEN error_procedure() IS NOT NULL THEN ', Procedure '+error_procedure() ELSE '' END    
                + ', Line '+convert(varchar,error_line())    
           ELSE error_message()    
         END    
END

IF @TranCount > 0 AND @ErrorNumber IS NOT NULL ROLLBACK TRANSACTION

INSERT INTO dbo.EventLog    
    (    
         Process
        ,Status
        ,Mode
        ,Action
        ,Target
        ,Rows
        ,Milliseconds
        ,EventDate
        ,EventText
        ,ParentEventID
        ,SPID
        ,UserName
        ,HostName
    )    
  SELECT @Process
        ,@Status
        ,@Mode
        ,@Action
        ,@Target
        ,@Rows
        ,@Milliseconds
        ,EventDate = getUTCdate()
        ,Text = CASE 
                   WHEN @ErrorNumber IS NULL THEN @Text
                   ELSE isnull(@Text,'')+CASE WHEN isnull(@Text,'')<>'' THEN char(10) ELSE '' END+@ErrorMessage
                 END
        ,@ParentEventID
        ,@@SPID
        ,UserName = left(system_user, 64)
        ,HostName = host_name()
    
SET @EventID = scope_identity()
    
-- Restore @@trancount
IF @TranCount > 0 AND @ErrorNumber IS NOT NULL BEGIN TRANSACTION

IF @ErrorNumber IS NOT NULL AND isnull(@ReRaisError,1) = 1
  RAISERROR(@ErrorMessage,@ErrorLevel,1)
GO