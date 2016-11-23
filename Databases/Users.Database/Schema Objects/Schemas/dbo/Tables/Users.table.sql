--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('Users') IS NOT NULL DROP TABLE Users
GO
CREATE TABLE Users
  (
     Id					uniqueidentifier	NOT NULL
    ,PartitionId		int					NOT NULL
    ,Name				nvarchar(50)		NULL
    ,Email				nvarchar(100)		NULL
	,PhoneNumber		nvarchar(50)		NULL
	,MsId				nvarchar(100)		NULL
    ,CreatedDate		datetime			NOT NULL  CONSTRAINT DF_Users_CreatedDate DEFAULT getUTCdate()
    ,UpdatedDate		datetime			NOT NULL  CONSTRAINT DF_Users_UpdatedDate DEFAULT getUTCdate()
	,IsSuppressed		bit					NOT NULL  
	,Json				nvarchar(max)		NULL
	,IsDeleted			bit					NOT NULL  CONSTRAINT DF_Users_IsDeleted DEFAULT 0
	,LinkedToUserId		uniqueidentifier	NULL  -- No need to have LinkedToUserPartitionId because it can be derived from LinkedToUserId
	,Source				nvarchar(50)		NULL
	,IsEmailConfirmed	bit					NOT NULL  CONSTRAINT DF_Users_IsEmailConfirmed DEFAULT 0			

     CONSTRAINT PKC_Users_Id_PartitionId PRIMARY KEY CLUSTERED (Id, PartitionId), 
  ) 
  --FEDERATED ON (PartitionId = PartitionId)
GO

CREATE UNIQUE NONCLUSTERED INDEX IXU_Email ON Users(Email) WHERE Email IS NOT NULL
GO

CREATE UNIQUE NONCLUSTERED INDEX IXU_MsId ON Users(MsId) WHERE MsId IS NOT NULL
GO

CREATE TRIGGER UsersUpdDel ON Users
FOR UPDATE, DELETE
AS
BEGIN
  INSERT INTO dbo.UsersHistory
      (
           Id
          ,PartitionId
          ,Name
          ,Email
		  ,PhoneNumber
          ,MsId
          ,CreatedDate
          ,UpdatedDate
          ,IsSuppressed
          ,Json
          ,IsDeleted
          ,LinkedToUserId
		  ,Source
		  ,IsEmailConfirmed
      )
    SELECT Id
          ,PartitionId
          ,Name
          ,Email
		  ,PhoneNumber
          ,MsId
          ,CreatedDate
          ,UpdatedDate
          ,IsSuppressed
          ,Json
          ,IsDeleted
          ,LinkedToUserId
		  ,Source
		  ,IsEmailConfirmed
      FROM deleted
    
  RETURN
END
GO