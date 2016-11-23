--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('ExternalUsers') IS NOT NULL DROP TABLE ExternalUsers
GO
CREATE TABLE ExternalUsers
  (
     ExternalId      nvarchar(100)    NOT NULL
    ,PartitionId     int              NOT NULL
    ,AuthProvider    tinyint          NOT NULL -- 0 for facebook, 1 for email
    ,UserId          uniqueidentifier NOT NULL  CONSTRAINT DF_ExternalUsers_UserId DEFAULT newid()
    ,CreatedDate     datetime         NOT NULL  CONSTRAINT DF_ExternalUsers_CreatedDate DEFAULT getUTCdate()
    ,UpdatedDate     datetime         NOT NULL  CONSTRAINT DF_ExternalUsers_UpdatedDate DEFAULT getUTCdate()

     CONSTRAINT PKC_ExternalUsers_ExternalId_AuthProvider PRIMARY KEY CLUSTERED (ExternalId, AuthProvider, PartitionId)
  ) 
  --FEDERATED ON (PartitionId = PartitionId)
GO
CREATE INDEX IX_UserId ON ExternalUsers (UserId)
GO
CREATE TRIGGER ExternalUsersUpdDel ON ExternalUsers
FOR UPDATE, DELETE
AS
BEGIN
  INSERT INTO dbo.ExternalUsersHistory
      (
           ExternalId
          ,PartitionId
          ,AuthProvider
          ,UserId
          ,CreatedDate
          ,UpdatedDate
      )
    SELECT ExternalId
          ,PartitionId
          ,AuthProvider
          ,UserId
          ,CreatedDate
          ,UpdatedDate
      FROM deleted
    
  RETURN
END
GO