--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[MerchantSubscriptions](
	[UserId] [uniqueidentifier] NOT NULL,
	[PartitionId] [int] NOT NULL,
	[SubscriptionType] [nvarchar](50) NOT NULL	CONSTRAINT [DF_MerchantSubscriptions_SubscriptionType]  DEFAULT ('TransactionReport'),
	[IsActive] [bit] NOT NULL	CONSTRAINT [DF_MerchantSubscriptions_IsActive]  DEFAULT ((1)),
	[Preferences] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL	CONSTRAINT [DF_MerchantSubscriptions_CreatedDate]  DEFAULT (getutcdate()),
	[UpdatedDate] [datetime] NOT NULL	CONSTRAINT [DF_MerchantSubscriptions_UpdatedDate]  DEFAULT (getutcdate()),
	[ScheduleType] [nvarchar](50) NULL,
 CONSTRAINT [PKC_MerchantSubscriptions_UserId_PartitionId_SubscriptionType] PRIMARY KEY CLUSTERED (UserId, PartitionId, SubscriptionType),
 CONSTRAINT [FK_MerchantSubscriptions_UserId_Users] FOREIGN KEY(UserId, PartitionId) REFERENCES Users(Id,PartitionId)
)

GO

CREATE TRIGGER [dbo].[MerchantSubscriptionsUpdDel] ON [dbo].[MerchantSubscriptions]
FOR UPDATE, DELETE
AS
BEGIN
  INSERT INTO dbo.MerchantSubscriptionsHistory
      (
           UserId
          ,PartitionId          
		  ,SubscriptionType
          ,IsActive
		  ,Preferences		  
          ,CreatedDate
          ,UpdatedDate
		  ,ScheduleType
      )
    SELECT UserId
          ,PartitionId
          ,SubscriptionType
          ,IsActive
		  ,Preferences
          ,CreatedDate
          ,UpdatedDate
		  ,ScheduleType
      FROM deleted
    
  RETURN
END
GO


