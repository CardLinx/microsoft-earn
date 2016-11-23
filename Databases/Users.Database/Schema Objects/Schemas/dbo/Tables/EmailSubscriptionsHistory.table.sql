--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('EmailSubscriptionsHistory') IS NOT NULL DROP TABLE EmailSubscriptionsHistory
GO
CREATE TABLE EmailSubscriptionsHistory
  (
     HistoryDate     datetime          NOT NULL  CONSTRAINT DF_EmailSubscriptionsHistory_HistoryDate DEFAULT getUTCdate()
    ,HistoryId       uniqueidentifier  NOT NULL  CONSTRAINT DF_EmailSubscriptionsHistory_HistoryId DEFAULT newid() -- The hope it is never needed, but just in case
    ,UserId      uniqueidentifier NOT NULL
   ,PartitionId int              NOT NULL
   ,LocationId  nvarchar(200) NOT NULL
   ,IsActive    bit              NOT NULL  
   ,CreatedDate datetime         NOT NULL  
   ,UpdatedDate datetime         NOT NULL  
      ,SubscriptionType nvarchar(50) NOT NULL CONSTRAINT DF_EmailSubscriptionsHistory_SubscriptionType DEFAULT 'WeeklyDeals'

     CONSTRAINT PKC_EmailSubscriptionsHistory_HistoryId_PartitionId PRIMARY KEY CLUSTERED (HistoryDate, HistoryId, PartitionId)
  ) 
  --FEDERATED ON (PartitionId = PartitionId)
GO
CREATE INDEX IX_UserId_PartitionId ON EmailSubscriptionsHistory (UserId, PartitionId)
GO