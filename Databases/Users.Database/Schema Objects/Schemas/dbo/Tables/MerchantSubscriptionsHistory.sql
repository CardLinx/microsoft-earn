--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[MerchantSubscriptionsHistory](
	[HistoryDate] [datetime] NOT NULL CONSTRAINT [DF_MerchantSubscriptionsHistory_HistoryDate]  DEFAULT (getutcdate()),
	[HistoryId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MerchantSubscriptionsHistory_HistoryId]  DEFAULT (newid()),
	[UserId] [uniqueidentifier] NOT NULL,
	[PartitionId] [int] NOT NULL,
	[SubscriptionType] [nvarchar](50) NOT NULL CONSTRAINT [DF_MerchantSubscriptionsHistory_SubscriptionType]  DEFAULT ('TransactionReport'),
	[IsActive] [bit] NOT NULL,
	[Preferences] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[ScheduleType] [nvarchar](50) NULL,
 CONSTRAINT [PKC_MerchantSubscriptionsHistory_HistoryDate_HistoryId_PartitionId] PRIMARY KEY CLUSTERED (HistoryDate, HistoryId, PartitionId)
)

GO