--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('ExternalUsersHistory') IS NOT NULL DROP TABLE ExternalUsersHistory
GO
CREATE TABLE ExternalUsersHistory
  (
     HistoryDate     datetime         NOT NULL  CONSTRAINT DF_ExternalUsersHistory_HistoryDate DEFAULT getUTCdate()
    ,HistoryId       uniqueidentifier NOT NULL  CONSTRAINT DF_ExternalUsersHistory_HistoryId DEFAULT newid() -- The hope it is never needed, but just in case

    ,ExternalId      nvarchar(100)    NOT NULL
    ,PartitionId     int              NOT NULL
    ,AuthProvider    tinyint          NOT NULL
    ,UserId          uniqueidentifier NOT NULL
    ,CreatedDate     datetime         NOT NULL
    ,UpdatedDate     datetime         NOT NULL

     CONSTRAINT PKC_ExternalUsersHistory_HistoryId_PartitionId PRIMARY KEY CLUSTERED (HistoryDate, HistoryId, PartitionId)
  ) 
  --FEDERATED ON (PartitionId = PartitionId)
GO

CREATE INDEX IX_ExternalId_PartitionId ON ExternalUsersHistory (ExternalId, PartitionId)
GO