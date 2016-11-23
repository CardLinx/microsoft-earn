--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('EmailSubscriptions') IS NOT NULL DROP TABLE Subscriptions
GO
CREATE TABLE EmailSubscriptions
  (
	UserId      uniqueidentifier NOT NULL
   ,PartitionId int              NOT NULL
   ,LocationId  nvarchar(200) NOT NULL
   ,IsActive    bit              NOT NULL  CONSTRAINT DF_EmailSubscriptions_IsActive DEFAULT 0
   ,SubscriptionType nvarchar(50) NOT NULL CONSTRAINT DF_EmailSubscriptions_SubscriptionType DEFAULT 'WeeklyDeals'
   ,CreatedDate datetime         NOT NULL  CONSTRAINT DF_EmailSubscriptions_CreatedDate DEFAULT getUTCdate()
   ,UpdatedDate datetime         NOT NULL  CONSTRAINT DF_EmailSubscriptions_UpdatedDate DEFAULT getUTCdate()
    
	CONSTRAINT PKC_Subscription_UserId_SubscriptionType_LocationId PRIMARY KEY CLUSTERED (UserId, SubscriptionType, LocationId, PartitionId)   
    ,CONSTRAINT FK_EmailSubscription_UserId_Users FOREIGN KEY (UserId, PartitionId) REFERENCES Users (Id, PartitionId)	
  )
  --FEDERATED ON (PartitionId = PartitionId)
GO
CREATE INDEX IX_LocationId ON EmailSubscriptions (LocationId)
GO
CREATE TRIGGER EmailSubscriptionsUpdDel ON EmailSubscriptions
FOR UPDATE, DELETE
AS
BEGIN
  INSERT INTO dbo.EmailSubscriptionsHistory
      (
           UserId
          ,PartitionId
          ,LocationId
          ,IsActive
		  ,SubscriptionType
          ,CreatedDate
          ,UpdatedDate
      )
    SELECT UserId
          ,PartitionId
          ,LocationId
          ,IsActive
		  ,SubscriptionType
          ,CreatedDate
          ,UpdatedDate
      FROM deleted
    
  RETURN
END
GO