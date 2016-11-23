--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--DROP TABLE EventLog
GO
CREATE TABLE dbo.EventLog
  (
     EventID       bigint IDENTITY(1,1) NOT NULL
    ,LocalDate     AS dateadd(hour,datediff(hour,getUTCdate(),getdate()),EventDate)
    ,Process       varchar(100)         NOT NULL
    ,Status        varchar(10)          NOT NULL
    ,Mode          varchar(100)         NULL
    ,Action        varchar(20)          NULL
    ,Target        varchar(100)         NULL
    ,Rows          bigint               NULL
    ,Milliseconds  int                  NULL
    ,EventText     nvarchar(3500)       NULL
    ,EventDate     datetime             NOT NULL
    ,ParentEventID bigint               NULL
    ,SPID          smallint             NOT NULL
    ,UserName      varchar(64)          NOT NULL
    ,HostName      varchar(64)          NOT NULL
  ) 
GO
CREATE UNIQUE CLUSTERED INDEX IXUC_EventDate_EventID ON EventLog (EventDate, EventID)
GO
ALTER TABLE EventLog ADD CONSTRAINT PK_EventLog_EventID PRIMARY KEY NONCLUSTERED (EventID)
GO