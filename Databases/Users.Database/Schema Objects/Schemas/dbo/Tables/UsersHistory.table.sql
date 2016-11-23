--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('UsersHistory') IS NOT NULL DROP TABLE UsersHistory
GO
CREATE TABLE UsersHistory
  (
     HistoryDate		datetime			NOT NULL  CONSTRAINT DF_UsersHistory_HistoryDate DEFAULT getUTCdate()
    ,HistoryId			uniqueidentifier	NOT NULL  CONSTRAINT DF_UsersHistory_HistoryId DEFAULT newid() -- The hope it is never needed, but just in case
 
    ,Id					uniqueidentifier	NOT NULL
    ,PartitionId		int					NOT NULL
    ,Name				nvarchar(50)		NULL
    ,Email				nvarchar(100)		NULL
	,PhoneNumber		nvarchar(50)		NULL
	,MsId				nvarchar(100)		NULL
    ,CreatedDate		datetime			NOT NULL
    ,UpdatedDate		datetime			NOT NULL
	,IsSuppressed		bit					NOT NULL  
	,Json				nvarchar(max)		NULL
	,IsDeleted			bit					NOT NULL
    ,LinkedToUserId		uniqueidentifier	NULL
	,Source				nvarchar(50)		NULL
	,IsEmailConfirmed	bit					NOT NULL  
    
     CONSTRAINT PKC_UsersHistory_HistoryId_PartitionId PRIMARY KEY CLUSTERED (HistoryDate, HistoryId, PartitionId)
  ) 
  --FEDERATED ON (PartitionId = PartitionId)
GO
CREATE INDEX IX_Id_PartitionId ON UsersHistory (Id, PartitionId)
GO